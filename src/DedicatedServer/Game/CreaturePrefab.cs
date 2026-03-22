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

    public static void Setup(CreatureBrain brain) {
        var go = brain.gameObject;
        if (go == null) return;

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

        // ── Step 4: Sensors (Rovers/FetchDrones only — standard critters lack them) ──
        var sensors = go.GetComponent<Sensors>();
        if (sensors != null && !sensors.isSpawned) {
            try { sensors.Spawn(); }
            catch (Exception ex) {
                Console.WriteLine($"[Animals] {go.name}: sensors.Spawn partial: {ex.GetBaseException().Message}");
            }
        }

        // ── Step 5: ensure StandardWorker (WorkerBase) is present — DS-006 ─────────
        // ChoreDriver.StatesInstance.ctor sets worker = GetComponent<WorkerBase>().
        // Primary fix is in FixChoreConsumers: AddOrGet<StandardWorker>() runs before
        // driver.Spawn() → ctor finds WorkerBase → worker set correctly.
        // This AddOrGet is a safety net for any SM started outside FixChoreConsumers.
        go.AddOrGet<StandardWorker>();
        var choreDriver = go.GetComponent<ChoreDriver>();
        if (choreDriver != null && choreDriver.GetSMI() == null) {
            // SM was never started (shouldn't happen after FixChoreConsumers, but guard it).
            choreDriver.smi.StartSM();
            Console.WriteLine($"[Animals] {go.name}: ChoreDriver SM started (safety net)");
        }

        // ── Diagnostic: one line per creature ────────────────────────────────────
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
