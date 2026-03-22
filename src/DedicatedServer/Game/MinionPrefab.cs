using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace DedicatedServer.Game;

/// <summary>
/// Per-duplicant component bootstrap for the dedicated server headless environment.
///
/// The real game runs MinionConfig.OnSpawn() → BaseMinionConfig.BaseOnSpawn() from within
/// Unity's normal lifecycle (Awake/Start/OnEnable fire in order). In headless the lifecycle
/// is partial (no display, no audio, assets incomplete) so we replicate the critical path:
///
/// Setup(go) pipeline:
///   1. Remove render-only components (CharacterOverlay, AnimEventHandler)
///   2. Ensure required functional components exist
///   3. EnsureAssignableProxy — proxy GO required for identity/slot management
///   4. Setup sensors — 6 of 8 (excluding AssignableReachabilitySensor which NPEs in headless)
///   5. Isolate smc.stateMachines — CRITICAL: on save-load path all dupe SMCs share one list
///      via MemberwiseClone; assign a fresh List so each dupe gets its own SM registry
///   6. ResetSharedReferences — isolate ChoreConsumer/ChoreProvider fields (also MemberwiseClone)
///   7. Start all 52 SMs via RationalAi.Instance (mirrors BaseMinionConfig.BaseOnSpawn minus ARS)
///   8. AddProvider — own ChoreProvider in consumer.providers so FindNextChore finds IdleChore
///   9. Navigator 7 transition layers
///  10. Navigator SM init + start
///  11. StandardWorker + ChoreDriver SM
///  12. Brain.Spawn + BrainScheduler re-registration
///  13. Pre-add GameTags.Idle + Sensors.Spawn
///  14. IdleCellSensor cell seeding
/// </summary>
public static class MinionPrefab {

    // Reflection cache for IdleCellSensor.cell (private int).
    // Used to seed the initial idle cell = dupe's own spawn cell so adjacent dupes
    // don't share the same BFS result cell (causes IsPreemptable to fail for one of them).
    private static readonly FieldInfo _idleSensorCellField =
        typeof(IdleCellSensor).GetField("cell", BindingFlags.Instance | BindingFlags.NonPublic)!;

