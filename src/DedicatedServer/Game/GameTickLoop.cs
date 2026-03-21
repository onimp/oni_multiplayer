using System;
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
            if (_simSubTick == 0) {
                _tickSimDll();
                // After each 200ms SimDLL step, process nav dirty cells. Mirrors Game.UnsafeSim200ms()
                // which calls Pathfinding.Instance.UpdateNavGrids() after world.UpdateCellInfo().
                // Without this, tile changes from the sim never propagate to the nav graph.
                Pathfinding.Instance?.UpdateNavGrids();
            }
            // Advances all SIM_EVERY_TICK / SIM_33ms / SIM_200ms / SIM_1000ms / SIM_4000ms buckets
            Singleton<StateMachineUpdater>.Instance.AdvanceOneSimSubTick();
            _accumulatedTime -= SubTickTime;
        }

        // Drive RENDER_EVERY_TICK bucket — covers BrainScheduler (Dupe/Creature AI)
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
                Console.WriteLine($"[DS-005] ForceUpdateSensors: {brain.name} error: {e.GetBaseException().Message}");
            }
        }
        Console.WriteLine($"[DS-005] ForceUpdateSensors at tick={SensorWarmupTick}: updated {updated} brain(s)");
    }
}
