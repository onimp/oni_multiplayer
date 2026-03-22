using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    // One tick after dupe brain kick: force-kick creature brains.
    // CreatureBrain uses the same Brain.UpdateChores() path as MinionBrain, but
    // ForceUpdateBrains() skips them when ChoreDriver is missing (init may fail in headless).
    // We kick them separately here so they get chores and begin moving.
    private const int CreatureChoreKickTick = ChoreKickTick + 1;

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
            if (_simSubTick == 0) {
                _tickSimDll();
                // After each 200ms SimDLL step, process nav dirty cells. Mirrors Game.UnsafeSim200ms()
                // which calls Pathfinding.Instance.UpdateNavGrids() after world.UpdateCellInfo().
                // Without this, tile changes from the sim never propagate to the nav graph.
                Pathfinding.Instance?.UpdateNavGrids();
            }
            // Advances all SIM_EVERY_TICK / SIM_33ms / SIM_200ms / SIM_1000ms / SIM_4000ms buckets.
            // No try/catch: BreathMonitor NPE root cause is fixed (WorldContainer.AlertManager
            // is properly initialized in WorldBuilder via smc.CreateSMIS()/StartSMIS()).
            // Any remaining exception here is a real bug that must surface, not be silenced.
            Singleton<StateMachineUpdater>.Instance.AdvanceOneSimSubTick();
            // Advance GameClock each subtick — mirrors SimAndRenderScheduler.sim33ms bucket.
            // Placed after AdvanceOneSimSubTick() in the normal code path (no finally needed
            // since AdvanceOneSimSubTick no longer throws after the AlertStateManager fix).
            GameClock.Instance?.Sim33ms(SubTickTime);
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

        // GameScheduler ticks: mirrors GameScheduler.Update() (private Unity callback).
        // IdleChore.Begin() → StateMachine.States root.idle.onfloor.AddScheduledCallback →
        // GameScheduler.Instance.Schedule("IdleMove", ...) registers a callback.
        // Without Update() being called, the Scheduler never dequeues entries → IdleMove
        // callback never fires → Navigator stays idle → dupe never moves.
        // GameScheduler.Instance is set in GameScheduler.OnPrefabInit() (called from
        // WorldBuilder.InitializeWorld). GetScheduler() exposes the internal Scheduler
        // (its Update() is public).
        GameScheduler.Instance?.GetScheduler()?.Update();

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
        // tick=62: kick creature brains one tick after dupe brains.
        // ForceUpdateBrains() skips creatures (they may lack ChoreDriver component
        // when ChoreConsumer.InitializeComponent() failed in headless). Handled here.
        if (_tickCount == CreatureChoreKickTick) {
            ForceUpdateCreatureBrains();
        }
        // Delayed chore check: SM transition nochore→haschore may not complete in same frame
        // as brain.UpdateBrain(). Check at tick=100 (39 ticks / ~650ms after ForceUpdateBrains).
        if (_tickCount == 100) {
            DelayedChoreCheck();
        }
    }

    private static void ForceUpdateBrains() {
        Debug.LogWarning("[DS-005] ForceUpdateBrains ENTERING");

        // Global chore inventory: log total chores in GlobalChoreProvider across all worlds.
        // choreWorldMap is public on ChoreProvider (base of GlobalChoreProvider).
        try {
            var gcp = GlobalChoreProvider.Instance;
            if (gcp != null) {
                var totalChores = 0;
                foreach (var kvp in gcp.choreWorldMap) totalChores += kvp.Value.Count;
                Debug.LogWarning($"[ChoreDebug] GlobalChoreProvider: choreWorldMap worlds={gcp.choreWorldMap.Count} totalChores={totalChores} fetches={gcp.fetches.Count}");
                // Log chore types present (first 10).
                var sb2 = new StringBuilder();
                var shown = 0;
                foreach (var kvp in gcp.choreWorldMap) {
                    foreach (var ch in kvp.Value) {
                        if (shown++ >= 10) break;
                        sb2.Append(ch?.GetType().Name ?? "null").Append(' ');
                    }
                }
                if (shown > 0) Debug.LogWarning($"[ChoreDebug] GlobalChoreProvider chore types: {sb2}");
            } else {
                Debug.LogWarning("[ChoreDebug] GlobalChoreProvider.Instance == null");
            }
        } catch (Exception e) {
            Debug.LogWarning($"[ChoreDebug] GlobalChoreProvider inspection failed: {e.GetBaseException().Message}");
        }

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

                // Log providers list.
                var providerList = _providersField?.GetValue(consumer) as IList;
                var sb = new StringBuilder();
                if (providerList != null)
                    foreach (var p in providerList) sb.Append(p?.GetType().Name ?? "null").Append(' ');

                var smi = driver.GetSMI() as ChoreDriver.StatesInstance;
                var choreBefore = smi?.GetCurrentChore()?.GetType().Name ?? "null";

                Debug.LogWarning($"[Brain] {brain.name}: running={brain.IsRunning()} " +
                    $"choreBefore={choreBefore} " +
                    $"providers({providerList?.Count ?? -1})=[{sb.ToString().TrimEnd()}] " +
                    $"smi={(smi != null ? "ok" : "null")} smiRunning={smi?.IsRunning()}");

                // Nav + cell diagnostic for Minions at tick=61 (after sensor warmup).
                // Hypothesis 1: nav.cachedCell != physicalCell → pathfinding uses wrong origin.
                // Hypothesis 2: idleCell == cachedCell → IdleChore finishes immediately (already there).
                if (brain.gameObject.HasTag(GameTags.BaseMinion)) {
                    try {
                        var nav2       = brain.GetComponent<Navigator>();
                        var sensors2   = brain.GetComponent<Sensors>();
                        var idleSensor = sensors2?.GetSensor<IdleCellSensor>();
                        var idleCell   = idleSensor?.GetCell() ?? -1;
                        var physCell   = Grid.PosToCell(brain.transform.position);
                        var navCell    = nav2?.cachedCell ?? -1;
                        var cdSmi      = driver?.GetSMI() as ChoreDriver.StatesInstance;
                        Debug.LogWarning($"[NavDiag] {brain.name}: physicalCell={physCell} nav.cachedCell={navCell} " +
                            $"idleCell={idleCell} sameAsPhys={physCell == navCell} idleIsPhys={idleCell == physCell} " +
                            $"choreDriverSmi={(cdSmi != null ? "ok" : "null")} choreDriverSmiRunning={cdSmi?.IsRunning()}");
                    } catch (Exception e) {
                        Debug.LogWarning($"[NavDiag] {brain.name} EXCEPTION: {e.GetBaseException().Message}");
                    }
                }

                // Deep per-provider diagnostic only for Minions (not critters).
                if (brain.gameObject.HasTag(GameTags.BaseMinion) && consumer.consumerState != null) {
                    try {
                        if (providerList != null) {
                            foreach (ChoreProvider provider in providerList) {
                                if (provider == null) continue;
                                var succeeded = new List<Chore.Precondition.Context>();
                                var failed    = new List<Chore.Precondition.Context>();
                                provider.CollectChores(consumer.consumerState, succeeded, failed);
                                Debug.LogWarning($"[ChoreDebug] {brain.name} provider={provider.GetType().Name} succeeded={succeeded.Count} failed={failed.Count}");
                                foreach (var ctx in succeeded.Take(3)) {
                                    // Log FullName + choreType.id to identify ChoreTableChore`2 actual type.
                                    Debug.LogWarning($"  -> chore={ctx.chore?.GetType().FullName} " +
                                        $"choreType.id={ctx.chore?.choreType?.Id} " +
                                        $"priority={ctx.masterPriority}");
                                }
                                if (succeeded.Count == 0 && failed.Count > 0) {
                                    foreach (var ctx in failed.Take(3))
                                        Debug.LogWarning($"  xx failed chore={ctx.chore?.GetType().Name} failedPreconditionId={ctx.failedPreconditionId}");
                                }
                            }
                        }
                    } catch (Exception e) {
                        Debug.LogWarning($"[ChoreDebug] {brain.name} CollectChores inspection failed: {e.GetBaseException().Message}");
                    }
                }

                // Reset SM error flags per-brain before FindNextChore:
                //   Instance.error  — global static; GoTo guard: if (Instance.error || ...) return;
                //   smi.isCrashed   — per-instance bool, set alongside Instance.error by Error().
                // Any GoTo() failure during spawn sets both. Instance.error was addressed in
                // aa4bb54; smi.isCrashed is reset here as a companion per-instance reset.
                var smiState = driver.smi?.GetCurrentState();
                Debug.LogWarning($"[Brain] {brain.name}: SMState PRE: " +
                    $"globalError={StateMachine.Instance.error} " +
                    $"smiCrashed={driver.smi?.isCrashed} " +
                    $"smiState={smiState?.name ?? "null"} " +
                    $"smiRunning={driver.smi?.IsRunning()}");

                StateMachine.Instance.error = false;
                if (driver.smi != null)
                    driver.smi.isCrashed = false;

                // If the SM isn't in nochore (wrong state or null), force it there so
                // SetChore's nextChore.Set() → ParamTransition → GoTo(haschore) has a clean base.
                if (driver.smi != null && smiState != driver.smi.sm.nochore) {
                    Debug.LogWarning($"[Brain] {brain.name}: SM not in nochore " +
                        $"(was '{smiState?.name ?? "null"}') — forcing GoTo(nochore)");
                    driver.smi.GoTo(driver.smi.sm.nochore);
                }

                // Direct FindNextChore instead of brain.UpdateBrain():
                // UpdateBrain() checks IsRunning() first — if false (brain not fully started in
                // fallback path) it's a no-op and no chore is ever assigned.
                // FindNextChore bypasses that guard and goes straight to provider.CollectChores →
                // ChooseChore → driver.SetChore, which transitions nochore→haschore immediately.
                var choreContext = default(Chore.Precondition.Context);
                var found = consumer.FindNextChore(ref choreContext);
                if (found) {
                    Debug.LogWarning($"[Brain] {brain.name}: FindNextChore FOUND " +
                        $"choreType={choreContext.chore?.GetType().Name} " +
                        $"choreType.id={choreContext.chore?.choreType?.Id} " +
                        $"isValid={choreContext.chore?.IsValid()}");
                    // DS-005 fix: Parameter<T>.Context.Set() has an equality guard —
                    //   if (!EqualityComparer<T>.Default.Equals(value, this.value)) { onDirty(smi) }
                    // If a previous blocked GoTo attempt already set nextChore to the same chore
                    // object, the subsequent Set(same chore) is a no-op → ParamTransition never fires
                    // → SM stays in nochore → BeginChore never runs → currentChore stays null.
                    // Fix: pre-clear nextChore to null to force a null→chore value change that
                    // always fires onDirty regardless of what nextChore held before.
                    if (driver.smi != null)
                        driver.smi.sm.nextChore.Set(null, driver.smi);
                    driver.SetChore(choreContext);
                    updated++;
                } else {
                    Debug.LogWarning($"[Brain] {brain.name}: FindNextChore returned false " +
                        $"(no chore available) running={brain.IsRunning()}");
                }

                var choreAfter = driver.GetCurrentChore()?.GetType().Name ?? "NULL";
                Debug.LogWarning($"[Brain] {brain.name}: POST-SetChore: currentChore={choreAfter} " +
                    $"(was {choreBefore}) " +
                    $"globalError={StateMachine.Instance.error} " +
                    $"smiCrashed={driver.smi?.isCrashed} " +
                    $"smiState={driver.smi?.GetCurrentState()?.name ?? "null"}");
            } catch (Exception e) {
                Debug.LogWarning($"[Brain] {brain.name} ForceUpdateBrains EXCEPTION: {e}");
            }
        }
        Debug.LogWarning($"[DS-005] ForceUpdateBrains DONE: kicked {updated} brain(s)");
    }

    /// <summary>
    /// Checks chore + movement state 39 ticks after ForceUpdateBrains.
    /// The nochore→haschore SM transition may not complete in the same frame as UpdateBrain().
    /// At tick=100 the transition should have fired and the navigator should be moving.
    /// </summary>
    private static void DelayedChoreCheck() {
        Debug.LogWarning("[DS-005] DelayedChoreCheck at tick=100");
        foreach (var brain in Components.Brains.Items) {
            if (brain == null) continue;
            if (!brain.gameObject.HasTag(GameTags.BaseMinion)) continue;
            try {
                var driver     = brain.GetComponent<ChoreDriver>();
                var nav        = brain.GetComponent<Navigator>();
                var sensors    = brain.GetComponent<Sensors>();
                var idleSensor = sensors?.GetSensor<IdleCellSensor>();
                var chore      = driver?.GetCurrentChore();
                var physCell   = Grid.PosToCell(brain.transform.position);
                var navCell    = nav?.cachedCell ?? -1;
                var idleCell   = idleSensor?.GetCell() ?? -1;
                // ChoreDriver.GetCurrentChore() is public: delegates to smi.sm.currentChore.Get(smi)
                Debug.LogWarning($"[Tick100] {brain.name}: " +
                    $"chore={chore?.GetType().FullName ?? "null"} " +
                    $"choreType.id={chore?.choreType?.Id ?? "null"} " +
                    $"isRunning={brain.IsRunning()} " +
                    $"physicalCell={physCell} nav.cachedCell={navCell} cellMatch={physCell == navCell} " +
                    $"idleCell={idleCell} idleIsPhys={idleCell == physCell} " +
                    $"nav.IsMoving={nav?.IsMoving()} " +
                    $"choreDriverSmi={(driver?.GetSMI() != null ? "ok" : "null")}");
            } catch (Exception e) {
                Debug.LogWarning($"[Tick100] {brain.name} EXCEPTION: {e.GetBaseException().Message}");
            }
        }
    }

    /// <summary>
    /// Force-kicks creature brains at tick=62, one frame after dupe brains are kicked.
    ///
    /// ForceUpdateBrains() skips all brains where ChoreDriver is missing via GetComponent
    /// (creatures whose ChoreConsumer.InitializeComponent() failed in headless, leaving
    /// choreConsumer.choreDriver=null). This method handles them separately:
    ///
    ///   1. Resets StateMachine.Instance.error (same guard as ForceUpdateBrains).
    ///   2. Resets ChoreDriver.smi.isCrashed if driver SM is present.
    ///   3. Calls consumer.FindNextChore() + driver.SetChore() to assign the first chore.
    ///   4. Falls back to brain.UpdateBrain() when consumer/driver component is absent
    ///      (pure SM-driven creatures with no ChoreDriver component on the GO).
    ///
    /// After this kick the creature's ChoreDriver SM transitions nochore→haschore →
    /// BeginChore → Navigator starts pathing → creature moves.
    /// </summary>
    internal static void ForceUpdateCreatureBrains() {
        Debug.LogWarning("[Animals] ForceUpdateCreatureBrains ENTERING");
        var updated = 0;
        var skipped = 0;

        foreach (var brain in Components.Brains.Items) {
            if (brain is not CreatureBrain) continue;
            var go = brain.gameObject;
            if (go == null) continue;

            try {
                if (!brain.IsRunning()) {
                    Debug.LogWarning($"[Animals] {brain.name}: SKIP running=false");
                    skipped++;
                    continue;
                }

                var consumer = brain.GetComponent<ChoreConsumer>();
                if (consumer == null) {
                    Debug.LogWarning($"[Animals] {brain.name}: SKIP no ChoreConsumer");
                    skipped++;
                    continue;
                }

                // Reset SM error flags — same pattern as ForceUpdateBrains for dupes.
                StateMachine.Instance.error = false;

                var driver = brain.GetComponent<ChoreDriver>();
                if (driver != null) {
                    // Mirror dupe path: reset isCrashed, ensure nochore state, then FindNextChore.
                    if (driver.smi != null)
                        driver.smi.isCrashed = false;

                    if (driver.smi != null && driver.smi.GetCurrentState() != driver.smi.sm.nochore) {
                        Debug.LogWarning($"[Animals] {brain.name}: SM not in nochore " +
                            $"(was '{driver.smi.GetCurrentState()?.name ?? "null"}') — forcing nochore");
                        driver.smi.GoTo(driver.smi.sm.nochore);
                    }

                    var context = default(Chore.Precondition.Context);
                    var found = consumer.FindNextChore(ref context);

                    var choreBefore = driver.GetCurrentChore()?.GetType().Name ?? "null";
                    if (found) {
                        // Pre-clear nextChore to force the equality guard to fire (same fix as dupes).
                        if (driver.smi != null)
                            driver.smi.sm.nextChore.Set(null, driver.smi);
                        driver.SetChore(context);
                        updated++;
                    }

                    var choreAfter = driver.GetCurrentChore()?.GetType().Name ?? "null";
                    var cell = Grid.PosToCell(go);
                    Debug.LogWarning($"[Animals] {brain.name}: cell={cell} " +
                        $"found={found} choreBefore={choreBefore} choreAfter={choreAfter} " +
                        $"smiState={driver.smi?.GetCurrentState()?.name ?? "null"}");
                } else {
                    // No ChoreDriver component: creature is purely SM-driven (rare).
                    // UpdateBrain() fires onPreUpdate (Navigator.UpdateProbe) which is all
                    // we can do — the SM itself will trigger chore assignment on next tick.
                    brain.UpdateBrain();
                    var cell = Grid.PosToCell(go);
                    Debug.LogWarning($"[Animals] {brain.name}: cell={cell} no ChoreDriver — UpdateBrain() called");
                    updated++;
                }

            } catch (Exception ex) {
                Debug.LogWarning($"[Animals] {brain.name}: ForceUpdateCreatureBrains EXCEPTION: " +
                    $"{ex.GetBaseException().Message}");
            }
        }

        Debug.LogWarning($"[Animals] ForceUpdateCreatureBrains DONE: kicked={updated} skipped={skipped}");
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