    public static void Setup(GameObject go) {
        var smc = go.GetComponent<StateMachineController>();
        if (smc == null) {
            Console.WriteLine($"[FixRationalAi] {go.name}: StateMachineController=null, skipping");
            return;
        }

        // ── Remove render-only components that NPE during headless OnSpawn ──────
        // These components are added by BaseMinionConfig.BaseMinion() for visual/audio use
        // but crash during OnSpawn in headless because their dependencies are absent:
        //   CharacterOverlay.OnSpawn → NameDisplayScreen.Instance.AddNewEntry
        //     → Util.KInstantiateUI(original, nameDisplayCanvas.gameObject) where
        //       nameDisplayCanvas is a Unity-serialized Canvas, always null in headless.
        //   AnimEventHandler.OnSpawn[IL_0x4c] → animCollider.offset where animCollider is
        //     [MyCmpGet] KBoxCollider2D — null because KBoxCollider2D is render-only (absent).
        // DestroyImmediate removes the component before any Spawn() can fire.
        // AddOrGet on a render-only component is fine after this — it is a no-op if we
        // previously removed the same component type, but we never add CharacterOverlay or
        // AnimEventHandler (both are in the render-only skip list).
        var charOverlay = go.GetComponent<CharacterOverlay>();
        if (charOverlay != null) UnityEngine.Object.DestroyImmediate(charOverlay);
        var animEventHandler = go.GetComponent<AnimEventHandler>();
        if (animEventHandler != null) UnityEngine.Object.DestroyImmediate(animEventHandler);

        // ── Ensure all BaseMinionConfig.BaseMinion() functional components are present ──
        // AddOrGet is a no-op when the component already exists (e.g. loaded from save).
        // Components must be present before BaseOnSpawn() runs so all subscriptions and
        // SM starts succeed. Order matches BaseMinionConfig.BaseMinion() lines 315-354;
        // render-only components (KBatchedAnimController, KBoxCollider2D, SnapOn,
        // AnimEventHandler, GridVisibility, CharacterOverlay, DecorProvider) are skipped.
        go.AddOrGet<KSelectable>();                                          // selectableEntityTemplate
        go.AddOrGet<ChoreProvider>();                                        // line 315
        go.AddOrGetDef<DebugGoToMonitor.Def>();                             // line 316
        go.AddOrGet<Schedulable>();                                          // line 322
        go.AddOrGet<FactionAlignment>().Alignment = FactionManager.FactionID.Duplicant; // line 331
        go.AddOrGet<Weapon>();                                               // line 332
        go.AddOrGet<RangedAttackable>();                                     // line 333
        var occupyArea = go.AddOrGet<OccupyArea>();                         // line 335
        occupyArea.objectLayers = new ObjectLayer[1];
        occupyArea.ApplyToCells = false;
        occupyArea.SetCellOffsets(new CellOffset[] {
            new CellOffset(0, 0),
            new CellOffset(0, 1)
        });
        go.AddOrGet<Pickupable>();                                           // line 343
        go.AddOrGet<CreatureSimTemperatureTransfer>();                       // line 344
        go.AddOrGet<SicknessTrigger>();                                      // line 348
        go.AddOrGet<ClothingWearer>();                                       // line 349
        go.AddOrGet<SuitEquipper>();                                         // line 350
        go.AddOrGet<ConsumableConsumer>();                                   // line 352
        go.AddOrGet<MinionResume>();                                         // line 354

        // Ensure assignable proxy exists — needed for RationalAi.alive → slot management.
        var identity = go.GetComponent<MinionIdentity>();
        if (identity != null)
            EnsureAssignableProxy(go, identity);

        // ── Step 4: Sensors (excluding AssignableReachabilitySensor) ─────────
        // BaseOnSpawn adds 8 sensors including ARS; ARS.ctor calls
        // identity.assignableProxy.Get() which NPEs in headless (proxy is incomplete).
        // We add the 6 safe ones directly; ARS and BalloonStandCellSensor are excluded.
        var sensorsComp = go.GetComponent<Sensors>();
        if (sensorsComp != null) {
            sensorsComp.Add(new PathProberSensor(sensorsComp));
            sensorsComp.Add(new SafeCellSensor(sensorsComp));
            sensorsComp.Add(new IdleCellSensor(sensorsComp));
            sensorsComp.Add(new PickupableSensor(sensorsComp));
            sensorsComp.Add(new ClosestEdibleSensor(sensorsComp));
            sensorsComp.Add(new MingleCellSensor(sensorsComp));
        }

        // ── Step 5: Isolate smc.stateMachines ────────────────────────────────
        // SMOKING GUN (confirmed 14b8aa5): smc hashes are distinct (3 separate objects)
        // but getSMI returns hash 568807501 for ALL 3 dupes — the SAME IdleMonitor.
        //
        // Root cause: on the save-load path Unity uses MemberwiseClone internally
        // (CloneSingle is bypassed). MemberwiseClone is a shallow copy. The private
        // field `List<StateMachine.Instance> stateMachines` is the SAME list object
        // in all 3 SMCs. When dupe1 creates IdleMonitor.Instance(smc1), its ctor calls
        // smc1.AddStateMachineInstance(this) → appended to the shared list. GetSMI
        // iterates from index 0 → always returns dupe0's IdleMonitor for everyone.
        //
        // Fix: assign a new List to THIS dupe's SMC field. Since smc0/smc1/smc2 are
        // distinct objects, `smc1.stateMachines = new List<>()` only changes smc1's
        // field; smc0 and smc2 retain their reference to the old (or own) list.
        smc.stateMachines = new List<StateMachine.Instance>();

        // ── Step 6: Reset shared ChoreConsumer/ChoreProvider references ──────
        // Same MemberwiseClone issue: providers, choreProvider, choreWorldMap etc.
        // are shared. Reset all to fresh instances before RationalAi creates SMs
        // (IdleMonitor → IdleChore goes to cp.choreWorldMap; must be fresh).
        ResetSharedReferences(go);

        // ── Step 7: Start all 52 SMs via RationalAi.Instance ─────────────────
        // Mirrors BaseMinionConfig.BaseOnSpawn minus the sensor setup (done above).
        // RationalAi.alive.ToggleStateMachineList(GetStateMachinesToRunWhenAlive)
        // calls each factory function → creates and starts every sub-SM (IdleMonitor,
        // DeathMonitor, BreathMonitor, etc.) into THIS dupe's fresh stateMachines list.
        var allSmFactories = BaseMinionConfig.BaseRationalAiStateMachines()
            .Concat(new Func<RationalAi.Instance, StateMachine.Instance>[] {
                // Additional 9 SMs from MinionConfig.RATIONAL_AI_STATE_MACHINES
                (smi) => new BreathMonitor.Instance(smi.master),
                (smi) => new SteppedInMonitor.Instance(smi.master),
                (smi) => new Dreamer.Instance(smi.master),
                (smi) => new StaminaMonitor.Instance(smi.master),
                (smi) => new RationMonitor.Instance(smi.master),
                (smi) => new CalorieMonitor.Instance(smi.master),
                (smi) => new BladderMonitor.Instance(smi.master),
                (smi) => new HygieneMonitor.Instance(smi.master),
                (smi) => new TiredMonitor.Instance(smi.master)
            }).ToArray();
        var rationalAiSmi = new RationalAi.Instance(smc, new Tag("Minion"));
        rationalAiSmi.stateMachinesToRunWhenAlive = allSmFactories;
        rationalAiSmi.StartSM(); // → root.Enter → alive.Enter → ToggleStateMachineList → 52 SMs start

        // ── Step 8: Own ChoreProvider in providers ───────────────────────────
        // RationalAi.Instance ctor (via AddUrge) may touch consumer.urges (already fresh).
        // AddProvider wires cp into consumer.providers so FindNextChore iterates it.
        var consumerPost = go.GetComponent<ChoreConsumer>();
        if (consumerPost != null)
            consumerPost.AddProvider(go.GetComponent<ChoreProvider>());

        // ── Step 9: Navigator transition layers (all 7 from BaseOnSpawn) ─────
        var nav = go.GetComponent<Navigator>();
        if (nav?.transitionDriver != null) {
            nav.transitionDriver.overrideLayers.Add(new BipedTransitionLayer(nav, 3.325f, 2.5f));
            nav.transitionDriver.overrideLayers.Add(new DoorTransitionLayer(nav));
            nav.transitionDriver.overrideLayers.Add(new TubeTransitionLayer(nav));
            nav.transitionDriver.overrideLayers.Add(new LadderDiseaseTransitionLayer(nav));
            nav.transitionDriver.overrideLayers.Add(new ReactableTransitionLayer(nav));
            nav.transitionDriver.overrideLayers.Add(new NavTeleportTransitionLayer(nav));
            nav.transitionDriver.overrideLayers.Add(new SplashTransitionLayer(nav));
        }

        // ── Step 10: Navigator SM init + start ───────────────────────────────
        // BaseOnSpawn does NOT call nav.smi.StartSM(). Must start explicitly so
        // normal.moving fires and Navigator.Advance() works.
        var nav3 = go.GetComponent<Navigator>();
        if (nav3 != null && !nav3.IsInitialized())
            nav3.InitializeComponent();
        if (nav3 != null && nav3.GetSMI() == null)
            nav3.smi.StartSM(); // lazy-creates instance, transitions to normal.stopped

        // DS-006 fix (complete): StandardWorker must exist BEFORE StatesInstance.ctor runs.
        // StatesInstance.ctor sets: worker = GetComponent<WorkerBase>().
        // If StandardWorker is absent at ctor time (headless prefab bootstrap may fail),
        // worker=null → haschore.Update b__5_3 [0x0075]: smi.worker.GetWorkable() → NPE
        // every SIM_EVERY_TICK, propagating to Program.Main catch handler + aborting the subtick.
        //
        // Original fix was guarded: only added StandardWorker when GetSMI()==null (SM not
        // yet started). Bug: for dupes whose TriggerLifecycle ran OnSpawn() successfully,
        // GetSMI()!=null → guard skipped → StandardWorker never added → worker=null baked in.
        //
        // Full fix:
        //   1. AddOrGet<StandardWorker>() unconditionally (no-op if already present).
        //   2. Start SM only when not already started (avoids double-start).
        //   3. If SM is already running but worker==null (ctor ran before StandardWorker was
        //      added), patch the live SMI directly — worker is a public field.
        go.AddOrGet<StandardWorker>();
        var choreDriver = go.GetComponent<ChoreDriver>();
        if (choreDriver != null) {
            if (choreDriver.GetSMI() == null) {
                choreDriver.smi.StartSM(); // lazy-creates StatesInstance, enters nochore
                Debug.LogWarning($"[FixRationalAi] {go.name}: ChoreDriver SM started");
            }
            // DS-006b: StandardWorker is guaranteed by TriggerLifecycle Phase 1.5 injection
            // (UnityRuntime.TriggerLifecycle adds it before Phase 2 → OnSpawn → StartSM → ctor).
            // No post-hoc retrofit needed here.
        }

        // Spawn Brain — sets running=true + choreConsumer + registers with BrainScheduler.
        // Without this: Brain.IsRunning()=false → BrainGroup.RenderEveryTick skips brain
        // → UpdateBrain() never called → chore never picked even though IdleChore exists.
        var brain = go.GetComponent<MinionBrain>();
        if (brain != null && !brain.isSpawned) {
            brain.Spawn();
            Console.WriteLine($"[FixRationalAi] {go.name}: Brain spawned (running={brain.IsRunning()})");
        }

        // Re-register brain with BrainScheduler if IsRunning()==false.
        // Scenario: TriggerLifecycle called Brain.OnSpawn() (isSpawned=true) but
        // BrainScheduler.AddBrain() failed or was skipped → running=false baked in.
        // The isSpawned guard above skips brain.Spawn() for these dupes.
        // Fix: Remove + Add re-triggers BrainScheduler.OnAddBrain → sets running=true.
        // Mirrors the same fix in CreaturePrefab.Setup() for critter brains.
        if (brain != null && !brain.IsRunning()) {
            Components.Brains.Remove(brain);
            Components.Brains.Add(brain);
            Console.WriteLine($"[FixRationalAi] {go.name}: brain re-registered with BrainScheduler, IsRunning={brain.IsRunning()}");
        }

        // Pre-add GameTags.Idle to break the IdleCellSensor deadlock.
        // IdleCellSensor.Update() returns InvalidCell when !prefabid.HasTag(GameTags.Idle).
        // IdleChore adds the tag only AFTER Brain picks it — circular dependency.
        // Pre-adding breaks the cycle; IdleChore will re-add/remove via ToggleTag normally.
        go.GetComponent<KPrefabID>()?.AddTag(GameTags.Idle);

        // Spawn Sensors — subscribes OnBrainPreUpdate to Brain.onPreUpdate.
        // Must happen AFTER Brain.Spawn() so Brain.onPreUpdate delegate is initialised.
        var sensors2 = go.GetComponent<Sensors>();
        if (sensors2 != null && !sensors2.isSpawned) {
            sensors2.Spawn();
        }

        // Seed IdleCellSensor with the dupe's own spawn cell.
        //
        // Root cause: IdleCellSensor.cell defaults to 0 (not Grid.InvalidCell=-1).
        // On the first Brain tick, IdleCellSensor.Update() runs a BFS for the
        // best idle cell.  For two adjacent dupes (cells 50304 and 50305), the BFS
        // may return the SAME cell (e.g. 50304) for BOTH — the one cell with the
        // best SafeFlags score nearby.  Dupe 50304 claims that IdleChore first
        // (sets chore.driver). When dupe 50305's consumer evaluates its IdleChore it
        // also sees driver != null (both share the same chore via the same target
        // cell path), so IsPreemptable (precondition 5) fails → failedPreconditionId=5.
        //
        // Fix: seed each dupe's IdleCellSensor to its OWN current cell so adjacent
        // dupes start with unique idle cells.  The BFS will refine the cell on the
        // first sensor update; the seed is only used until that first update runs.
        var idleSensorToSeed = sensors2?.GetSensor<IdleCellSensor>();
        if (idleSensorToSeed != null && _idleSensorCellField != null) {
            var dupeCell = Grid.PosToCell(go);
            if (Grid.IsValidCell(dupeCell)) {
                _idleSensorCellField.SetValue(idleSensorToSeed, dupeCell);
                Console.WriteLine($"[FixRationalAi] {go.name}: IdleCellSensor seeded to own cell {dupeCell}");
            }
        }

        Console.WriteLine($"[FixRationalAi] {go.name}: OK");
    }

