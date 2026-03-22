using System;
using System.Collections;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace DedicatedServer.Game;

/// <summary>
/// Drives game simulation — mirrors Game.SimEveryTick() for headless use.
///
/// Real game tick lifecycle (per Game.SimEveryTick):
///   simSubTick cycles 0-11 (12 subticks = 200ms of sim-time at 60fps)
///   Every 12th subtick: SimMessages.NewGameFrame → StepTheSim (SimDLL physics)
///   Every subtick: StateMachineUpdater.AdvanceOneSimSubTick (state machines, AI, sensors)
///
/// We mirror that exactly — one call to Update(dt) per frame advances both SimDLL and state machines.
/// </summary>
public class GameTickLoop {

    private const float SubTickTime = 1f / 60f;   // 16.67ms per subtick
    private const int SimFrameSubTicks = 12;       // SimDLL advances every 12 subticks (200ms)

    // Tick at which we force-refresh all dupe sensors to pick up warm PathGrid data.
    // IdleCellSensor runs once at tick≈5 (PathGrid cold → idleCell=-1) then never reruns.
    // By tick 60 (≈12s) AsyncPathProber has warmed PathGrid → sensors re-run → idleCell≠-1.
    private const int SensorWarmupTick = 60;
    // One tick after sensor warmup: force Brain.UpdateBrain() on all brains.
    // UpdateBrain → UpdateChores → FindBetterChore → choreConsumer.choreDriver.SetChore
    // → ChoreDriver transitions nochore→haschore → BeginChore → chore running.
    private const int ChoreKickTick = 61;

