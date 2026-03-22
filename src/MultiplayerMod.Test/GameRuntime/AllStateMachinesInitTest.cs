using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Klei.AI;
using MultiplayerMod.Test.GameRuntime;
using NUnit.Framework;
using UnityEngine;

namespace MultiplayerMod.Test.GameRuntime;

/// <summary>
/// Tracking test: calls BaseMinionConfig.BaseOnSpawn on a headless dupe and verifies
/// that all sub-StateMachines initialise without throwing.
///
/// EXPECTED TO FAIL INITIALLY — this deliberately exposes every SM that throws in the
/// test environment so we can fix them one by one until all 52 SMs initialise cleanly.
///
/// How to read the output when it fails:
///   • "BaseOnSpawn_CompletesWithoutException" — shows the FIRST SM that stops the chain.
///   • "AllSubSMs_EachFactory_InitializesWithoutException" — shows ALL failing SMs because
///     each factory is wrapped individually in try/catch (order-independent).
///   • "BaseOnSpawn_SMCount_Is52" — shows how many SMs actually registered after the (partial) run.
///
/// Target state (when all pass): 52 SMIs registered, each IsRunning() == true.
///
/// SM breakdown (target):
///   1  RationalAi
///   1  DeathMonitor  (RationalAi.root ToggleStateMachine)
///  43  BaseRationalAiStateMachines sub-SMs
///   1  Navigator SM  (StateMachineComponent, auto-started by OnSpawn)
///   1  ChoreDriver SM
///   ~5 TaskAvailabilityMonitor + others toggled by sub-SMs on first tick
///  ---
///  52  total
/// </summary>
[TestFixture]
public class AllStateMachinesInitTest : PlayableGameTest {

    private static readonly FieldInfo SmcStateMachinesField =
        typeof(StateMachineController)
            .GetField("stateMachines", BindingFlags.Instance | BindingFlags.NonPublic)!;

    // ─── Per-test setup / teardown ────────────────────────────────────────────

    [SetUp]
    public void SetUp() {
        // Must match AbstractChoreTest.AbstractSetUp — each test gets a clean SM environment.
        Singleton<StateMachineManager>.Instance.Clear();
        Singleton<StateMachineUpdater>.Instance.Clear();
        StateMachine.Instance.error = false;
    }

    [TearDown]
    public void TestTearDown() {
        StateMachine.Instance.error = false;
    }

    // ─── Test 1: integration — BaseOnSpawn must not throw ────────────────────

    /// <summary>
    /// Calls BaseMinionConfig.BaseOnSpawn exactly as the real game does.
    /// Asserts no exception is thrown.
    /// FAILS INITIALLY: shows the FIRST SM whose factory or StartSM() throws.
    /// </summary>
    [Test]
    public void BaseOnSpawn_CompletesWithoutException() {
        var go = CreateFullDupeGO();
        Assert.DoesNotThrow(
            () => BaseMinionConfig.BaseOnSpawn(
                go,
                new Tag("Minion"),
                BaseMinionConfig.BaseRationalAiStateMachines()
            ),
            "BaseMinionConfig.BaseOnSpawn must complete without throwing for all sub-SMs. " +
            "Fix the SM reported in the exception above, then re-run."
        );
    }

    // ─── Test 2: SM count ─────────────────────────────────────────────────────

    /// <summary>
    /// After BaseOnSpawn (ignoring any exception), checks how many SMIs actually registered.
    /// FAILS INITIALLY: count will be &lt; 52 until all SM failures are fixed.
    /// </summary>
    [Test]
    public void BaseOnSpawn_SMCount_Is52() {
        var go = CreateFullDupeGO();
        try {
            BaseMinionConfig.BaseOnSpawn(go, new Tag("Minion"), BaseMinionConfig.BaseRationalAiStateMachines());
        } catch {
            // collect whatever managed to register before the throw
        }

        var smc   = go.GetComponent<StateMachineController>();
        var smis  = GetStateMachines(smc);
        var names = string.Join(", ", smis.Select(s => s.GetType().Name));

        Assert.That(smis.Count, Is.EqualTo(52),
            $"Expected 52 SMIs after BaseOnSpawn but got {smis.Count}.  " +
            $"Currently registered: [{names}]"
        );
    }