    /// <summary>
    /// Ensures identity.assignableProxy is non-null before BaseOnSpawn.
    /// AssignableReachabilitySensor.ctor calls identity.assignableProxy.Get() → NPE if null.
    ///
    /// Tries ValidateProxy() first (standard game path). Falls back to direct GO creation
    /// replicating MinionAssignablesProxyConfig.CreatePrefab() + InitAssignableProxy().
    /// The GO must be inactive when components are added so Awake/OnPrefabInit fires only
    /// once, after SetTarget is called — matching the game's KInstantiate flow.
    /// </summary>
    /// <summary>
    /// Creates a fresh MinionAssignablesProxy GO and wires it to the identity.
    ///
    /// In headless, Assets.GetPrefab is unavailable, so ValidateProxy() would NPE.
    /// We bypass it entirely and create the proxy directly, replicating
    /// MinionAssignablesProxyConfig.CreatePrefab() + InitAssignableProxy().
    /// The GO must be inactive when components are added so Awake/OnPrefabInit fires
    /// only once, after SetTarget is called — matching the real game's KInstantiate flow.
    /// </summary>
    private static void EnsureAssignableProxy(GameObject go, MinionIdentity identity) {
        if (identity.assignableProxy?.Get() != null) return;

        var proxyGO = new GameObject("MinionAssignablesProxy");
        proxyGO.SetActive(false);          // suppress Awake until fully wired
        proxyGO.AddOrGet<Ownables>();      // proxy.GetComponents<Assignables>() must be non-empty
        proxyGO.AddOrGet<Equipment>();     // ConfigureAssignableSlots needs Equipment for EquipmentSlots
        proxyGO.AddOrGet<KPrefabID>();     // Ref<T>.Set needs KPrefabID.InstanceID to be valid

        var proxy = proxyGO.AddOrGet<MinionAssignablesProxy>();

        if (identity.assignableProxy == null)
            identity.assignableProxy = new Ref<MinionAssignablesProxy>();
        identity.assignableProxy.Set(proxy);
        proxy.SetTarget(identity, go);

        proxyGO.SetActive(true);           // Awake fires → OnPrefabInit → ConfigureAssignableSlots
    }

