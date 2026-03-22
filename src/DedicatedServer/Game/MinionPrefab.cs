using System;
using System.Reflection;
using UnityEngine;

namespace DedicatedServer.Game;

/// <summary>
/// Per-duplicant component bootstrap for the dedicated server headless environment.
///
/// The real game runs MinionConfig.OnSpawn() → BaseMinionConfig.BaseOnSpawn() from within
/// Unity's normal lifecycle (Awake/Start/OnEnable fire in order). In headless the lifecycle
/// may be partial (no display, no audio, assets incomplete) and several sub-steps throw.
///
/// Setup(go) replicates the full pipeline for one dupe GO:
///   1. EnsureAssignableProxy  — proxy GO required by AssignableReachabilitySensor.ctor
///   2. BaseMinionConfig.BaseOnSpawn — adds 8 sensors, starts all 52 SMs via RationalAi
///   3. Fallback (if BaseOnSpawn throws) — minimal sensors + critical monitors
///   4. Navigator SM start
///   5. ChoreDriver SM start + StandardWorker guard
///   6. Brain.Spawn  — sets running=true, registers with BrainScheduler
///   7. IdleTag pre-add + Sensors.Spawn
///   8. IdleCellSensor seeding to own cell
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

        // ValidateProxy BEFORE BaseOnSpawn: AssignableReachabilitySensor.ctor calls
        // identity.assignableProxy.Get() → NPE if null.
        var identity = go.GetComponent<MinionIdentity>();
        if (identity != null)
            EnsureAssignableProxy(go, identity);

        // Ensure proxy GO has Ownables + Equipment (both : Assignables).
        // AssignableReachabilitySensor.ctor calls proxy.GetComponents<Assignables>() — if
        // the proxy was created from Assets.GetPrefab("MinionAssignablesProxy") (headless
        // prefab is incomplete) those components may be missing → GetComponents returns empty
        // → [0x00028] NPE → BaseOnSpawn fails.
        // Our direct-creation fallback in EnsureAssignableProxy already adds them, but
        // if ValidateProxy() succeeded and returned the game's prefab-based proxy we still
        // need to patch it here.
        var proxyGo = identity?.assignableProxy?.Get()?.gameObject;
        if (proxyGo != null) {
            var assignables = proxyGo.GetComponents<Assignables>();
            if (assignables == null || assignables.Length == 0) {
                proxyGo.AddOrGet<Ownables>();
                proxyGo.AddOrGet<Equipment>();
                Debug.LogWarning($"[FixRationalAi] {go.name}: added Ownables+Equipment to proxy GO");
            }
        }

        // Diagnostic: pre-BaseOnSpawn proxy/slot state
        {
            var proxy2      = identity?.assignableProxy?.Get();
            var proxyGo2    = proxy2?.gameObject;
            var ownables2   = proxyGo2?.GetComponent<Ownables>();
            var equipment2  = proxyGo2?.GetComponent<Equipment>();
            var assignables2 = proxyGo2?.GetComponents<Assignables>();
            var slots2      = Db._Instance?.AssignableSlots;
            Debug.LogWarning($"[FixRationalAi] {go.name} pre-BaseOnSpawn: " +
                $"proxy={proxy2 != null} ownables={ownables2 != null} equipment={equipment2 != null} " +
                $"assignables.Length={assignables2?.Length ?? -1} " +
                $"slotsConfigured={proxy2?.slotsConfigured} " +
                $"Db.AssignableSlots={slots2 != null} " +
                $"Db.AssignableSlots.resources={(slots2?.resources == null ? "null" : slots2.resources.Count.ToString())}");
        }

        // Primary: BaseMinionConfig.BaseOnSpawn — adds all 8 sensors, starts all 52 SMs
        // via RationalAi.Instance.StartSM(), adds 7 navigator transition layers.
        // Mirrors exactly what the real game does in MinionConfig.OnSpawn().
        var baseOnSpawnOk = false;
        try {
            BaseMinionConfig.BaseOnSpawn(go, new Tag("Minion"), BaseMinionConfig.BaseRationalAiStateMachines());
            baseOnSpawnOk = true;
            Debug.LogWarning($"[FixRationalAi] {go.name}: baseOnSpawnOk=True");
        } catch (Exception ex) {
            // Full chain: type, message, full stack, inner exception
            Debug.LogWarning($"[FixRationalAi] {go.name}: BaseOnSpawn EXCEPTION: {ex.GetType().Name}: {ex.Message}\nStack: {ex.StackTrace}\nInner: {ex.InnerException}");
            // Directly probe AssignableReachabilitySensor ctor to isolate exact crash line
            try {
                var sensors = go.GetComponent<Sensors>();
                var ars = new AssignableReachabilitySensor(sensors);
                Debug.LogWarning($"[FixRationalAi] {go.name}: Direct ARS ctor: OK (unexpected)");
            } catch (Exception e2) {
                Debug.LogWarning($"[FixRationalAi] {go.name}: Direct ARS ctor CRASH: {e2.GetType().Name}: {e2.Message}\nStack: {e2.StackTrace}");
            }
        }

        if (!baseOnSpawnOk) {
            // Fallback: minimal sensor set + critical monitors only.
            // PathProberSensor + SafeCellSensor: needed for SafeFlags / IdleCellQuery allMet.
            // IdleCellSensor: finds the idle cell Brain picks for IdleChore.
            // AssignableReachabilitySensor EXCLUDED: ctor NPEs on assignableProxy.Get().
            var sensorsFb = go.GetComponent<Sensors>();
            if (sensorsFb != null) {
                sensorsFb.Add(new PathProberSensor(sensorsFb));
                sensorsFb.Add(new SafeCellSensor(sensorsFb));
                sensorsFb.Add(new IdleCellSensor(sensorsFb));
            }

            // IdleMonitor first so IdleChore exists in ChoreProvider before other monitors.
            var idleMonitorSmi = new IdleMonitor.Instance(smc);
            idleMonitorSmi.StartSM();
            Console.WriteLine($"[FixRationalAi] {go.name}: fallback IdleMonitor started");

            StartMonitor<BreathMonitor>(smc,  "BreathMonitor");
            StartMonitor<CalorieMonitor>(smc, "CalorieMonitor");
            StartMonitor<BladderMonitor>(smc, "BladderMonitor");
            StartMonitor<StaminaMonitor>(smc, "StaminaMonitor");

            // Navigator transition layers (BaseOnSpawn adds them; must add manually in fallback).
            var navFb = go.GetComponent<Navigator>();
            if (navFb?.transitionDriver != null) {
                navFb.transitionDriver.overrideLayers.Add(new BipedTransitionLayer(navFb, 3.325f, 2.5f));
                navFb.transitionDriver.overrideLayers.Add(new DoorTransitionLayer(navFb));
                navFb.transitionDriver.overrideLayers.Add(new LadderDiseaseTransitionLayer(navFb));
                navFb.transitionDriver.overrideLayers.Add(new NavTeleportTransitionLayer(navFb));
            }
        }

        // Navigator SM: BaseOnSpawn adds transition layers but does NOT call nav.smi.StartSM().
        // The SM must be started so normal.moving fires and Navigator.Advance() works.
        var nav3 = go.GetComponent<Navigator>();
        if (nav3 != null && !nav3.IsInitialized()) {
            try {
                nav3.InitializeComponent();
                Console.WriteLine($"[FixRationalAi] {go.name}: Navigator.InitializeComponent OK, NavGrid={nav3.NavGrid?.id ?? "null"}");
            } catch (Exception ex) {
                Console.WriteLine($"[FixRationalAi] {go.name}: Navigator.InitializeComponent partial: {ex.GetBaseException().Message}");
            }
        }
        if (nav3 != null && nav3.GetSMI() == null) {
            try {
                nav3.smi.StartSM(); // lazy-creates instance, transitions to normal.stopped
                Console.WriteLine($"[FixRationalAi] {go.name}: Navigator.smi.StartSM() called, smi={nav3.GetSMI() != null}");
            } catch (Exception ex) {
                Console.WriteLine($"[FixRationalAi] {go.name}: Navigator.smi.StartSM partial: {ex.GetBaseException().Message}");
            }
        }

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

        // DS-007: ensure the dupe's own ChoreProvider is in ChoreConsumer.providers.
        // MinionModifiers.OnSpawn() normally calls AddProvider(GlobalChoreProvider.Instance)
        // AND AddProvider(choreProvider) via OnPrefabInit's providers.Add(choreProvider).
        // For clones whose providers list was reset to empty by CloneSingle (fdb3d2c),
        // MinionModifiers.OnSpawn() may not re-run → list stays empty → FindNextChore
        // iterates 0 providers → always returns false → no chore assigned → no movement.
        // Same single-line fix as CreaturePrefab.Setup().
        var consumer = go.GetComponent<ChoreConsumer>();
        if (consumer != null)
            consumer.AddProvider(go.GetComponent<ChoreProvider>());

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

        Console.WriteLine($"[FixRationalAi] {go.name}: OK (baseOnSpawnOk={baseOnSpawnOk})");
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
    private static void EnsureAssignableProxy(GameObject go, MinionIdentity identity) {
        if (identity.assignableProxy?.Get() != null) return;

        // Try the standard game path first.
        try {
            identity.ValidateProxy();
            if (identity.assignableProxy?.Get() != null) {
                Debug.LogWarning($"[FixRationalAi] {go.name}: ValidateProxy OK");
                return;
            }
            Debug.LogWarning($"[FixRationalAi] {go.name}: ValidateProxy returned but proxy still null — using direct creation");
        } catch (Exception ex) {
            Debug.LogWarning($"[FixRationalAi] {go.name}: ValidateProxy threw ({ex.GetBaseException().Message}) — using direct proxy creation");
        }

        // Fallback: create the MinionAssignablesProxy GO directly, bypassing Assets.GetPrefab.
        // Replicates MinionAssignablesProxyConfig.CreatePrefab() + InitAssignableProxy().
        // Create INACTIVE so Awake/OnPrefabInit fires only after SetTarget is called.
        try {
            var proxyGO = new GameObject("MinionAssignablesProxy");
            proxyGO.SetActive(false);          // suppress Awake until fully wired
            proxyGO.AddOrGet<Ownables>();      // AssignableReachabilitySensor.GetComponents<Assignables>()
            proxyGO.AddOrGet<Equipment>();     // ConfigureAssignableSlots needs Equipment for EquipmentSlots
            proxyGO.AddOrGet<KPrefabID>();     // Ref<T>.Set(proxy) calls proxy.GetComponent<KPrefabID>().InstanceID
                                               //   → NPE if KPrefabID absent → proxy obj field never stored
                                               //   → assignableProxy.Get() returns null in ARS.ctor → NPE
            var proxy = proxyGO.AddOrGet<MinionAssignablesProxy>();

            // Wire ref BEFORE SetActive so OnPrefabInit (fired on activation) can use it.
            if (identity.assignableProxy == null)
                identity.assignableProxy = new Ref<MinionAssignablesProxy>();
            identity.assignableProxy.Set(proxy);
            proxy.SetTarget(identity, go);    // mirrors SetTarget call in InitAssignableProxy

            proxyGO.SetActive(true);           // Awake fires → OnPrefabInit → ConfigureAssignableSlots
            Debug.LogWarning($"[FixRationalAi] {go.name}: direct proxy creation OK, proxy={identity.assignableProxy.Get() != null}");
        } catch (Exception ex) {
            Debug.LogWarning($"[FixRationalAi] {go.name}: direct proxy creation FAILED:\n{ex}");
        }
    }

    /// <summary>
    /// Starts a StateMachine.Instance for the given SM type on the target.
    /// Mirrors the IdleMonitor pattern: new TSM.Instance(target); instance.StartSM().
    /// Uses reflection to construct TSM.Instance — avoids complex nested generic constraints.
    /// Wrapped in try/catch — monitors access Db.Amounts which may be partial in headless.
    /// </summary>
    private static void StartMonitor<TSM>(IStateMachineTarget target, string name)
        where TSM : StateMachine {
        try {
            var instanceType = typeof(TSM).GetNestedType("Instance");
            if (instanceType == null) {
                Console.WriteLine($"[StartMonitor] {name}: nested Instance type not found");
                return;
            }
            var instance = (StateMachine.Instance)Activator.CreateInstance(instanceType, target);
            instance.StartSM();
            Console.WriteLine($"[StartMonitor] {name}: started OK");
        } catch (Exception ex) {
            Console.WriteLine($"[StartMonitor] {name}: {ex.GetBaseException().Message}");
        }
    }
}
