using System;
using System.Reflection;
using Klei.AI;
using UnityEngine;

namespace DedicatedServer.Game;

/// <summary>
/// Per-creature component bootstrap for the dedicated server headless environment.
///
/// Same problem as dupes: TriggerLifecycle fires Brain.OnSpawn() but may partially fail,
/// leaving brain.running=false or Navigator SM unstarted → CreatureBrainGroup skips the brain.
///
/// Key differences from MinionPrefab (dupe fix):
///   - No Schedule/Schedulable — creatures have no work schedule.
///   - No RationalAi — creature chores come from ChoreTable + state machine defs (already
///     started by KPrefabID.OnSpawn → StartSMIS during TriggerLifecycle).
///   - Sensors not present on most creatures (only Rovers/FetchDrones have them).
///   - Uses CreatureBrainGroup (GameTags.CreatureBrain) not DupeBrainGroup.
///
/// Setup(brain) steps:
///   0. AddOrGet canonical creature components (Pickupable, Clearable, Traits, Health,
///      RangedAttackable, FactionAlignment, Prioritizable, Effects) and SM Defs
///      (CritterEmoteMonitor, CreatureDebugGoToMonitor, DeathMonitor, CreatureThoughtGraph,
///      AnimInterruptMonitor, CritterTemperatureMonitor). StartSMIS() restarts any crashed.
///   1. Ensure brain.running=true — if false, re-register in BrainScheduler via Remove+Add.
///   2. Ensure Navigator SM started — if nav.GetSMI()==null, call nav.smi.StartSM().
///   3. Ensure consumerState != null (ChoreConsumer.OnSpawn may have failed).
///   4. Sensors.Spawn for Rovers/FetchDrones.
///   5. StandardWorker guard + ChoreDriver SM safety net.
///   6. Diagnostic log: cell, brain.running, nav state, chore, consumerState.
/// </summary>
public static class CreaturePrefab {

    // Reflection cache for Brain.running (private field).
    // Used to force brain.running=true when Brain.Spawn() is a no-op (isSpawned already set).
    private static readonly FieldInfo _brainRunningField =
        typeof(Brain).GetField("running", BindingFlags.Instance | BindingFlags.NonPublic)!;

    // Reflection cache for ChoreDriver.StatesInstance.<worker>k__BackingField.
    //
    // ROOT CAUSE: ChoreDriver.StatesInstance.ctor sets
    //   worker = base.master.GetComponent<WorkerBase>()
    // TriggerLifecycle Phase 1.5 injects StandardWorker BEFORE Phase 2 (Spawn→StartSM→ctor)
    // so the ctor should see a non-null WorkerBase. However, for critters whose ChoreDriver is
    // added via [MyCmpAdd] on ChoreConsumer and where Phase 1 partially fails, the Phase 1.5
    // condition `components.Any(c => c is ChoreDriver)` may evaluate false → StandardWorker
    // not injected → ctor sees null → worker=null → haschore.Update b__5_3 NPEs every tick.
    //
    // FIX: CreaturePrefab.Setup() adds StandardWorker unconditionally (step 0), then
    // immediately checks the existing StatesInstance.worker and sets the backing field if null.
    // This is a belt-and-suspenders safety net on top of the Phase 1.5 injection.
    private static readonly FieldInfo _choreDriverWorkerField =
        typeof(ChoreDriver.StatesInstance).GetField(
            "<worker>k__BackingField",
            BindingFlags.Instance | BindingFlags.NonPublic);

