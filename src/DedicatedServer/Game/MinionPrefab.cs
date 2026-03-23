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

    // Counter used to assign distinct personalities to fresh-spawn dupes whose
    // personalityResourceId is HashedString.Invalid (no save-file data).
    // Each Setup() call that needs a fallback personality increments this.
    private static int _personalityCounter;

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
        // HeadlessAnimController: no-op KAnimControllerBase stub so that
        // StandardWorker.StartWork / Work / InternalStopWork calls like
        //   GetComponent<KAnimControllerBase>().Offset += ...
        //   GetComponent<KAnimControllerBase>().Play(...)
        //   GetComponent<KAnimControllerBase>().IsStopped()
        // return a non-null object instead of NPE-ing.
        S(id, "3-HeadlessAnimController",     () => go.AddOrGet<HeadlessAnimController>());
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

        // ── Step 6a: ensure personalityResourceId is valid ────────────────────
        // Must run BEFORE step 6b so the name lookup has a valid ID to work with.
        // Save-loaded dupes already carry their own unique personalityResourceId;
        // fresh headless spawns default to HashedString.Invalid for all dupes.
        // For fresh spawns, cycle through available personalities so each dupe gets
        // a distinct one rather than all sharing personalities[0].
        // SpeechMonitor.CreateMouth also needs this (Personalities.Get → speech_mouth).
        S(id, "6a-PersonalityId", () => {
            var identity = go.GetComponent<MinionIdentity>();
            if (identity == null) {
                Console.WriteLine($"[SETUP go={id}] 6a: SKIP — no MinionIdentity");
                return;
            }
            var personalities = Db.Get().Personalities.resources;
            // Diagnostic: always log current state so we can see what each dupe carries
            Console.WriteLine($"[SETUP go={id}] 6a-DIAG: personalityResourceId={identity.personalityResourceId} " +
                $"isInvalid={identity.personalityResourceId == HashedString.Invalid} " +
                $"personalities.Count={personalities?.Count ?? 0} " +
                $"_counter={_personalityCounter}");
            if (personalities?.Count > 0) {
                // Log first 3 personality names so we know what's in Db
                for (int pi = 0; pi < Math.Min(3, personalities.Count); pi++)
                    Console.WriteLine($"[SETUP go={id}] 6a-DIAG: personalities[{pi}].Id={personalities[pi].Id} .Name={personalities[pi].Name}");
            }
            if (identity.personalityResourceId == HashedString.Invalid) {
                if (personalities?.Count > 0) {
                    var idx = System.Threading.Interlocked.Increment(ref _personalityCounter) % personalities.Count;
                    identity.personalityResourceId = personalities[idx].Id;
                    Console.WriteLine($"[SETUP go={id}] 6a: was invalid — assigned idx={idx} id={personalities[idx].Id} name={personalities[idx].Name}");
                }
            } else {
                var resolved = Db.Get().Personalities.TryGet(identity.personalityResourceId);
                Console.WriteLine($"[SETUP go={id}] 6a: already valid — resolved name={resolved?.Name ?? "NULL"} (id={identity.personalityResourceId})");
            }
        });

        // ── Step 6b: unique dupe name ─────────────────────────────────────────
        // After MemberwiseClone on save-load, all 3 dupes share the same go.name
        // (e.g. "Alisa" for all three). Each MinionIdentity has its own serialized
        // nameStringKey and personalityResourceId from the save file — use those.
        // personalityResourceId is now guaranteed valid from step 6a above.
        // SetName(name) sets both identity.name (MonoBehaviour.name) and go.name.
        S(id, "6b-unique-name", () => {
            var identity = go.GetComponent<MinionIdentity>();
            if (identity == null) return;
            // Prefer personality Name (canonical display name); fall back to nameStringKey.
            string uniqueName = null;
            if (identity.personalityResourceId != HashedString.Invalid)
                uniqueName = Db.Get().Personalities.Get(identity.personalityResourceId)?.Name;
            if (string.IsNullOrEmpty(uniqueName))
                uniqueName = identity.nameStringKey;
            if (!string.IsNullOrEmpty(uniqueName) && go.name != uniqueName) {
                identity.SetName(uniqueName);
                Console.WriteLine($"[SETUP] renamed dupe → {uniqueName}");
            }
        });

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

        // ── Step 9b: Calorie repair ───────────────────────────────────────────
        // If the save file has this dupe with calories=0, CalorieMonitor.Instance will
        // immediately enter the depleted state on StartSM → Kill() → DeathMonitor.Kill(Starvation)
        // → dying_duplicant → DieChore → die.Enter (b__9_5) → KFMOD/Messenger crash.
        // Fix: reset to 50% before CalorieMonitor SM is created so it starts in satisfied.
        // Db.Get().Amounts.Calories.Lookup() finds the existing AmountInstance on this dupe.
        S(id, "9b-CalorieRepair", () => {
            var calories = Db.Get().Amounts.Calories.Lookup(go);
            if (calories != null && calories.value <= 0) {
                calories.value = calories.GetMax() * 0.5f;
                Console.WriteLine($"[SETUP go={id}] 9b: calories were 0 — reset to 50% ({calories.value:F0} kcal)");
            }
        });

        // Step 9c removed: personality assignment moved to step 6a (before name lookup in 6b).

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

                // SMs that are headless-unsafe: remove from smc.stateMachines and skip StartSM.
                // These are purely cosmetic or require game objects not available in headless.
                // The ctor already added the instance to smc.stateMachines — must Remove() before continuing.
                if (IsHeadlessUnsafeSM(smInst)) {
                    smc.stateMachines.Remove(smInst);
                    Console.WriteLine($"[SETUP go={id}] 10-SM[{idx}]={smType}: SKIPPED (headless-unsafe)");
                    continue;
                }

                smInst.StartSM();
                // Guard against SMs that set the static error flag without throwing.
                // GoTo() short-circuits when error=true → IdleMonitor.StartSM() → GoTo(idle)
                // returns immediately → currentState=null → IsRunning()=false → no IdleChore.
                // Resetting here ensures every subsequent SM starts with a clean error state.
                if (StateMachine.Instance.error) {
                    Console.WriteLine($"[SETUP go={id}] 10-SM[{idx}]={smType}: WARNING set Instance.error=True, resetting");
                    StateMachine.Instance.error = false;
                }
                Console.WriteLine($"[SETUP go={id}] 10-SM[{idx}]={smType}: OK");
            } catch (Exception ex) {
                Console.WriteLine($"[SETUP go={id}] 10-SM[{idx}]={smType} FAIL: {ex.GetType().Name}: {ex.Message}");
                Console.WriteLine($"[SETUP go={id}] 10-SM[{idx}]={smType} STACK: {ex.StackTrace}");
                // Reset error flag and CONTINUE so remaining SMs (including IdleMonitor) still start.
                // throw would exit the loop at the first failure → dupes 1+2 miss IdleMonitor.
                StateMachine.Instance.error = false;
                continue;
            }
        }

        // Purge null entries left by failed SM ctors or headless-unsafe removals.
        // StateMachineController.GetSMI<T>() iterates stateMachines — a null entry NPEs.
        smc.stateMachines.RemoveAll(s => s == null);

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

        // ── Step 16c: HeadlessGasProvider — prevents suffocation death ──────────
        // OxygenBreather.hasAir starts true but flips to false after a 2-second
        // hysteresis timer when Sim200ms finds no gas provider with HasOxygen()=true
        // (headless: no O2 simulation, cells are vacuum).
        // hasAir=false → OxygenBreatherHasAirChanged event → SuffocationMonitor
        // transitions satisfied→noOxygen → breath drains → Kill(Deaths.Suffocation).
        // HeadlessGasProvider.HasOxygen()=true keeps hasAir=true permanently.
        // SafeCellQuery.GetFlags(): current-cell breathability uses brain.OxygenBreather.HasOxygen
        // (now true); adjacent cells use GasBreatherFromWorldProvider which reads actual
        // Grid.Element from the loaded save (valid gas data, not vacuum near colony spawn).
        S(id, "16c-HeadlessGasProvider", () => {
            var ob = go.GetComponent<OxygenBreather>();
            ob?.AddGasProvider(new HeadlessGasProvider());
            Console.WriteLine($"[SETUP go={id}] 16c: HeadlessGasProvider added (HasOxygen=true, prevents suffocation)");
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
    /// Returns true for SMs that crash in headless and have no value there.
    /// These are removed from smc.stateMachines before StartSM is called.
    /// </summary>
    private static bool IsHeadlessUnsafeSM(StateMachine.Instance smi) =>
        smi is CreatureCalorieMonitor.Instance // requires DietManager (not initialized in headless)
     || smi is CreatureThoughtGraph.Instance;  // creature thought-bubble UI — same crash pattern as ThoughtGraph
    // Removed from skip list (now safe):
    // SpeechMonitor.Instance   — SetMouthId NPE fixed: step 9c ensures personalityResourceId is valid
    // ThoughtGraph.Instance    — safe: NameDisplayScreen stubbed, SpeechMonitor live (BeginTalking no longer NPEs)
    // CalorieMonitor.Instance  — safe: ThoughtGraph running → GetSMI<ThoughtGraph.Instance>() returns live instance
    // RationMonitor.Instance   — safe: SaveGame+ColonyRationMonitor initialized before SpawnStarterMinions
    // RadiationMonitor.Instance — safe: per-SM error reset (commit 2db37e0) prevents error propagation to
    //   IdleMonitor. Original skip was added before the error reset existed. With the reset in place,
    //   any NPE in RadiationMonitor.StartSM() sets error=true, is immediately reset, and does NOT
    //   prevent subsequent SMs (IdleMonitor) from starting normally. RadiationBalance=0 for fresh dupes
    //   → no sick/deadly transitions → no spurious Dying tag. Bionic dupes with genuine high radiation
    //   stay incapacitated (correct game behavior).

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
