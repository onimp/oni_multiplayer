using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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

    // Reflection accessor for StateMachineController.stateMachines (private List<StateMachine.Instance>).
    // Used to log the list's identity (hash) and contents per dupe at specific ticks so we can
    // pinpoint exactly WHEN the shared-list collapse happens (fresh list at Setup, same list at tick=100).
    private static readonly FieldInfo _smcStateMachinesField =
        typeof(StateMachineController)
            .GetField("stateMachines", BindingFlags.Instance | BindingFlags.NonPublic);

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

    // One-shot flag: set to true once idle.move enterActions+exitActions are all wrapped.
    // Checked every tick; after patching it becomes a single bool test (near-zero cost).
    private bool _idleChorePatchApplied;

    // Rolling 1-second UPS counter.
    // _upsTicks counts Update() calls in the current 1s window.
    // _upsStopwatch measures wall-clock time; when it exceeds 1s the window resets.
    private int _upsTicks;
    private readonly Stopwatch _upsStopwatch = Stopwatch.StartNew();

    /// <summary>Real updates-per-second measured over a rolling 1-second window.</summary>
    public int Ups { get; private set; }

    /// <param name="tickSimDll">Called every 200ms to advance SimDLL (pass WorldBuilder.TickSimulation)</param>
    public GameTickLoop(System.Action tickSimDll) {
        _tickSimDll = tickSimDll;
    }

    /// <summary>Advance simulation by dt seconds. Call from main loop at ~60fps or any rate.</summary>
    public void Update(float dt) {
        // Rolling 1s UPS: count calls in the current window; snapshot+reset when window expires.
        _upsTicks++;
        if (_upsStopwatch.ElapsedMilliseconds >= 1000) {
            Ups = _upsTicks;
            _upsTicks = 0;
            _upsStopwatch.Restart();
        }

        // Patch FIRST — before any SM tick can fire idle.move Enter/Exit actions.
        // PatchIdleChoreExitActions is a one-shot: cheap bool check after _idleChorePatchApplied=true.
        // Must run before AdvanceOneSimSubTick() so the wrapper is in place before the SM fires.
        PatchIdleChoreExitActions();

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

        // AsyncPathProber.TickFrame() is intentionally NOT called here.
        // Root cause of crash: PotentialScratchPad is sized from Pathfinding.MaxLinksPerCell()
        // at background worker thread start time (NavGrids minimal). After WorldBuilder loads the
        // save and calls UpdateNavGrids(), maxLinksPerCell grows → AddPotentials accesses
        // scratch.linksInCellRange[k] beyond original array size → ArgumentOutOfRangeException
        // stored in agentException → rethrown here via TickFrame() → server crash.
        // Without TickFrame(): background worker stays idle (NextTask always false → Sleep(1)).
        // TryBuildPathFromCache always misses → Navigator falls back to synchronous
        // PathFinder.UpdatePath() BFS on every navigation step. Fully supported: BrainScheduler
        // already sets executePathProbeTaskAsync=false for some navigators (same sync-only path).

        // GameScheduler ticks: mirrors GameScheduler.Update() (private Unity callback).
        // IdleChore.Begin() → StateMachine.States root.idle.onfloor.AddScheduledCallback →
        // GameScheduler.Instance.Schedule("IdleMove", ...) registers a callback.
        // Without Update() being called, the Scheduler never dequeues entries → IdleMove
        // callback never fires → Navigator stays idle → dupe never moves.
        // GameScheduler.Instance is set in GameScheduler.OnPrefabInit() (called from
        // WorldBuilder.InitializeWorld). GetScheduler() exposes the internal Scheduler
        // (its Update() is public).
        GameScheduler.Instance?.GetScheduler()?.Update();

        // Tick-level SMC list diagnostic: log listHash+count+item0 for each dupe at ticks
        // 1, 10, 61, 62, 100 to pinpoint WHEN the stateMachines list is replaced or shared.
        // item0 hash shows which IdleMonitor instance is at [0] — if all dupes show the same
        // item0 hash after initially having distinct ones, that tick is the collapse point.
        if (_tickCount == 1 || _tickCount == 10 || _tickCount == 61 ||
            _tickCount == 62 || _tickCount == 100) {
            DiagSmcStateMachines(_tickCount);
        }

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
            // Re-index any dupe ChoreProvider entries stored under world key -1.
            // IdleChore is added to ChoreProvider during IdleMonitor.OnSpawn (TriggerLifecycle),
            // when Grid.WorldIdx may not yet be populated → GetMyParentWorldId() returns -1.
            // CollectChores queries with the real world key → miss → no chores found.
            // After ForceUpdateBrains (which logs the miss), fix the keys so the
            // natural BrainScheduler.RenderEveryTick at tick=62+ can find them.
            ReindexChoreProviders();
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

        // Runtime diagnostic at tick=200: catch runtime state well past tick=0 transients.
        // Logs per-dupe: brain running, IdleMonitor state/error, providers count,
        // choreWorldMap count, and SM.error for all state machines.
        if (_tickCount == 200) {
            RuntimeDiagAt200();
        }

    }

    private static void ForceUpdateBrains() {
        Debug.LogWarning("[DS-005] ForceUpdateBrains ENTERING");

        // Global chore inventory: log total chores in GlobalChoreProvider across all worlds.
        // choreWorldMap is public on ChoreProvider (base of GlobalChoreProvider).
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

        var updated = 0;
        foreach (var brain in Components.Brains.Items) {
            if (brain == null) continue;
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
            if (brain.gameObject.HasTag(GameTags.BaseMinion)) {
                var nav2       = brain.GetComponent<Navigator>();
                var sensors2   = brain.GetComponent<Sensors>();
                var idleSensor = sensors2?.GetSensor<IdleCellSensor>();
                var idleCell   = idleSensor?.GetCell() ?? -1;
                var physCell   = Grid.PosToCell(brain.transform.position);
                var navCell    = nav2?.cachedCell ?? -1;
                var cdSmi      = driver.GetSMI() as ChoreDriver.StatesInstance;
                Debug.LogWarning($"[NavDiag] {brain.name}: physicalCell={physCell} nav.cachedCell={navCell} " +
                    $"idleCell={idleCell} sameAsPhys={physCell == navCell} idleIsPhys={idleCell == physCell} " +
                    $"choreDriverSmi={(cdSmi != null ? "ok" : "null")} choreDriverSmiRunning={cdSmi?.IsRunning()}");
            }

            // Deep per-provider diagnostic only for Minions (not critters).
            if (brain.gameObject.HasTag(GameTags.BaseMinion) && consumer.consumerState != null && providerList != null) {
                foreach (ChoreProvider provider in providerList) {
                    if (provider == null) continue;
                    var succeeded = new List<Chore.Precondition.Context>();
                    var failed    = new List<Chore.Precondition.Context>();
                    provider.CollectChores(consumer.consumerState, succeeded, failed);
                    Debug.LogWarning($"[ChoreDebug] {brain.name} provider={provider.GetType().Name} succeeded={succeeded.Count} failed={failed.Count}");
                    foreach (var ctx in succeeded.Take(3)) {
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

            // Reset SM error flags per-brain before FindNextChore.
            var smiState = driver.smi?.GetCurrentState();
            Debug.LogWarning($"[Brain] {brain.name}: SMState PRE: " +
                $"globalError={StateMachine.Instance.error} " +
                $"smiCrashed={driver.smi?.isCrashed} " +
                $"smiState={smiState?.name ?? "null"} " +
                $"smiRunning={driver.smi?.IsRunning()}");

            StateMachine.Instance.error = false;
            if (driver.smi != null)
                driver.smi.isCrashed = false;

            // If the SM isn't in nochore, force it there.
            if (driver.smi != null && smiState != driver.smi.sm.nochore) {
                Debug.LogWarning($"[Brain] {brain.name}: SM not in nochore " +
                    $"(was '{smiState?.name ?? "null"}') — forcing GoTo(nochore)");
                driver.smi.GoTo(driver.smi.sm.nochore);
            }

            var choreContext = default(Chore.Precondition.Context);
            var found = consumer.FindNextChore(ref choreContext);
            if (found) {
                Debug.LogWarning($"[Brain] {brain.name}: FindNextChore FOUND " +
                    $"choreType={choreContext.chore?.GetType().Name} " +
                    $"choreType.id={choreContext.chore?.choreType?.Id} " +
                    $"isValid={choreContext.chore?.IsValid()}");
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
        }
        Debug.LogWarning($"[DS-005] ForceUpdateBrains DONE: kicked {updated} brain(s)");
    }

    /// <summary>
    /// Checks chore + movement state 39 ticks after ForceUpdateBrains.
    /// The nochore→haschore SM transition may not complete in the same frame as UpdateBrain().
    /// <summary>
    /// Logs smc.stateMachines list identity (hash, count, item[0] hash) for each dupe at the
    /// given tick. Used to find the exact tick where the list is replaced/shared post-Setup.
    ///
    /// Reading: if listHash changes between tick=1 and tick=N, the list was replaced at tick N.
    ///          if item0 hash changes, a different SM is now at position 0 (prepended or swapped).
    ///          if all dupes share the same listHash at tick N, the lists were re-merged at tick N.
    /// </summary>
    private void DiagSmcStateMachines(int tick) {
        foreach (var brain in Components.Brains.Items) {
            if (brain == null) continue;
            if (!brain.gameObject.HasTag(GameTags.BaseMinion)) continue;
            var go  = brain.gameObject;
            var smc = go.GetComponent<StateMachineController>();
            var list = _smcStateMachinesField?.GetValue(smc) as IList;
            Console.WriteLine("[DIAG tick=" + tick + "] dupe=" + go.GetInstanceID() +
                " listHash=" + list?.GetHashCode() +
                " count=" + list?.Count +
                " item0=" + list?[0]?.GetHashCode());
        }
    }

    /// At tick=100 the transition should have fired and the navigator should be moving.
    /// </summary>
    private static void DelayedChoreCheck() {
        Debug.LogWarning("[DS-005] DelayedChoreCheck at tick=100");
        foreach (var brain in Components.Brains.Items) {
            if (brain == null) continue;
            if (!brain.gameObject.HasTag(GameTags.BaseMinion)) continue;
            var go = brain.gameObject;
            Console.WriteLine("[DS] Dupe SMC hash=" + go.GetComponent<StateMachineController>()?.GetHashCode() + " IdleMonitor=" + go.GetComponent<StateMachineController>()?.GetSMI<IdleMonitor.Instance>()?.GetHashCode());
            var driver     = brain.GetComponent<ChoreDriver>();
            var nav        = brain.GetComponent<Navigator>();
            var sensors    = brain.GetComponent<Sensors>();
            var idleSensor = sensors?.GetSensor<IdleCellSensor>();
            var chore      = driver?.GetCurrentChore();
            var physCell   = Grid.PosToCell(brain.transform.position);
            var navCell    = nav?.cachedCell ?? -1;
            var idleCell   = idleSensor?.GetCell() ?? -1;
            Debug.LogWarning($"[Tick100] {brain.name}: " +
                $"chore={chore?.GetType().FullName ?? "null"} " +
                $"choreType.id={chore?.choreType?.Id ?? "null"} " +
                $"isRunning={brain.IsRunning()} " +
                $"physicalCell={physCell} nav.cachedCell={navCell} cellMatch={physCell == navCell} " +
                $"idleCell={idleCell} idleIsPhys={idleCell == physCell} " +
                $"nav.IsMoving={nav?.IsMoving()} " +
                $"choreDriverSmi={(driver?.GetSMI() != null ? "ok" : "null")}");
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
                brain.UpdateBrain();
                var cell = Grid.PosToCell(go);
                Debug.LogWarning($"[Animals] {brain.name}: cell={cell} no ChoreDriver — UpdateBrain() called");
                updated++;
            }
        }

        Debug.LogWarning($"[Animals] ForceUpdateCreatureBrains DONE: kicked={updated} skipped={skipped}");
    }

    /// <summary>
    /// Re-indexes any dupe ChoreProvider entries stored under world key -1.
    ///
    /// ROOT CAUSE:
    ///   IdleMonitor enters 'idle' during TriggerLifecycle Phase 3 (OnSpawn).
    ///   ToggleRecurringChore Enter fires → new IdleChore → provider.AddChore(this).
    ///   AddChore calls chore.gameObject.GetMyParentWorldId() for the map key.
    ///   If Grid.WorldIdx[cell] == byte.MaxValue at that moment (not yet written),
    ///   GetMyParentWorldId() returns -1 → chore stored under key -1.
    ///   CollectChores queries with the valid world key (0+) → no match → miss.
    ///
    /// FIX: move all chores from choreWorldMap[-1] to choreWorldMap[correctKey],
    ///   where correctKey = go.GetMyParentWorldId() (returns a valid ID post warm-up).
    ///   Called once at tick=61, right after ForceUpdateBrains. The natural
    ///   BrainScheduler.RenderEveryTick at tick=62+ then finds the re-indexed chores.
    /// </summary>
    private static void ReindexChoreProviders() {
        var reindexed = 0;
        foreach (var brain in Components.Brains.Items) {
            if (brain == null) continue;
            if (!brain.gameObject.HasTag(GameTags.BaseMinion)) continue;
            var go       = brain.gameObject;
            var provider = go.GetComponent<ChoreProvider>();
            if (provider == null) continue;
            if (!provider.choreWorldMap.TryGetValue(-1, out var miskeyed) || miskeyed.Count == 0) continue;
            var correctKey = go.GetMyParentWorldId();
            if (correctKey == -1) {
                Debug.LogWarning($"[WorldID] {go.name}: GetMyParentWorldId still -1, deferring reindex");
                continue;
            }
            if (!provider.choreWorldMap.ContainsKey(correctKey))
                provider.choreWorldMap[correctKey] = new List<Chore>();
            provider.choreWorldMap[correctKey].AddRange(miskeyed);
            provider.choreWorldMap.Remove(-1);
            reindexed++;
            Debug.LogWarning($"[WorldID] {go.name}: reindexed {miskeyed.Count} chore(s) key=-1 → key={correctKey}");
        }
        Debug.LogWarning($"[WorldID] ReindexChoreProviders: {reindexed} provider(s) reindexed");
    }

    private static void ForceUpdateSensors() {
        var updated = 0;
        foreach (var brain in Components.Brains.Items) {
            if (brain == null) continue;
            var sensors = brain.GetComponent<Sensors>();
            if (sensors == null) continue;
            sensors.UpdateSensors();
            updated++;
        }
        Debug.LogWarning($"[DS-005] ForceUpdateSensors at tick={SensorWarmupTick}: updated {updated} brain(s)");
    }

    /// <summary>
    /// One-shot patch: wraps ALL idle.move enter+exit actions so KBatchedAnimController
    /// calls are skipped when animController is null (headless).
    ///
    /// ROOT CAUSE (two NPE sites):
    ///   1. idle.move.ToggleAnims("anim_loco_walk_kanim")
    ///      Enter: state_target.Get&lt;KAnimControllerBase&gt;(smi).AddAnimOverrides(…)  ← NPE first
    ///      Exit:  state_target.Get&lt;KAnimControllerBase&gt;(smi).RemoveAnimOverrides(…) ← NPE second
    ///      ToggleAnims has NO null-check before AddAnimOverrides/RemoveAnimOverrides.
    ///   2. idle.move.Exit("ClearWalk", smi => smi.animController.Play("idle_default"))
    ///      Direct field access with no null check (fires after ToggleAnims Exit).
    ///   All three NPE when KBatchedAnimController = null in headless → SM.error=true →
    ///   GoTo() no-ops → dupe freezes.
    ///
    /// FIX: wrap ALL enter+exit actions — not just ClearWalk — with an animController guard.
    ///   WrapActions replaces every callback with a wrapper that skips the call when
    ///   smi.animController is null. Works for both ToggleAnims lambdas and ClearWalk.
    ///
    /// APPROACH:
    ///   Harmony patching InitializeStates on the generic GameStateMachine type triggers
    ///   a JIT/type-resolution infinite loop at boot (99.6% CPU, server never starts).
    ///   Instead: access the IdleChore.States singleton directly via a live chore's smi.sm,
    ///   then rewrite its enterActions+exitActions lists in-place.
    /// </summary>
    private void PatchIdleChoreExitActions() {
        if (_idleChorePatchApplied) return;

        foreach (var brain in Components.Brains.Items) {
            if (brain == null) continue;

            // Try the currently active chore first (fast path after tick=61 kick).
            var chore = brain.GetComponent<ChoreDriver>()?.GetCurrentChore() as IdleChore;

            // Fallback: search the dupe's own ChoreProvider chore list.
            // IdleMonitor.ToggleRecurringChore adds IdleChore to the provider as soon as
            // IdleMonitor enters its idle state — even before ChoreDriver.BeginChore fires.
            if (chore == null) {
                var cp = brain.GetComponent<ChoreProvider>();
                if (cp != null) {
                    foreach (var kvp in cp.choreWorldMap) {
                        foreach (var c in kvp.Value) {
                            if (c is IdleChore ic) { chore = ic; break; }
                        }
                        if (chore != null) break;
                    }
                }
            }

            if (chore?.smi == null) continue;

            // smi.sm is the IdleChore.States singleton (created on first IdleChore instantiation).
            PatchMoveActions(chore.smi.sm);
            _idleChorePatchApplied = true;
            return;
        }
    }

    /// <summary>
    /// Patches idle.move enterActions AND exitActions in-place: wraps every callback with
    /// an animController null-guard so KBatchedAnimController calls are skipped in headless.
    ///
    /// StateMachine.Action is a struct { public string name; public object callback; }.
    /// enterActions/exitActions are public List&lt;StateMachine.Action&gt; on StateMachine.BaseState.
    /// All callbacks have signature delegate void(IdleChore.StatesInstance smi).
    /// Delegate.CreateDelegate preserves each callback's original runtime delegate type so
    /// the SM framework's internal (State.Callback)action.callback cast still succeeds.
    /// </summary>
    private static void PatchMoveActions(IdleChore.States states) {
        WrapActions(states.idle.move.enterActions, "enter");
        WrapActions(states.idle.move.exitActions,  "exit");
    }

    /// <summary>
    /// Replaces every callback in the given action list with a wrapper that skips
    /// animation-dependent actions in headless (assets never loaded in DedicatedServer).
    ///
    /// Root cause: Assets.GetAnim() returns null in headless → ToggleAnims calls
    /// AddAnimOverrides(null) → KAnimControllerBase.AddAnimOverrides logs the error
    /// but continues → kanim_file.GetData() NPE (line 825) → SM.error=true → dupe freezes.
    ///
    /// Fix: skip actions identified by name as animation-asset-dependent.
    /// All other actions (MoveTo, UpdateNavType, Trigger(BeginWalk/EndWalk), Transition)
    /// execute normally — they have no dependency on loaded animation assets.
    /// </summary>
    private static void WrapActions(List<StateMachine.Action> actions, string listName) {
        if (actions == null) return;

        for (var i = 0; i < actions.Count; i++) {
            var originalDelegate = actions[i].callback as Delegate;
            if (originalDelegate == null) continue;

            // Convert original to Action<StatesInstance> to invoke without DynamicInvoke.
            // All SM callbacks have the same single-parameter signature.
            var captured = (Action<IdleChore.StatesInstance>) Delegate.CreateDelegate(
                typeof(Action<IdleChore.StatesInstance>),
                originalDelegate.Target,
                originalDelegate.Method);

            // Capture name for closure (loop variable captured by ref would alias later iterations).
            var actionName = actions[i].name;

            // Skip animation-asset actions; run all others unchanged.
            Action<IdleChore.StatesInstance> wrapper = smi => {
                if (IsAnimAction(actionName)) return;
                captured(smi);
            };

            // Restore original runtime delegate type so (State.Callback)action.callback cast works.
            var patched = Delegate.CreateDelegate(
                originalDelegate.GetType(),
                wrapper.Target,
                wrapper.Method);

            // StateMachine.Action is a struct — must replace by index.
            actions[i] = new StateMachine.Action(actions[i].name, patched);
        }
    }

    /// <summary>
    /// Returns true for action names that call into KAnimControllerBase with a KAnimFile
    /// loaded from Assets. In DedicatedServer, Assets.GetAnim() always returns null, so
    /// these actions would NPE inside AddAnimOverrides/RemoveAnimOverrides.
    ///
    /// Actions NOT listed here (MoveTo, UpdateNavType, Trigger(BeginWalk/EndWalk),
    /// Transition) have no asset dependency and execute normally.
    /// </summary>
    private static bool IsAnimAction(string name) =>
        name != null && (
            name.Contains("ToggleAnims") ||   // AddAnimOverrides / RemoveAnimOverrides
            name == "ClearWalk"               // smi.animController.Play("idle_default")
        );

    /// <summary>
    /// Runtime diagnostic at tick=200: logs per-dupe brain/IdleMonitor/providers state,
    /// then logs SM.error for every state machine on each dupe's StateMachineController.
    /// Fires once — well past tick=0 transients but early enough to catch runtime failures.
    /// </summary>
    private static void RuntimeDiagAt200() {
        Console.WriteLine("[RT_DIAG] tick=200 — runtime state snapshot");
        foreach (var go in Components.LiveMinionIdentities.Items.Select(m => m.gameObject)) {
            var smc      = go.GetComponent<StateMachineController>();
            var consumer = go.GetComponent<ChoreConsumer>();
            var idleMon  = smc?.GetSMI<IdleMonitor.Instance>();
            var brain    = go.GetComponent<MinionBrain>();
            int provCount     = consumer?.providers?.Count ?? -1;
            int choreMapCount = consumer?.choreProvider?.choreWorldMap?.Count ?? -1;
            Console.WriteLine($"[RT_DIAG] go={go.GetInstanceID()} brain.isRunning={brain?.IsRunning()} " +
                $"idleMon={idleMon?.GetHashCode().ToString() ?? "NULL"} " +
                $"idleMon.isCrashed={idleMon?.isCrashed} " +
                $"globalSMError={StateMachine.Instance.error} " +
                $"idleMon.currentState={idleMon?.GetCurrentState()?.name ?? "NULL"} " +
                $"providers={provCount} choreMapCount={choreMapCount}");
        }

        foreach (var go in Components.LiveMinionIdentities.Items.Select(m => m.gameObject)) {
            var smc = go.GetComponent<StateMachineController>();
            if (smc?.stateMachines == null) continue;
            for (var i = 0; i < smc.stateMachines.Count; i++) {
                var sm = smc.stateMachines[i];
                if (sm != null && sm.isCrashed)
                    Console.WriteLine($"[SM_ERROR] go={go.GetInstanceID()} sm[{i}]={sm.GetType().Name} isCrashed=True");
            }
        }
    }
}