    public static void Setup(CreatureBrain brain) {
        var go = brain.gameObject;
        if (go == null) return;

        // ── DS-006b: StandardWorker + worker-field retrofit ──────────────────────
        // ChoreDriver.StatesInstance.ctor: worker = GetComponent<WorkerBase>().
        // TriggerLifecycle Phase 1.5 injects StandardWorker before Phase 2 (Spawn→StartSM→ctor)
        // so the ctor should see a non-null WorkerBase.  However, for critters whose ChoreDriver
        // is added via [MyCmpAdd] on ChoreConsumer and where Phase 1 partially fails, the
        // Phase 1.5 check may miss the GO → StandardWorker not injected → ctor sees null
        // → worker=null → b__5_3 NPEs on smi.worker.GetWorkable() every tick.
        // Belt-and-suspenders: re-add StandardWorker (idempotent), then retrofit the existing
        // StatesInstance.worker backing field if it is still null.
        go.AddOrGet<StandardWorker>();
        var existingChoreDriverSmi = go.GetSMI<ChoreDriver.StatesInstance>();
        if (existingChoreDriverSmi != null && existingChoreDriverSmi.worker == null) {
            var workerBase = go.GetComponent<WorkerBase>();
            if (workerBase != null) {
                _choreDriverWorkerField?.SetValue(existingChoreDriverSmi, workerBase);
                Console.WriteLine($"[Animals] {go.name}: retrofitted ChoreDriver.worker (was null → {workerBase.GetType().Name})");
            }
        }

        // ── Remove render-only components that NPE during headless OnSpawn ──────
        // ExtendEntityToBasicCreature() adds both CharacterOverlay and AnimEventHandler to
        // every critter prefab. Both crash in headless:
        //   CharacterOverlay.OnSpawn → NameDisplayScreen.AddNewEntry → nameDisplayCanvas NPE
        //   AnimEventHandler.OnSpawn[IL_0x4c] → animCollider (KBoxCollider2D) NPE
        // Mirrors the same fix applied to MinionPrefab.Setup().
        var charOverlay = go.GetComponent<CharacterOverlay>();
        if (charOverlay != null) UnityEngine.Object.DestroyImmediate(charOverlay);
        var animEventHandler = go.GetComponent<AnimEventHandler>();
        if (animEventHandler != null) UnityEngine.Object.DestroyImmediate(animEventHandler);

        // ── Canonical creature components (from ExtendEntityToBasicCreature) ──────
        // AddOrGet is idempotent: returns existing component or adds a new one.
        // Ensures components are present even if the prefab was partially registered
        // in headless or the save predates the component being added.
        // Critical field values match EntityTemplates.ExtendEntityToBasicCreature().
        go.AddOrGet<Pickupable>();
        go.AddOrGet<Clearable>().isClearable = false;   // confirmed 890x NPE in server log
        go.AddOrGet<Traits>();
        go.AddOrGet<Health>().isCritter = true;
        go.AddOrGet<RangedAttackable>();
        go.AddOrGet<FactionAlignment>();
        go.AddOrGet<Prioritizable>();                   // confirmed 858x NPE in server log
        go.AddOrGet<Effects>();
        // Facing is added via [MyCmpAdd] on Navigator.InitializeComponent().
        // If Navigator.InitializeComponent() failed in headless, Facing is absent →
        // FallMonitor.GetBackCell(), FixedCaptureStates, etc. NPE on GetComponent<Facing>().
        go.AddOrGet<Facing>();

        // SM Defs: ensure all creature behaviour defs are registered on the SMC.
        // AddOrGetDef is idempotent — no-op if def already present.
        // NOTE: StartSMIS() is intentionally NOT called here.
        //   TriggerLifecycle (KPrefabID.OnSpawn) already called StartSMIS() for every creature
        //   during world load.  Re-calling it on a headless-loaded creature is unsafe:
        //   StateMachineController.StartSMIS() iterates smc.stateMachines (private list) and
        //   calls GetSMI(type) for each registered def.  GetSMI iterates the same list and calls
        //   instance.GetType() — which NPEs when the list contains a null entry left by a
        //   partial TriggerLifecycle failure in the headless environment.
        //   Root cause: option (B) — StartSMIS is redundant and dangerous here.
        go.AddOrGetDef<CritterEmoteMonitor.Def>();
        go.AddOrGetDef<CreatureDebugGoToMonitor.Def>();
        go.AddOrGetDef<DeathMonitor.Def>();
        go.AddOrGetDef<CreatureThoughtGraph.Def>();
        go.AddOrGetDef<AnimInterruptMonitor.Def>();
        go.AddOrGetDef<CritterTemperatureMonitor.Def>();

        // ── Step 1: ensure brain.running = true ──────────────────────────────────
        // Brain.Spawn() is a no-op when isSpawned=true (already set by TriggerLifecycle).
        // If OnSpawn crashed after "running=true" was set, brain is registered but running.
        // If Components.Brains.Add() failed (BrainGroup.HasTag mismatch), brain is not in
        // CreatureBrainGroup → UpdateBrain() never called → creature stuck.
        // Fix: remove from Components.Brains, force running=true via reflection, re-add
        // to retrigger BrainScheduler.OnAddBrain → HasTag(CreatureBrain) → AddBrain().
        if (!brain.IsRunning()) {
            try { Components.Brains.Remove(brain); } catch { /* ignore if not registered */ }
            _brainRunningField?.SetValue(brain, true);
            Components.Brains.Add(brain);
            Console.WriteLine($"[Animals] {go.name}: brain was NOT running — re-registered");
        }

        // ── Step 2: ensure Navigator SM is running ────────────────────────────────
        // Navigator.OnSpawn() starts the SM (normal.stopped state). In headless it may
        // throw (TargetLocator KInstantiate → NullRef) leaving _smi=null → no movement.
        var nav = go.GetComponent<Navigator>();
        if (nav != null) {
            if (!nav.IsInitialized()) {
                try { nav.InitializeComponent(); }
                catch (Exception ex) {
                    Console.WriteLine($"[Animals] {go.name}: Navigator.InitializeComponent partial: {ex.GetBaseException().Message}");
                }
            }
            if (nav.GetSMI() == null) {
                try {
                    nav.smi.StartSM();
                    Console.WriteLine($"[Animals] {go.name}: Navigator.StartSM() called");
                } catch (Exception ex) {
                    Console.WriteLine($"[Animals] {go.name}: Navigator.StartSM partial: {ex.GetBaseException().Message}");
                }
            }
        }

        // ── Step 3: ensure ChoreConsumer.consumerState != null ────────────────────
        var cc = go.GetComponent<ChoreConsumer>();
        if (cc != null && cc.consumerState == null) {
            try {
                cc.consumerState = new ChoreConsumerState(cc);
            } catch (Exception ex) {
                Console.WriteLine($"[Animals] {go.name}: consumerState init failed: {ex.GetBaseException().Message}");
            }
        }

        // ── Step 3.5: ensure creature's own ChoreProvider is in consumer.providers ──
        // ChoreConsumer.providers (private List<ChoreProvider>) is the list FindNextChore()
        // iterates to collect available chores. For dupes, MinionModifiers.OnSpawn() calls
        // AddProvider(GlobalChoreProvider.Instance) — regular creatures NEVER get AddProvider()
        // called → providers stays empty → FindNextChore() always returns false → no chore
        // assigned → ForceUpdateCreatureBrains() at tick=62 can't find a chore → nochore forever.
        //
        // The creature's own ChoreProvider holds the SM chores created by choreTableInstance
        // (IdleStates, MoveToSafetyChore, etc.) — it just needs to be registered in providers.
        var choreProvider = go.GetComponent<ChoreProvider>();
        if (cc != null && choreProvider != null) {
            cc.AddProvider(choreProvider);
        }

        // ── Step 4: Sensors (Rovers/FetchDrones only — standard critters lack them) ──
        var sensors = go.GetComponent<Sensors>();
        if (sensors != null && !sensors.isSpawned) {
            try { sensors.Spawn(); }
            catch (Exception ex) {
                Console.WriteLine($"[Animals] {go.name}: sensors.Spawn partial: {ex.GetBaseException().Message}");
            }
        }

        // ── Step 5: ChoreDriver SM + DS-006b worker retrofit ─────────────────────
        // StandardWorker is already added unconditionally at the top of Setup().
        // If ChoreDriver SM was never started (shouldn't happen after FixChoreConsumers, but guard).
        var choreDriver = go.GetComponent<ChoreDriver>();
        if (choreDriver != null && choreDriver.GetSMI() == null) {
            choreDriver.smi.StartSM();
            Console.WriteLine($"[Animals] {go.name}: ChoreDriver SM started (safety net)");
        }
        // DS-006b: StandardWorker is guaranteed by TriggerLifecycle Phase 1.5 injection
        // (UnityRuntime.TriggerLifecycle adds it before Phase 2 → OnSpawn → StartSM → ctor).
        // No post-hoc retrofit needed here.

        // Declare smc here so both Step 6 and Step 7 can use it.
        var smc = go.GetComponent<StateMachineController>();

        // ── Step 6: fix broken/missing creature monitor SMs ──────────────────
        // ROOT CAUSE (two-stage crash during TriggerLifecycle):
        //
        // Stage A — CreateSMIS() (KPrefabID.OnPrefabInit):
        //   Activator.CreateInstance(CreatureThoughtGraph.Instance, master, def) fires the ctor
        //   body AFTER base(master,def) which already added the partial instance to
        //   smc.stateMachines.  The ctor immediately NPEs on
        //   NameDisplayScreen.Instance.RegisterComponent() (null in headless) → CreateSMIS
        //   throws, exits the foreach loop early → monitor defs AFTER CreatureThoughtGraph
        //   (AnimInterruptMonitor, CritterTemperatureMonitor, CreatureFallMonitor,
        //   BurrowMonitor, etc.) are NEVER instantiated.
        //
        // Stage B — StartSMIS() (KPrefabID.OnSpawn):
        //   CritterEmoteMonitor.Instance was created successfully in Stage A (constructor
        //   safe; only accesses Db.Get().Emotes.Critter), so StartSMIS finds it and calls
        //   StartSM() → GoTo(cooldown) → cooldown.Enter callback:
        //     NameDisplayScreen.Instance.SetThoughtBubbleDisplay(...)  ← NPE (null in headless)
        //   Exception caught by GoTo's try/catch → Error() → StateMachine.Instance.error=True
        //   → ALL subsequent GoTo() calls on any SM return immediately → every following
        //   StartSM() in the loop is a no-op → DeathMonitor, Navigator, etc. stuck at null.
        //
        // FIX (performed after WorldBuilder resets Instance.error at line 328):
        //   6a. Remove null entries left by partial CreateSMIS failures.
        //   6b. Remove headless-unsafe instances (CritterEmoteMonitor crashes StartSM;
        //       CreatureThoughtGraph has a corrupt partial instance from Stage A).
        //   6c. For each monitor def in cmpdef.defs (skip unsafe), create the SMI if
        //       missing (those never instantiated after the Stage A crash).
        //   6d. Reset Instance.error (FixRationalAi may have re-set it for dupes),
        //       then start all non-running monitor SMIs with per-SM try-catch.
        if (smc != null && smc.cmpdef?.defs != null) {
            // 6a: purge null entries (left by partial constructor failures)
            smc.stateMachines.RemoveAll(s => s == null);

            // 6b: remove headless-unsafe instances
            var emoteSmi = smc.GetSMI<CritterEmoteMonitor.Instance>();
            if (emoteSmi != null) {
                smc.stateMachines.Remove(emoteSmi);
                Console.WriteLine($"[Animals] {go.name}: 6b removed CritterEmoteMonitor (cooldown.Enter → NameDisplayScreen NPE)");
            }
            var thoughtSmi = smc.GetSMI<CreatureThoughtGraph.Instance>();
            if (thoughtSmi != null) {
                smc.stateMachines.Remove(thoughtSmi);
                Console.WriteLine($"[Animals] {go.name}: 6b removed CreatureThoughtGraph (ctor → NameDisplayScreen NPE)");
            }

            // 6c+6d: for each monitor def, ensure SMI exists then start it
            StateMachine.Instance.error = false;  // must reset BEFORE any StartSM call
            foreach (var def in smc.cmpdef.defs) {
                if (def is CritterEmoteMonitor.Def || def is CreatureThoughtGraph.Def)
                    continue;  // headless-unsafe — skip entirely
                StateMachine.Instance existingSmi = null;
                try {
                    var smType   = def.GetStateMachineType();
                    var smiType  = Singleton<StateMachineManager>.Instance
                                       .CreateStateMachine(smType)
                                       .GetStateMachineInstanceType();
                    existingSmi  = smc.GetSMI(smiType);
                    if (existingSmi == null) {
                        existingSmi = def.CreateSMI(smc);  // base ctor adds to smc.stateMachines
                        Console.WriteLine($"[Animals] {go.name}: 6c created {smType.Name}");
                    }
                } catch (Exception ex) {
                    Console.WriteLine($"[Animals] {go.name}: 6c CreateSMI {def.GetType().DeclaringType?.Name ?? def.GetType().Name} FAILED: {ex.GetBaseException().Message}");
                    continue;
                }
                if (existingSmi != null && !existingSmi.IsRunning()) {
                    try {
                        existingSmi.StartSM();
                        Console.WriteLine($"[Animals] {go.name}: 6d started {existingSmi.GetType().Name}");
                    } catch (Exception ex) {
                        smc.stateMachines.Remove(existingSmi);
                        Console.WriteLine($"[Animals] {go.name}: 6d StartSM {existingSmi.GetType().Name} FAILED+removed: {ex.GetBaseException().Message}");
                    }
                }
            }
        }

        // ── Step 7: ensure CreatureFallMonitor SM is running ─────────────────────
        // CreatureFallMonitor.grounded evaluates ShouldFall() each tick and toggles the
        // GameTags.Creatures.Falling behaviour → creates FallStates chore → ToggleGravity().
        // If TriggerLifecycle's StartSMIS() crashed before reaching CreatureFallMonitor.Def
        // (e.g. NPE in another SM def), the instance is never created → creature floats.
        // CreatureFallMonitor.Def is per-creature (added in individual config files, e.g.
        // BaseHatchConfig, BaseDreckoConfig, BasePacuConfig) — not in ExtendEntityToBasicCreature.
        // Fix: check GetSMI; if null and def is present, manually start the instance.
        // (smc declared above, before Step 6)
        if (smc != null) {
            var fallSmi = go.GetSMI<CreatureFallMonitor.Instance>();
            if (fallSmi == null) {
                var fallDef = smc.GetDef<CreatureFallMonitor.Def>();
                if (fallDef != null) {
                    try {
                        new CreatureFallMonitor.Instance(smc, fallDef).StartSM();
                        Console.WriteLine($"[Animals] {go.name}: CreatureFallMonitor.Instance started");
                    } catch (Exception ex) {
                        Console.WriteLine($"[Animals] {go.name}: CreatureFallMonitor start failed: {ex.GetBaseException().Message}");
                    }
                }
            }
        }

        // ── Step 8: Diagnostic: one line per creature ────────────────────────────
        var cell  = Grid.PosToCell(go);
        var chore = choreDriver?.GetCurrentChore();
        Console.WriteLine(
            $"[Animals] {go.name}: cell={cell} running={brain.IsRunning()} " +
            $"nav={(nav?.GetSMI() != null ? "OK" : "null")} " +
            $"chore={chore?.GetType().Name ?? "null"} " +
            $"consumerState={cc?.consumerState != null} " +
            $"worker={choreDriver?.GetSMI<ChoreDriver.StatesInstance>()?.worker != null}"
        );
    }
}