    /// <summary>
    /// Resets all shared mutable reference fields on ChoreConsumer and ChoreProvider
    /// to fresh per-dupe instances.
    ///
    /// On the save-load path, Unity uses MemberwiseClone internally (CloneSingle is
    /// bypassed). MemberwiseClone is a shallow copy — every reference-type field points
    /// to the SAME object across all dupe clones that share the prefab template:
    ///
    ///   consumer.providers (List)             → shared; providersBefore=4,5,6 in diagnostic
    ///   consumer.choreProvider ([MyCmpAdd])   → shared; points to dupe0's CP for all dupes
    ///   consumer.urges (List)                 → shared; urge-based chore routing broken
    ///   consumer.behaviourPreconditions (Dict)→ shared; RunBehaviourPrecondition misrouted
    ///   consumer.preconditionSnapshot (class) → shared; snapshot state corrupted
    ///   consumer.lastSuccessful... (class)    → shared; same issue
    ///   consumer.choreGroupPriorities (Dict)  → shared; priority overrides leak across dupes
    ///   consumer.choreTypePriorities (Dict)   → shared; type-priority leaks
    ///   consumer.traitDisabledChoreGroups     → shared; trait data crosses dupes
    ///   consumer.userDisabledChoreGroups      → shared; user-disabled state crosses dupes
    ///   cp.choreWorldMap (Dict)               → shared; all IdleChores land in dupe0's map
    ///
    /// Must be called BEFORE new IdleMonitor.Instance(smc) so the IdleChore created
    /// during StartSM() is routed to THIS dupe's own fresh choreWorldMap.
    /// providers is set to empty here; caller must call AddProvider(cp) after StartSM.
    /// </summary>
    private static void ResetSharedReferences(GameObject go) {
        var consumer = go.GetComponent<ChoreConsumer>();
        var cp = go.GetComponent<ChoreProvider>();
        if (consumer == null || cp == null) return;

        consumer.choreProvider = cp;
        consumer.providers = new List<ChoreProvider>();
        consumer.urges = new List<Urge>();
        consumer.behaviourPreconditions = new Dictionary<Tag, ChoreConsumer.BehaviourPrecondition>();
        consumer.preconditionSnapshot = new ChoreConsumer.PreconditionSnapshot();
        consumer.lastSuccessfulPreconditionSnapshot = new ChoreConsumer.PreconditionSnapshot();
        consumer.choreGroupPriorities = new Dictionary<HashedString, ChoreConsumer.PriorityInfo>();
        consumer.choreTypePriorities = new Dictionary<HashedString, int>();
        consumer.traitDisabledChoreGroups = new List<HashedString>();
        consumer.userDisabledChoreGroups = new List<HashedString>();

        cp.choreWorldMap = new Dictionary<int, List<Chore>>();
    }

}
