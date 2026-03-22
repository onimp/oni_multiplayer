using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace DedicatedServer.Game;

/// <summary>
/// Per-duplicant component bootstrap for the dedicated server headless environment.
///
/// DIAGNOSTIC MODE: every step is wrapped in try { ... log OK } catch { log FAIL; throw }.
/// The throw is intentional — it surfaces the exact failure point with full type/message.
/// FixRationalAi() catches per-dupe so all 3 dupes get diagnosed in a single run.
///
/// Setup(go) pipeline:
///   1.  Accumulators guard (failsafe before any OnSpawn that needs them)
///   2.  Remove render-only components (CharacterOverlay, AnimEventHandler)
///   3.  Ensure required functional components (KSelectable, ChoreProvider, …)
///   4.  EnsureAssignableProxy
///   5.  Sensors — 6 of 8 (ARS excluded)
///   6.  smc.stateMachines isolation (SMOKING GUN fix)
///   7.  ResetSharedReferences (ChoreConsumer/ChoreProvider)
///   8.  RationalAi.Instance creation
///   9.  RationalAi.StartSM (root+alive, empty sub-SM list)
///  10.  Each sub-SM individually (52 total, named)
///  11.  IdleMonitor state readout
///  12.  AddProvider
///  13.  Navigator transition layers
///  14.  Navigator SM init + start
///  15.  StandardWorker + ChoreDriver SM
///  16.  Brain.Spawn + BrainScheduler re-registration
///  17.  GameTags.Idle pre-add + Sensors.Spawn
///  18.  IdleCellSensor cell seeding
///  19.  Consumer.providers final readout
/// </summary>
public static class MinionPrefab {

    private static readonly FieldInfo _idleSensorCellField =
        typeof(IdleCellSensor).GetField("cell", BindingFlags.Instance | BindingFlags.NonPublic)!;

    // ── Step helper ─────────────────────────────────────────────────────────
    // Logs OK on success, logs FAIL + rethrows on exception.
    // The rethrow is REQUIRED: this is NOT a silent catch — it surfaces which step
    // first fails and its full stack trace.
    private static void S(string id, string step, System.Action body) {
        try {
            body();
            Console.WriteLine($"[SETUP go={id}] {step}: OK");
        } catch (Exception ex) {
            Console.WriteLine($"[SETUP go={id}] {step} FAIL: {ex.GetType().Name}: {ex.Message}");
            throw;
        }
    }