    // ─── Test 3: comprehensive per-SM tracking ────────────────────────────────

    /// <summary>
    /// Tries each of the 43 RationalAi sub-SM factory functions individually,
    /// wrapping each in try/catch so ALL failing SMs are discovered in one run
    /// (unlike BaseOnSpawn which stops at the first failure).
    ///
    /// FAILS INITIALLY with a full table of every SM that throws and the exact error.
    /// SUCCESS: failures list is empty — all 43 factories initialise cleanly.
    ///
    /// This is the PRIMARY TRACKING TEST: run it, fix one SM at a time,
    /// re-run to confirm progress.
    /// </summary>
    [Test]
    public void AllSubSMs_EachFactory_InitializesWithoutException() {
        var go  = CreateFullDupeGO();
        var smc = go.GetComponent<StateMachineController>();

        // Create RationalAi.Instance (the target for all sub-SM factory lambdas).
        // We do NOT call StartSM() here — we only need it as a carrier for smi.master (= smc).
        RationalAi.Instance? raiSmi   = null;
        string?               raiError = null;
        try {
            raiSmi = new RationalAi.Instance(smc, new Tag("Minion"));
        } catch (Exception ex) {
            raiError = $"RationalAi.Instance ctor threw {ex.GetType().Name}: {ex.GetBaseException().Message}";
        }

        var factories = BaseMinionConfig.BaseRationalAiStateMachines();
        var failures  = new List<string>();
        var passed    = 0;

        for (int i = 0; i < factories.Length; i++) {
            if (raiSmi == null) {
                // Can't run any factory without the base RationalAi instance
                failures.Add($"[{i:00}] SKIPPED — {raiError}");
                continue;
            }

            // --- Try factory (SM ctor) ---
            StateMachine.Instance? smi    = null;
            string                  smName = $"factory[{i:00}]";
            try {
                smi    = factories[i](raiSmi);
                smName = smi.GetType().Name;
            } catch (Exception ex) {
                var loc = FirstStackLine(ex.GetBaseException());
                failures.Add(
                    $"[{i:00}] {smName}  CTOR threw {ex.GetType().Name}: " +
                    $"{ex.GetBaseException().Message}  @ {loc}"
                );
                continue;
            }

            // --- Try StartSM() ---
            try {
                smi.StartSM();
                passed++;
            } catch (Exception ex) {
                var loc = FirstStackLine(ex.GetBaseException());
                failures.Add(
                    $"[{i:00}] {smName}  StartSM threw {ex.GetType().Name}: " +
                    $"{ex.GetBaseException().Message}  @ {loc}"
                );
            }
        }

        // Build detailed report — visible in NUnit output when the assertion fails
        var regSmis = GetStateMachines(smc);
        var sb      = new StringBuilder();
        sb.AppendLine($"\n=== AllSubSMs Report: {passed}/{factories.Length} passed " +
                      $"({failures.Count} failed) ===");
        if (failures.Any()) {
            sb.AppendLine($"\nFAILING SMs ({failures.Count}):");
            foreach (var f in failures) sb.AppendLine($"  {f}");
        }
        sb.AppendLine($"\nRegistered SMIs on smc: {regSmis.Count}");
        sb.AppendLine($"  [{string.Join(", ", regSmis.Select(s => s.GetType().Name))}]");
        sb.AppendLine();

        Assert.That(failures, Is.Empty, sb.ToString());
    }

    // ─── Helper: reflect into StateMachineController.stateMachines ───────────

    private static List<StateMachine.Instance> GetStateMachines(StateMachineController smc) =>
        (List<StateMachine.Instance>) SmcStateMachinesField.GetValue(smc)!;

    private static string FirstStackLine(Exception ex) =>
        ex.StackTrace?.Split('\n').FirstOrDefault()?.Trim() ?? "<no stack>";

    // ─── Helper: build a dupe GO with all components BaseOnSpawn needs ────────

