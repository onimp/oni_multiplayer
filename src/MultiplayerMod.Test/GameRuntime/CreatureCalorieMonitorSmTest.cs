using NUnit.Framework;
using UnityEngine;

namespace MultiplayerMod.Test.GameRuntime;

/// <summary>
/// Tests that <c>CreatureCalorieMonitor</c> and <c>SolidConsumerMonitor</c> SMs
/// can be instantiated safely once <c>DietManager.Instance</c> is initialized.
///
/// ROOT CAUSE:
///   Both SM constructors call <c>DietManager.Instance.GetPrefabDiet(gameObject)</c>.
///   In headless, <c>DietManager</c> is a <c>KMonoBehaviour</c> singleton never
///   initialized by the boot sequence → <c>Instance == null</c> → NPE in the ctor
///   → partial instance (with <c>metabolism == null</c>) left in
///   <c>smc.stateMachines</c> → on any tick, <c>UpdateMetabolismCalorieModifier</c>
///   NPEs at <c>smi.metabolism.GetTotalValue()</c>.
///
/// FIX (WorldBuilder):
///   Before <c>SpawnEntities()</c>, after <c>RegisterEntities()</c> (which populates
///   <c>Assets.Prefabs</c>), add a <c>DietManager</c> GO and call
///   <c>InitializeComponent()</c>.  <c>OnPrefabInit</c> calls
///   <c>CollectSaveDiets(null)</c> → iterates prefabs → populates the diet dict →
///   sets <c>Instance = this</c>.  Both SM ctors then receive a non-null
///   <c>Instance</c> and <c>GetPrefabDiet</c> returns a <c>Diet</c> or
///   <c>null</c> (empty dict for creatures whose prefab has no matching tag —
///   harmless; creature simply has no food preference).
/// </summary>
public class CreatureCalorieMonitorSmTest : PlayableGameTest {

    [SetUp]
    public void SetUp() {
        Singleton<StateMachineManager>.Instance.Clear();
        Singleton<StateMachineUpdater>.Instance.Clear();
        StateMachine.Instance.error = false;
        // Ensure no DietManager from a previous test bleeds into this one.
        DietManager.DestroyInstance();
    }

    [TearDown]
    public void TestTearDown() {
        StateMachine.Instance.error = false;
        DietManager.DestroyInstance();
    }

    // ─── ROOT CAUSE TESTS ─────────────────────────────────────────────────────

    /// <summary>
    /// Documents root cause: <c>DietManager.Instance</c> is null in the test
    /// environment (mirrors headless server before the fix).
    ///
    /// WorldBuilder never created a DietManager before this fix, so any creature
    /// spawned during <c>SpawnEntities()</c> had its
    /// <c>CreatureCalorieMonitor.Instance</c> and <c>SolidConsumerMonitor.Instance</c>
    /// ctors throw NullReferenceException at <c>DietManager.Instance.GetPrefabDiet()</c>.
    /// </summary>
    [Test]
    public void DietManager_Instance_IsNull_WhenNotInitialized() {
        Assert.IsNull(DietManager.Instance,
            "DietManager.Instance must be null when no DietManager has been initialized. " +
            "This mirrors the headless server state before the fix: WorldBuilder never " +
            "created a DietManager → GetPrefabDiet() → NPE in Stomach ctor / " +
            "SolidConsumerMonitor ctor → partial SM instance with metabolism=null → " +
            "UpdateMetabolismCalorieModifier NPE on every tick.");
    }

    /// <summary>
    /// Concrete proof of the crash path: <c>SolidConsumerMonitor.Instance.ctor</c>
    /// line 36 (<c>diet = DietManager.Instance.GetPrefabDiet(gameObject)</c>) throws
    /// <c>NullReferenceException</c> when <c>DietManager.Instance</c> is null.
    ///
    /// <c>SolidConsumerMonitor.Instance</c> is simpler than
    /// <c>CreatureCalorieMonitor.Instance</c> (fewer dependencies after the base ctor)
    /// and cleanly isolates the DietManager dependency without unrelated NPEs.
    /// </summary>
    [Test]
    public void SolidConsumerMonitor_Ctor_Throws_WhenDietManagerInstanceNull() {
        Assert.IsNull(DietManager.Instance, "Precondition: DietManager.Instance must be null");

        var go  = createGameObject();
        var smc = go.AddComponent<StateMachineController>();
        go.AddComponent<KPrefabID>();
        var def = go.AddOrGetDef<SolidConsumerMonitor.Def>();

        Assert.Throws<System.NullReferenceException>(
            () => { var _ = new SolidConsumerMonitor.Instance(smc, def); },
            "SolidConsumerMonitor.Instance ctor must throw NullReferenceException when " +
            "DietManager.Instance is null. Line 36: " +
            "diet = DietManager.Instance.GetPrefabDiet(base.gameObject) " +
            "→ DietManager.Instance is null → NPE. " +
            "This is the exact crash that caused both SMs to be skipped in " +
            "CreaturePrefab step 6c and MinionPrefab.IsHeadlessUnsafeSM.");
    }