    public static void Setup(GameObject go) {
        var id = go.name;

        var smc = go.GetComponent<StateMachineController>();
        if (smc == null) {
            Console.WriteLine($"[SETUP go={id}] SKIP: StateMachineController=null");
            return;
        }

        // ── Step 1: Accumulators guard ───────────────────────────────────────
        // Failsafe: Game.OnPrefabInit crashes at ~line 820, skipping lines 823-824.
        // OxygenBreather.OnSpawn[IL_0x0021]: Game.Instance.accumulators.Add("O2", this) → NPE.
        // WorldBuilder also sets these before SpawnStarterMinions — this is defence-in-depth.
        S(id, "1-accumulators", () => {
            global::Game.Instance.accumulators ??= new Accumulators();
            global::Game.Instance.plantElementAbsorbers ??= new PlantElementAbsorbers();
        });

        // ── Step 2: Remove render-only components ────────────────────────────
        S(id, "2-remove-CharacterOverlay", () => {
            var c = go.GetComponent<CharacterOverlay>();
            if (c != null) UnityEngine.Object.DestroyImmediate(c);
        });
        S(id, "2-remove-AnimEventHandler", () => {
            var c = go.GetComponent<AnimEventHandler>();
            if (c != null) UnityEngine.Object.DestroyImmediate(c);
        });

        // ── Step 3: Ensure required functional components ────────────────────
        S(id, "3-KSelectable",                () => go.AddOrGet<KSelectable>());
        S(id, "3-ChoreProvider",              () => go.AddOrGet<ChoreProvider>());
        S(id, "3-DebugGoToMonitor",           () => go.AddOrGetDef<DebugGoToMonitor.Def>());
        S(id, "3-Schedulable",                () => go.AddOrGet<Schedulable>());
        S(id, "3-FactionAlignment",           () => go.AddOrGet<FactionAlignment>().Alignment = FactionManager.FactionID.Duplicant);
        S(id, "3-Weapon",                     () => go.AddOrGet<Weapon>());
        S(id, "3-RangedAttackable",           () => go.AddOrGet<RangedAttackable>());
        S(id, "3-OccupyArea", () => {
            var occupyArea = go.AddOrGet<OccupyArea>();
            occupyArea.objectLayers = new ObjectLayer[1];
            occupyArea.ApplyToCells = false;
            occupyArea.SetCellOffsets(new CellOffset[] { new CellOffset(0, 0), new CellOffset(0, 1) });
        });
        S(id, "3-Pickupable",                 () => go.AddOrGet<Pickupable>());
        S(id, "3-CreatureSimTemperatureTransfer", () => go.AddOrGet<CreatureSimTemperatureTransfer>());
        S(id, "3-SicknessTrigger",            () => go.AddOrGet<SicknessTrigger>());
        S(id, "3-ClothingWearer",             () => go.AddOrGet<ClothingWearer>());
        S(id, "3-SuitEquipper",               () => go.AddOrGet<SuitEquipper>());
        S(id, "3-ConsumableConsumer",         () => go.AddOrGet<ConsumableConsumer>());
        S(id, "3-MinionResume",               () => go.AddOrGet<MinionResume>());

        // ── Step 4: EnsureAssignableProxy ────────────────────────────────────
        S(id, "4-EnsureAssignableProxy", () => {
            var identity = go.GetComponent<MinionIdentity>();
            if (identity != null) EnsureAssignableProxy(go, identity);
        });

        // ── Step 5: Sensors ──────────────────────────────────────────────────
        // ARS excluded: identity.assignableProxy.Get() NPEs in headless (proxy incomplete).
        // BalloonStandCellSensor excluded: not needed for movement.
        S(id, "5-PathProberSensor",    () => { var s = go.GetComponent<Sensors>(); if (s != null) s.Add(new PathProberSensor(s)); });
        S(id, "5-SafeCellSensor",      () => { var s = go.GetComponent<Sensors>(); if (s != null) s.Add(new SafeCellSensor(s)); });
        S(id, "5-IdleCellSensor",      () => { var s = go.GetComponent<Sensors>(); if (s != null) s.Add(new IdleCellSensor(s)); });
        S(id, "5-PickupableSensor",    () => { var s = go.GetComponent<Sensors>(); if (s != null) s.Add(new PickupableSensor(s)); });
        S(id, "5-ClosestEdibleSensor", () => { var s = go.GetComponent<Sensors>(); if (s != null) s.Add(new ClosestEdibleSensor(s)); });
        S(id, "5-MingleCellSensor",    () => { var s = go.GetComponent<Sensors>(); if (s != null) s.Add(new MingleCellSensor(s)); });

        // ── Step 6: smc.stateMachines isolation ──────────────────────────────
        S(id, "6-smc.stateMachines", () => smc.stateMachines = new List<StateMachine.Instance>());

        // ── Step 7: ResetSharedReferences ────────────────────────────────────
        S(id, "7-ResetSharedRefs", () => ResetSharedReferences(go));

        // ── Steps 8-9: RationalAi.Instance + StartSM ─────────────────────────
        // Create with EMPTY sub-SM list so each sub-SM can be started individually
        // with per-step diagnostic logging below.
        RationalAi.Instance rationalAiSmi = null;
        S(id, "8-RationalAi.new", () => {
            rationalAiSmi = new RationalAi.Instance(smc, new Tag("Minion"));
            rationalAiSmi.stateMachinesToRunWhenAlive =
                Array.Empty<Func<RationalAi.Instance, StateMachine.Instance>>();
        });
        S(id, "9-RationalAi.StartSM", () => rationalAiSmi.StartSM());

        // ── Step 10: All sub-SMs individually (52 total) ─────────────────────
        // Each factory creates a SM instance (ctor → smc.AddStateMachineInstance → list)
        // then StartSM fires it. Log each one by type name + result.
        var allSmFactories = BaseMinionConfig.BaseRationalAiStateMachines()
            .Concat(new Func<RationalAi.Instance, StateMachine.Instance>[] {
                smi => new BreathMonitor.Instance(smi.master),
                smi => new SteppedInMonitor.Instance(smi.master),
                smi => new Dreamer.Instance(smi.master),
                smi => new StaminaMonitor.Instance(smi.master),
                smi => new RationMonitor.Instance(smi.master),
                smi => new CalorieMonitor.Instance(smi.master),
                smi => new BladderMonitor.Instance(smi.master),
                smi => new HygieneMonitor.Instance(smi.master),
                smi => new TiredMonitor.Instance(smi.master),
            }).ToArray();

        for (var i = 0; i < allSmFactories.Length; i++) {
            var idx = i;
            var factory = allSmFactories[idx];
            string smType = "?";
            try {
                var smInst = factory(rationalAiSmi);
                smType = smInst.GetType().Name;

                // SpeechMonitor: pure mouth-animation + audio SM — useless in headless.
                // root.Enter(CreateMouth) → SetMouthId() → Db.Get().Personalities.Get(personalityResourceId)
                // returns null when personalityResourceId=HashedString.Invalid (0x0) in headless.
                // null.speech_mouth → NullReferenceException → globalSMError=True → all dupes frozen.
                // Fix: remove from smc.stateMachines (added by ctor) and skip StartSM.
                if (smInst is SpeechMonitor.Instance) {
                    smc.stateMachines.Remove(smInst);
                    Console.WriteLine($"[SETUP go={id}] 10-SM[{idx}]={smType}: SKIPPED (headless-unsafe, mouth anim+audio)");
                    continue;
                }

                smInst.StartSM();
                Console.WriteLine($"[SETUP go={id}] 10-SM[{idx}]={smType}: OK");
            } catch (Exception ex) {
                Console.WriteLine($"[SETUP go={id}] 10-SM[{idx}]={smType} FAIL: {ex.GetType().Name}: {ex.Message}");
                // Reset error flag so remaining SMs still attempt to start.
                // This preserves the "full cascade" intent: see ALL SM failures, not just the first.
                StateMachine.Instance.error = false;
                throw;
            }
        }

        // ── Step 11: IdleMonitor readout ─────────────────────────────────────
        S(id, "11-IdleMonitor-check", () => {
            var im = smc.GetSMI<IdleMonitor.Instance>();
            if (im == null) throw new InvalidOperationException("IdleMonitor.Instance is null after SM starts");
            Console.WriteLine($"[SETUP go={id}] IdleMonitor hash={im.GetHashCode()} running={im.IsRunning()}");
        });

        // ── Step 12: AddProvider ─────────────────────────────────────────────
        S(id, "12-AddProvider", () => {
            var consumer = go.GetComponent<ChoreConsumer>();
            if (consumer != null)
                consumer.AddProvider(go.GetComponent<ChoreProvider>());
        });

        // ── Step 13: Navigator transition layers ─────────────────────────────
        S(id, "13-NavLayers", () => {
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
        });

        // ── Step 14: Navigator SM ────────────────────────────────────────────
        S(id, "14-Navigator.InitializeComponent", () => {
            var nav = go.GetComponent<Navigator>();
            if (nav != null && !nav.IsInitialized()) nav.InitializeComponent();
        });
        S(id, "14-Navigator.StartSM", () => {
            var nav = go.GetComponent<Navigator>();
            if (nav != null && nav.GetSMI() == null) nav.smi.StartSM();
        });

        // ── Step 15: StandardWorker + ChoreDriver SM ─────────────────────────
        S(id, "15-StandardWorker", () => go.AddOrGet<StandardWorker>());
        S(id, "15-ChoreDriver.StartSM", () => {
            var choreDriver = go.GetComponent<ChoreDriver>();
            if (choreDriver != null && choreDriver.GetSMI() == null)
                choreDriver.smi.StartSM();
        });

        // ── Step 16: Brain ───────────────────────────────────────────────────
        S(id, "16-Brain.Spawn", () => {
            var brain = go.GetComponent<MinionBrain>();
            if (brain != null && !brain.isSpawned) brain.Spawn();
        });
        S(id, "16-Brain.ReRegister", () => {
            var brain = go.GetComponent<MinionBrain>();
            if (brain != null && !brain.IsRunning()) {
                Components.Brains.Remove(brain);
                Components.Brains.Add(brain);
            }
            var finalBrain = go.GetComponent<MinionBrain>();
            Console.WriteLine($"[SETUP go={id}] Brain running={finalBrain?.IsRunning()}, isSpawned={finalBrain?.isSpawned}");
        });

        // ── Step 17: GameTags.Idle + Sensors.Spawn ───────────────────────────
        S(id, "17-Idle-tag", () => go.GetComponent<KPrefabID>()?.AddTag(GameTags.Idle));
        S(id, "17-Sensors.Spawn", () => {
            var sensors = go.GetComponent<Sensors>();
            if (sensors != null && !sensors.isSpawned) sensors.Spawn();
        });

        // ── Step 18: IdleCellSensor seed ─────────────────────────────────────
        S(id, "18-IdleCellSensor-seed", () => {
            var sensors = go.GetComponent<Sensors>();
            var idleSensor = sensors?.GetSensor<IdleCellSensor>();
            if (idleSensor != null && _idleSensorCellField != null) {
                var cell = Grid.PosToCell(go);
                if (Grid.IsValidCell(cell)) {
                    _idleSensorCellField.SetValue(idleSensor, cell);
                    Console.WriteLine($"[SETUP go={id}] IdleCellSensor seeded to cell {cell}");
                }
            }
        });

        // ── Step 19: Consumer.providers final readout ────────────────────────
        S(id, "19-providers-readout", () => {
            var consumer = go.GetComponent<ChoreConsumer>();
            var cp = go.GetComponent<ChoreProvider>();
            int provCount = consumer?.providers?.Count ?? -1;
            int choreCount = cp?.choreWorldMap?.Values.Sum(v => v?.Count ?? 0) ?? -1;
            Console.WriteLine($"[SETUP go={id}] providers={provCount} choresInMap={choreCount}");
            if (provCount == 0)
                Console.WriteLine($"[SETUP go={id}] WARNING: providers is empty — FindNextChore will find nothing");
        });

        Console.WriteLine($"[SETUP go={id}] ALL STEPS OK");
    }

    /// <summary>
    /// Creates a fresh MinionAssignablesProxy GO and wires it to the identity.
    /// </summary>
    private static void EnsureAssignableProxy(GameObject go, MinionIdentity identity) {
        if (identity.assignableProxy?.Get() != null) return;

        var proxyGO = new GameObject("MinionAssignablesProxy");
        proxyGO.SetActive(false);
        proxyGO.AddOrGet<Ownables>();
        proxyGO.AddOrGet<Equipment>();
        proxyGO.AddOrGet<KPrefabID>();

        var proxy = proxyGO.AddOrGet<MinionAssignablesProxy>();

        if (identity.assignableProxy == null)
            identity.assignableProxy = new Ref<MinionAssignablesProxy>();
        identity.assignableProxy.Set(proxy);
        proxy.SetTarget(identity, go);

        proxyGO.SetActive(true);
    }

    /// <summary>
    /// Resets all shared mutable reference fields on ChoreConsumer and ChoreProvider.
    /// MemberwiseClone on save-load path makes these shared across all dupe clones.
    /// Must be called BEFORE IdleMonitor.Instance(smc) so IdleChore lands in own map.
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