    // Reflection accessor for ChoreConsumer.providers (List<ChoreProvider>).
    // IL field: "providers". Server runs the exposed DLL where it is Public — must include
    // BindingFlags.Public so the lookup succeeds against both the exposed and original DLL.
    private static readonly FieldInfo _providersField =
        typeof(ChoreConsumer).GetField("providers",
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

    private float _accumulatedTime;
    private int _simSubTick;
    private int _tickCount;
    private readonly System.Action _tickSimDll;

    /// <param name="tickSimDll">Called every 200ms to advance SimDLL (pass WorldBuilder.TickSimulation)</param>
    public GameTickLoop(System.Action tickSimDll) {
        _tickSimDll = tickSimDll;
    }

    /// <summary>Advance simulation by dt seconds. Call from main loop at ~60fps or any rate.</summary>
    public void Update(float dt) {
        var clampedDt = Mathf.Min(dt, 0.2f);
        _accumulatedTime += clampedDt;
        _tickCount++;

        while (_accumulatedTime >= SubTickTime) {
            _simSubTick = (_simSubTick + 1) % SimFrameSubTicks;
            try {
                if (_simSubTick == 0) {
                    _tickSimDll();
                    // After each 200ms SimDLL step, process nav dirty cells. Mirrors Game.UnsafeSim200ms()
                    // which calls Pathfinding.Instance.UpdateNavGrids() after world.UpdateCellInfo().
                    // Without this, tile changes from the sim never propagate to the nav graph.
                    Pathfinding.Instance?.UpdateNavGrids();
                }
                // Advances all SIM_EVERY_TICK / SIM_33ms / SIM_200ms / SIM_1000ms / SIM_4000ms buckets
                Singleton<StateMachineUpdater>.Instance.AdvanceOneSimSubTick();
            } catch (Exception ex) {
                // BreathMonitor.IsLowBreath() → WorldContainer.AlertManager → NPE fires 563K× per run.
                // Without this catch the while-loop aborts → _tickCount never reaches SensorWarmupTick
                // → ForceUpdateSensors never fires → idleCell=-1 forever → no movement.
                // Log at low frequency to avoid console spam while still surfacing the root cause.
                if (_tickCount % 200 == 0)
                    Debug.LogWarning($"[GameTickLoop] SubTick ex (count={_tickCount}): {ex.GetBaseException().Message}");
            }
            _accumulatedTime -= SubTickTime;
        }

        // Drive RENDER_EVERY_TICK bucket — covers BrainScheduler (Dupe/Creature AI).
        // Render-only components (LightSymbolTracker etc.) are removed from this scheduler
        // in WorldBuilder.DisableRenderingOnlyComponents() — no try/catch needed here.
        Singleton<StateMachineUpdater>.Instance.RenderEveryTick(clampedDt);

        // Dispatch async path probe work orders to the background thread and apply completed
        // PathGrid results back to navigators. Mirrors Game.LateUpdate() where TickFrame()
        // is called every Unity frame. Without this, PathGrid costs are never populated
        // → Navigator.GetNavigationCost() always returns -1 → path-cost-based chore selection fails.
        AsyncPathProber.Instance?.TickFrame();

        // DS-005: force-refresh dupe sensors once PathGrid is warm.
        // IdleCellSensor.Update() runs once at tick≈5 when PathGrid is cold → idleCell=-1.
        // It only re-runs when Brain.onPreUpdate fires, which requires a chore to exist first
        // → deadlock: no idleCell → no chore → sensors never update → no idleCell.
        // Solution: at tick=SensorWarmupTick (60), forcibly call UpdateSensors() on all
        // dupe Sensors components. PathGrid is warm by then → idleCell≠-1 → IdleChore picked.
        // Components.Sensors doesn't exist — iterate via Components.Brains instead.
        if (_tickCount == SensorWarmupTick) {
            ForceUpdateSensors();
        }
        // tick=61: one frame after sensors warmed → force Brain.UpdateBrain() on every brain.
        // BrainScheduler may not have fired yet (suspended brains, cold start).
        // UpdateBrain → UpdateChores → FindBetterChore → choreDriver.SetChore
        // → ChoreDriver nochore→haschore → BeginChore → chore starts.
        if (_tickCount == ChoreKickTick) {
            ForceUpdateBrains();
        }
    }

    private static void ForceUpdateBrains() {
        Debug.LogWarning("[DS-005] ForceUpdateBrains ENTERING");
        var updated = 0;
        foreach (var brain in Components.Brains.Items) {
            if (brain == null) continue;
            try {
                var consumer = brain.GetComponent<ChoreConsumer>();
                var driver   = brain.GetComponent<ChoreDriver>();
                if (consumer == null || driver == null) {
                    Debug.LogWarning($"[Brain] {brain.name}: consumer={consumer != null} driver={driver != null} SKIP");
                    continue;
                }

                // Log providers list (private field) — who this consumer collects chores from.
                // Should have at least: own ChoreProvider + GlobalChoreProvider.
                var providerList = _providersField?.GetValue(consumer) as IList;
                var sb = new StringBuilder();
                if (providerList != null) {
                    foreach (var p in providerList)
                        sb.Append(p?.GetType().Name ?? "null").Append(' ');
                }

                // GetCurrentChore() lives on ChoreDriver.StatesInstance, not ChoreDriver itself.
                var smi = driver.GetSMI() as ChoreDriver.StatesInstance;
                var choreBefore = smi?.GetCurrentChore()?.GetType().Name ?? "null";

                Debug.LogWarning($"[Brain] {brain.name}: running={brain.IsRunning()} " +
                    $"choreBefore={choreBefore} " +
                    $"providers({providerList?.Count ?? -1})=[{sb.ToString().TrimEnd()}] " +
                    $"smi={(smi != null ? "ok" : "null")} smiRunning={smi?.IsRunning()}");

                brain.UpdateBrain();
                updated++;

                var choreAfter = (driver.GetSMI() as ChoreDriver.StatesInstance)?.GetCurrentChore()?.GetType().Name ?? "null";
                Debug.LogWarning($"[Brain] {brain.name}: choreAfter={choreAfter} (was {choreBefore})");
            } catch (Exception e) {
                Debug.LogWarning($"[Brain] {brain.name} ForceUpdateBrains EXCEPTION: {e}");
            }
        }
        Debug.LogWarning($"[DS-005] ForceUpdateBrains DONE: kicked {updated} brain(s)");
    }

    private static void ForceUpdateSensors() {
        var updated = 0;
        foreach (var brain in Components.Brains.Items) {
            if (brain == null) continue;
            var sensors = brain.GetComponent<Sensors>();
            if (sensors == null) continue;
            try {
                sensors.UpdateSensors();
                updated++;
            } catch (Exception e) {
                Debug.LogWarning($"[DS-005] ForceUpdateSensors: {brain.name} error: {e.GetBaseException().Message}");
            }
        }
        Debug.LogWarning($"[DS-005] ForceUpdateSensors at tick={SensorWarmupTick}: updated {updated} brain(s)");
    }
}
