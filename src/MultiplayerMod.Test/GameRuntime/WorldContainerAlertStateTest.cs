using NUnit.Framework;

namespace MultiplayerMod.Test.GameRuntime;

/// <summary>
/// Tests for WorldContainer.AlertManager initialization in headless (DS) environment.
///
/// Root problem: BreathMonitor.IsLowBreath() calls wc.AlertManager.IsRedAlert() every SM tick.
/// In headless, WorldContainer is created manually (not via prefab spawn pipeline), so
/// KPrefabID.OnSpawn() → StateMachineController.CreateSMIS/StartSMIS never fires →
/// AlertStateManager.Instance is never created → AlertManager getter returns null →
/// null.IsRedAlert() throws NullReferenceException ~600×/frame.
///
/// Fix (WorldBuilder.cs): after AddOrGetDef&lt;AlertStateManager.Def&gt;(), explicitly call
///   smc.CreateSMIS() + smc.StartSMIS()
/// to mirror KPrefabID.OnSpawn() and create the AlertStateManager.Instance in headless.
///
/// NOTE on test harness: createGameObject() (from PlayableGameTest) must be used instead of
/// new GameObject() directly. UnityTestRuntime.AddComponent only triggers Awake() for exact
/// types in supportedComponents. createGameObject() adds base KMonoBehaviour (which IS in
/// supportedComponents) → Awake fires → KObjectManager.GetOrCreateObject → obj is set.
/// This is required by GenericInstance ctor → TargetParameter.Context.Set → KMonoBehaviour.Subscribe.
/// </summary>
public class WorldContainerAlertStateTest : PlayableGameTest {

    /// <summary>
    /// After AddOrGetDef + CreateSMIS + StartSMIS, AlertManager must be non-null.
    /// Baseline failure: without CreateSMIS/StartSMIS, GetSMI&lt;AlertStateManager.Instance&gt;()
    /// returns null → AlertManager getter returns null → BreathMonitor NPE.
    /// </summary>
    [Test]
    public void WorldContainer_AfterCreateStartSMIS_AlertManager_IsNotNull() {
        var go = createGameObject();
        var wc = go.AddComponent<WorldContainer>();

        go.AddOrGetDef<AlertStateManager.Def>();
        var smc = go.GetComponent<StateMachineController>();
        smc.CreateSMIS();
        smc.StartSMIS();

        Assert.IsNotNull(
            wc.AlertManager,
            "wc.AlertManager must be non-null after CreateSMIS/StartSMIS. " +
            "If null: AlertStateManager.Instance was not added to smc.stateMachines — " +
            "BreathMonitor.IsLowBreath() would NPE on null.IsRedAlert() every tick.");
    }

    /// <summary>
    /// After CreateSMIS + StartSMIS, IsRedAlert() must return false (default 'off' state)
    /// without throwing. This is the actual call BreathMonitor.IsLowBreath() makes.
    /// The 'off' state has no Enter actions — GoTo(off) fires no lambdas accessing
    /// Game.Instance or Vignette.Instance (those are on 'on.red' Enter only).
    /// </summary>
    [Test]
    public void WorldContainer_AfterCreateStartSMIS_IsRedAlert_ReturnsFalse_WithoutException() {
        var go = createGameObject();
        var wc = go.AddComponent<WorldContainer>();

        go.AddOrGetDef<AlertStateManager.Def>();
        var smc = go.GetComponent<StateMachineController>();
        smc.CreateSMIS();
        smc.StartSMIS();

        bool result = false;
        Assert.DoesNotThrow(
            () => result = wc.AlertManager.IsRedAlert(),
            "wc.AlertManager.IsRedAlert() must not throw after CreateSMIS/StartSMIS. " +
            "This is the exact call in BreathMonitor.IsLowBreath() that was NPEing ~600×/frame.");

        Assert.IsFalse(result,
            "IsRedAlert() must return false in default 'off' state — " +
            "no red alert has been triggered after initialization.");
    }

    /// <summary>
    /// Regression guard: without CreateSMIS/StartSMIS, GetSMI&lt;AlertStateManager.Instance&gt;()
    /// returns null, so AlertManager getter returns null.
    /// Documents the broken state that the WorldBuilder fix addresses.
    ///
    /// Note: wc.AlertManager itself throws NullReferenceException when no StateMachineController
    /// exists on the GameObject (GetComponent&lt;StateMachineController&gt;() returns null →
    /// null.GetSMI() → NPE). We verify instead that GetSMI returns null when SMC exists
    /// but CreateSMIS was never called.
    /// </summary>
    [Test]
    public void WorldContainer_WithoutCreateStartSMIS_GetSMI_IsNull() {
        var go = createGameObject();
        var wc = go.AddComponent<WorldContainer>();

        // Add SMC (via AddOrGetDef) but do NOT call CreateSMIS/StartSMIS.
        // This simulates the broken pre-fix state: def registered but SMI never instantiated.
        go.AddOrGetDef<AlertStateManager.Def>();
        var smc = go.GetComponent<StateMachineController>();
        // No CreateSMIS / StartSMIS — SMI not in smc.stateMachines

        var smi = smc.GetSMI<AlertStateManager.Instance>();
        Assert.IsNull(smi,
            "GetSMI<AlertStateManager.Instance>() must be null without CreateSMIS/StartSMIS. " +
            "Documents the broken state: AlertManager then returns null → " +
            "BreathMonitor NPEs on null.IsRedAlert().");
    }
}
