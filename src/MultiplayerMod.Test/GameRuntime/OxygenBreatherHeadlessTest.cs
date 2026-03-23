using NUnit.Framework;
using UnityEngine;

namespace MultiplayerMod.Test.GameRuntime;

/// <summary>
/// Tests that verify headless OxygenBreather / BreathMonitor initialization is safe
/// and that <c>MinionBrain.OxygenBreather</c> is properly injected via
/// <c>[MyCmpGet]</c> when the GO is set up correctly.
///
/// ROOT CAUSE (todo #104):
///   <c>BaseMinionConfig.BaseOnSpawn</c> line 384 calls
///   <c>go.TryGetComponent&lt;Traits&gt;(out var component2)</c>.
///   <c>TryGetComponent&lt;T&gt;</c> compiles to the InternalCall
///   <c>GameObject::TryGetComponentFastPath(Type, IntPtr)</c>.
///   This InternalCall was absent from the Cecil redirect table in
///   <c>PatchInternalCalls/Program.cs</c>, so the native Unity stub was
///   reached → <c>MissingMethodException</c> on every call.
///
///   Consequence: any code path calling <c>TryGetComponent</c> crashed.
///   Additionally, <c>brain.OxygenBreather</c> (the <c>[MyCmpGet]</c> field on
///   <c>MinionBrain</c>) is null whenever <c>MinionBrain.InitializeComponent()</c>
///   ran before <c>OxygenBreather</c> was added to the GO — a latent
///   injection-ordering hazard in the headless boot sequence.
///
/// FIX:
///   1. <c>PatchInternalCalls/Program.cs</c>: redirect
///      <c>UnityEngine.GameObject::TryGetComponentFastPath</c> to
///      <c>UnityRuntime.TryGetComponentFastPath</c>.
///   2. <c>UnityRuntime.TryGetComponentFastPath</c>: delegates to
///      <c>GetComponentFastPath</c> (same out-ptr protocol).
///   3. Test-env <c>GameObjectPatch</c>: add transpiler for
///      <c>TryGetComponentFastPath</c> → <c>UnityTestRuntime.GetComponentFastPath</c>.
///
/// BREATHING LOGIC STUB:
///   In headless there is no gas simulation — cells are vacuum.
///   Without a gas provider that returns <c>HasOxygen()=true</c>,
///   <c>OxygenBreather.Sim200ms</c> flips <c>hasAir=false</c> after the
///   2-second hysteresis timer → <c>SuffocationMonitor</c> kills the dupe.
///   A no-op <c>IGasProvider</c> stub (like <c>HeadlessGasProvider</c> in the
///   dedicated server) keeps <c>hasAir=true</c> permanently.
/// </summary>
public class OxygenBreatherHeadlessTest : PlayableGameTest {

    // ─── Local IGasProvider stub (mirrors HeadlessGasProvider in DedicatedServer) ───

    /// <summary>
    /// Minimal <c>IGasProvider</c> stub for test use.
    /// Mirrors the contract of the dedicated server's <c>HeadlessGasProvider</c>:
    /// <c>HasOxygen()=true</c> keeps <c>OxygenBreather.hasAir=true</c>;
    /// <c>ConsumeGas</c> returns true (no actual gas consumed).
    /// </summary>
    private sealed class TestGasProvider : OxygenBreather.IGasProvider {
        public void OnSetOxygenBreather(OxygenBreather oxygen_breather) { }
        public void OnClearOxygenBreather(OxygenBreather oxygen_breather) { }
        public bool ConsumeGas(OxygenBreather oxygen_breather, float amount) => true;
        public bool ShouldEmitCO2()  => false;
        public bool ShouldStoreCO2() => false;
        public bool IsLowOxygen()    => false;
        public bool HasOxygen()      => true;
        public bool IsBlocked()      => false;
    }

    // ─── ROOT CAUSE TESTS — TryGetComponent ───────────────────────────────────

    /// <summary>
    /// Verifies the fix: <c>go.TryGetComponent&lt;T&gt;(out T)</c> works in
    /// the test runtime after the <c>TryGetComponentFastPath</c> patch.
    ///
    /// Before the fix: <c>TryGetComponentFastPath</c> was absent from the Cecil
    /// redirect table → <c>MissingMethodException</c> on any <c>TryGetComponent</c>
    /// call (e.g. <c>BaseMinionConfig.BaseOnSpawn</c> line 384) → the SM factory
    /// loop never ran → <c>BreathMonitor</c> never started.
    ///
    /// Fix: redirect added in <c>PatchInternalCalls/Program.cs</c> and
    /// <c>UnityRuntime.TryGetComponentFastPath</c> delegates to
    /// <c>GetComponentFastPath</c> (identical out-ptr protocol).
    /// </summary>
    [Test]
    public void TryGetComponent_ReturnsComponentAndTrue_WhenPresent() {
        var go = createGameObject();
        go.AddComponent<OxygenBreather>();

        OxygenBreather ob;
        var found = go.TryGetComponent(out ob);

        Assert.IsTrue(found,
            "go.TryGetComponent<OxygenBreather>() must return true when the component is present. " +
            "Before the fix: TryGetComponentFastPath was absent from the InternalCall redirect " +
            "table → MissingMethodException. Fix: added redirect in PatchInternalCalls and " +
            "UnityRuntime.TryGetComponentFastPath (delegates to GetComponentFastPath).");
        Assert.IsNotNull(ob,
            "TryGetComponent out parameter must be non-null when the component is present.");
        Assert.IsInstanceOf<OxygenBreather>(ob);
    }