    // ─── FIX TESTS ────────────────────────────────────────────────────────────

    /// <summary>
    /// Verifies the fix precondition: <c>InitializeComponent()</c> on a
    /// <c>DietManager</c> sets <c>Instance</c> to non-null.
    ///
    /// In WorldBuilder this is: <c>dietManagerGo.AddComponent&lt;DietManager&gt;().InitializeComponent()</c>.
    /// <c>OnPrefabInit</c> calls <c>CollectSaveDiets(null)</c> (iterates <c>Assets.Prefabs</c>)
    /// then sets <c>Instance = this</c>.  In the test environment <c>Assets.Prefabs</c> is
    /// mostly empty so the dict has no entries — but <c>Instance</c> is set and
    /// <c>GetPrefabDiet</c> returns <c>null</c> rather than throwing.
    /// </summary>
    [Test]
    public void DietManager_Instance_IsNotNull_AfterInitializeComponent() {
        var go = new GameObject("DietManager");
        var dm = go.AddComponent<DietManager>();
        dm.InitializeComponent(); // OnPrefabInit: CollectSaveDiets → Instance = this

        Assert.IsNotNull(DietManager.Instance,
            "DietManager.Instance must be non-null after InitializeComponent(). " +
            "OnPrefabInit calls CollectSaveDiets(null) then sets Instance = this. " +
            "WorldBuilder fix adds this call before SpawnEntities() so all creature " +
            "ctors that call GetPrefabDiet() find a non-null Instance.");
    }

    /// <summary>
    /// Verifies the fix: <c>SolidConsumerMonitor.Instance.ctor</c> does NOT throw
    /// when <c>DietManager.Instance</c> is initialized.
    ///
    /// With an empty diet dict (test env has no critter prefabs registered),
    /// <c>GetPrefabDiet(go)</c> returns <c>null</c> and assigns it to
    /// <c>SolidConsumerMonitor.Instance.diet</c> — no NPE.
    /// This is the same path taken on the real server for creatures whose prefab
    /// tag has no matching diet entry.
    ///
    /// This test FAILS without the WorldBuilder DietManager initialization
    /// (DietManager.Instance is null → NPE) and PASSES with it.
    /// </summary>
    [Test]
    public void SolidConsumerMonitor_Ctor_DoesNotThrow_WhenDietManagerInitialized() {
        // Initialize DietManager (mirrors WorldBuilder fix)
        var dmGo = new GameObject("DietManager");
        dmGo.AddComponent<DietManager>().InitializeComponent();
        Assert.IsNotNull(DietManager.Instance, "DietManager must be initialized for this test");

        var go  = createGameObject();
        var smc = go.AddComponent<StateMachineController>();
        go.AddComponent<KPrefabID>();
        var def = go.AddOrGetDef<SolidConsumerMonitor.Def>();

        Assert.DoesNotThrow(
            () => { var _ = new SolidConsumerMonitor.Instance(smc, def); },
            "SolidConsumerMonitor.Instance ctor must not throw when DietManager.Instance " +
            "is initialized. GetPrefabDiet returns null (empty dict in test env) → " +
            "diet = null (valid state: creature has no food preference). " +
            "Without the WorldBuilder DietManager init, this throws NullReferenceException.");
    }

    /// <summary>
    /// Verifies that <c>DietManager.GetPrefabDiet</c> returns <c>null</c> (not NPE)
    /// for a GO whose prefab tag is not registered in the diet dict.
    ///
    /// This is the expected behavior for creatures in the test env (no prefab diets
    /// are registered since <c>Assets.Prefabs</c> is empty). <c>diet == null</c>
    /// means the creature doesn't eat anything — harmless for headless simulation.
    /// </summary>
    [Test]
    public void DietManager_GetPrefabDiet_ReturnsNull_ForUnregisteredPrefab() {
        var dmGo = new GameObject("DietManager");
        dmGo.AddComponent<DietManager>().InitializeComponent();

        var go   = createGameObject();
        go.AddComponent<KPrefabID>();
        var diet = DietManager.Instance.GetPrefabDiet(go);

        Assert.IsNull(diet,
            "GetPrefabDiet must return null (not throw) for a GO whose prefab tag " +
            "is not in the diet dict. In test env, Assets.Prefabs is empty so the " +
            "dict has no entries — GetPrefabDiet returns null, which is the normal " +
            "code path for TryGetValue failure (line 115: TryGetValue → return null).");
    }
}