    /// <summary>
    /// Creates a dupe GameObject with the full component set required by
    /// BaseMinionConfig.BaseOnSpawn and the 43 sub-SM factories.
    ///
    /// Modelled on AbstractChoreTest.CreateMinion() but with:
    ///   • NO pre-added sensors — BaseOnSpawn adds PathProberSensor, SafeCellSensor, etc.
    ///   • StateMachineController explicitly added (target for RationalAi.Instance)
    ///   • MinionIdentity.Awake/Start run so assignableProxy is valid before
    ///     AssignableReachabilitySensor.ctor runs inside BaseOnSpawn.
    /// </summary>
    private static GameObject CreateFullDupeGO() {
        var go = createGameObject();

        // ── StateMachineController MUST come first ─────────────────────────────
        // ChoreProvider.Awake() calls FindOrAddComponent<StateMachineController>().
        // If SMC is absent when ChoreProvider.Awake() runs, the auto-add succeeds
        // but IsInitialized() == false → Debug.LogError → DebugLogHandlerPatch throws.
        go.AddComponent<StateMachineController>();

        // ── Chore infrastructure ──────────────────────────────────────────────
        go.AddComponent<ChoreProvider>();
        go.AddComponent<ChoreDriver>();
        go.AddComponent<User>();
        var consumer = go.AddComponent<ChoreConsumer>();
        consumer.Awake();

        // ── Identity / tags ───────────────────────────────────────────────────
        var kpid = go.AddComponent<KPrefabID>();
        // BaseMinion + Minion model tags required by DUPLICANTSTATS.GetStatsFor(gameObject):
        //   GetStatsFor(KPrefabID): if (!HasTag(BaseMinion)) return null
        //   Then: foreach model in AllModels if (HasTag(model)) return GetStatsFor(model)
        // Without these: DUPLICANTSTATS.GetStatsFor → null → .DiseaseImmunities → NPE
        // Fixes [11] GermExposureMonitor: inateImmunities = GetStatsFor(go).DiseaseImmunities.IMMUNITIES
        kpid.AddTag(GameTags.BaseMinion);
        kpid.AddTag(new Tag("Minion")); // GameTags.Minions.Models.AllModels includes "Minion"
        go.AddComponent<Prioritizable>();
        go.AddComponent<KSelectable>();
        go.AddComponent<ConsumableConsumer>().forbiddenTagSet = new HashSet<Tag>();
        go.AddComponent<StandardWorker>();

        // ── Animation (KBatchedAnimController needed by several monitors) ─────
        go.AddComponent<KBatchedAnimController>();
        go.AddComponent<MeshRenderer>();
        go.AddComponent<Facing>();

        // ── Modifiers / Attributes / Amounts ──────────────────────────────────
        // Use MinionModifiers (not plain Modifiers) — its OnPrefabInit adds ALL
        // Db.Attributes.resources (Sneezyness, ScaldingThreshold, QualityOfLife, GermResistance…)
        // and creates Sicknesses. Plain Modifiers only added attributes we explicitly listed.
        //
        // Fixes:
        //   [05] SneezeMonitor: Db.Get().Attributes.Sneezyness.Lookup(go) → null without this
        //   [10] SicknessMonitor: GetComponent<MinionModifiers>().sicknesses → NPE without this
        //   [11] GermExposureMonitor: GetComponent<MinionModifiers>().sicknesses → NPE without this
        //   [15] ScaldingMonitor: attributes.Get(Db.Get().Attributes.ScaldingThreshold) → null without this
        go.AddComponent<Effects>();
        go.AddComponent<Traits>();  // Modifiers.OnPrefabInit calls GetComponent<Traits>(); also
                                    // GermExposureMonitor stores traits = GetComponent<Traits>()
        var modifiers = go.AddComponent<MinionModifiers>();
        modifiers.Awake(); // → InitializeComponent → MinionModifiers.OnPrefabInit:
                           //   base.OnPrefabInit → amounts=new Amounts, sicknesses=new Sicknesses,
                           //   attributes=new Attributes (empty)
                           //   MinionModifiers adds all Db.Attributes.resources + disease amounts

        // Klei.AI.AttributeConverters component — needed by MinionModifiers.OnSpawn():
        //   SetupDependentAttribute(CarryAmount, CarryAmountFromStrength):
        //     attributeConverter.Lookup(this).Evaluate()
        //     → go.GetComponent<Klei.AI.AttributeConverters>().Get(CarryAmountFromStrength) → null
        //     → null.Evaluate() → NPE if the component is absent.
        // MUST be added AFTER modifiers.Awake() so GetAttributes() returns the populated
        // attribute set (Strength etc.) that OnPrefabInit iterates to build converter instances.
        go.AddComponent<Klei.AI.AttributeConverters>().Awake();

        // ScaldingMonitor [15]: Instance.ctor stores internalTemperature = Amounts.Temperature.Lookup(go).
        // StartSM root.Enter(SetInitialAverageExternalTemperature) reads internalTemperature.value
        // → NPE if Temperature amount was never added to the GO.
        // MinionModifiers only adds disease amounts; Temperature must be added explicitly.
        modifiers.AddAmount(Db.Get().Amounts.Temperature);

        // ── Additional components needed by sub-SMs ───────────────────────────
        go.AddComponent<MinionResume>();
        go.AddComponent<OxygenBreather>();
        go.AddComponent<ClothingWearer>();

        // ── Navigator + prefab stubs (required by PathProberSensor, AssignableReachabilitySensor) ─
        Assets.PrefabsByTag[(Tag) TargetLocator.ID]              = kpid;
        Assets.PrefabsByTag[(Tag) MinionAssignablesProxyConfig.ID] =
            createGameObject().AddComponent<MinionAssignablesProxy>().gameObject.AddComponent<KPrefabID>();
        var locGo = createGameObject();
        locGo.AddComponent<Approachable>();
        Assets.PrefabsByTag[(Tag) ApproachableLocator.ID] = locGo.AddComponent<KPrefabID>();

        var nav = go.AddComponent<Navigator>();
        nav.NavGridName     = TUNING.DUPLICANTSTATS.STANDARD.BaseStats.NAV_GRID_NAME;
        nav.CurrentNavType  = NavType.Floor;
        nav.Awake();
        nav.Start();
        nav.SetAbilities(new MinionPathFinderAbilities(nav));
        nav.NavGrid.NavTable.SetValid(19, NavType.Floor, true);

        // ── MinionIdentity ────────────────────────────────────────────────────
        // Must run Awake/Start before BaseOnSpawn so that:
        //   • assignableProxy is populated (AssignableReachabilitySensor.ctor needs it)
        //   • GetSoleOwner() / GetEquipment() work in ChoreConsumerState
        var minionIdentity = go.AddComponent<MinionIdentity>();
        minionIdentity.personalityResourceId = (HashedString) "TESTDUPE";
        minionIdentity.Awake();
        minionIdentity.Start();
        var ownables = minionIdentity.assignableProxy.Get().FindOrAdd<Ownables>();
        ownables.slots.Add(
            new OwnableSlotInstance(ownables, (OwnableSlot) Db.Get().AssignableSlots.MessStation)
        );

        // ── Sensors — add SafeCellSensor so SafeCellMonitor.Instance ctor can find it ─
        // BaseOnSpawn normally adds sensors via component.Add(new SafeCellSensor(component))
        // etc., but in the per-factory test we call factories directly without BaseOnSpawn.
        // SafeCellMonitor.Instance ctor calls GetSensor<SafeCellSensor>() which logs
        // Debug.LogError → DebugLogHandlerPatch exception if the sensor is missing.
        // SensorsPatch.Sensors_Add_Prefix skips Update() so the ctor is safe even without
        // full game state (Navigator / MinionBrain are present; Traits may be null, OK).
        var sensors = go.AddComponent<Sensors>();
        sensors.Add(new SafeCellSensor(sensors)); // needed by SafeCellMonitor (factory [23])

        // ── MinionBrain ───────────────────────────────────────────────────────
        go.AddComponent<MinionBrain>().Awake();

        return go;
    }
}