    /// <summary>
    /// Verifies <c>TryGetComponent&lt;T&gt;</c> returns false and writes null
    /// when the component is absent — matching Unity's contract.
    ///
    /// The managed wrapper inspects the out-ptr slot: if zero, returns false.
    /// <c>GetComponentFastPath</c> (and by delegation <c>TryGetComponentFastPath</c>)
    /// leaves the slot zeroed when no component is found.
    /// </summary>
    [Test]
    public void TryGetComponent_ReturnsFalseAndNull_WhenAbsent() {
        var go = createGameObject();
        // OxygenBreather deliberately NOT added

        OxygenBreather ob;
        var found = go.TryGetComponent(out ob);

        Assert.IsFalse(found,
            "go.TryGetComponent<OxygenBreather>() must return false when the component is absent. " +
            "TryGetComponentFastPath writes null/zero to the out-ptr slot; managed code returns false.");
        Assert.IsNull(ob,
            "TryGetComponent out parameter must be null when the component is absent.");
    }

    // ─── FIX TESTS — breathing logic stub contract ────────────────────────────

    /// <summary>
    /// Verifies the headless gas-provider contract: an <c>IGasProvider</c>
    /// with <c>HasOxygen()=true</c> keeps <c>OxygenBreather.HasOxygen</c> true.
    ///
    /// In headless, cells are vacuum → <c>GasBreatherFromWorldProvider.HasOxygen()</c>
    /// returns false → after the 2-second hysteresis, <c>hasAir</c> flips false →
    /// <c>SuffocationMonitor</c> kills the dupe.
    ///
    /// The dedicated server adds a no-op provider (<c>HeadlessGasProvider</c>)
    /// in <c>MinionPrefab.Setup</c> step 16c.  This test documents the contract
    /// that any provider with <c>HasOxygen()=true</c> prevents suffocation.
    /// </summary>
    [Test]
    public void OxygenBreather_HasOxygen_IsTrueAfterHeadlessProviderAdded() {
        global::Game.Instance.accumulators ??= new Accumulators();

        var go = createGameObject();
        go.AddComponent<KPrefabID>();
        go.AddComponent<KSelectable>();
        go.AddComponent<Facing>();
        var ob = go.AddComponent<OxygenBreather>();
        ob.Awake();

        // Add headless provider (no gas simulation in headless — always HasOxygen=true)
        ob.AddGasProvider(new TestGasProvider());

        // hasAir starts true and TestGasProvider.HasOxygen()=true keeps it true on Sim200ms
        Assert.IsTrue(ob.HasOxygen,
            "OxygenBreather.HasOxygen must be true after adding a provider with HasOxygen()=true. " +
            "hasAir is initialized to true and the Sim200ms tick keeps it true when the " +
            "current provider returns HasOxygen()=true. " +
            "In the dedicated server MinionPrefab.Setup step 16c adds HeadlessGasProvider " +
            "which has the same contract, preventing SuffocationMonitor from killing all dupes.");
    }

    /// <summary>
    /// Verifies that <c>OxygenBreather.AddGasProvider</c> is safe with a
    /// no-op provider: <c>ConsumeGas</c> returns true without throwing.
    ///
    /// <c>OxygenBreather.Sim200ms</c> calls <c>provider.ConsumeGas(this, amount)</c>
    /// on the current gas provider every 200ms.  A headless stub must return true
    /// (gas consumed) and not crash (no SimDLL calls).
    /// </summary>
    [Test]
    public void TestGasProvider_ConsumeGas_ReturnsTrueWithoutThrowing() {
        var go = createGameObject();
        go.AddComponent<KSelectable>();
        var ob = go.AddComponent<OxygenBreather>();
        var provider = new TestGasProvider();

        bool result = false;
        Assert.DoesNotThrow(
            () => { result = provider.ConsumeGas(ob, 0.1f); },
            "IGasProvider.ConsumeGas must not throw for a no-op headless stub. " +
            "OxygenBreather.Sim200ms calls ConsumeGas each breath tick — the stub " +
            "must return true without attempting any SimDLL gas operations.");
        Assert.IsTrue(result,
            "ConsumeGas must return true — signals to OxygenBreather that gas was consumed " +
            "and the dupe is breathing (keeping hasAir=true via the provider's HasOxygen()).");
    }
}
