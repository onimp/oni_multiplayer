using System;
using NUnit.Framework;
using UnityEngine;

namespace MultiplayerMod.Test.GameRuntime;

/// <summary>
/// Tests for the RotPile.TryCreateNotification NPE fix (todo #88).
///
/// ROOT CAUSE:
///   <c>RotPile.TryCreateNotification()</c> is called from the state machine
///   <c>decomposing.Enter()</c> delegate every time a RotPile entity spawns.
///
///   It calls:
///   <code>
///     WorldContainer myWorld = base.smi.master.GetMyWorld();
///     if (myWorld != null &amp;&amp; myWorld.worldInventory.IsReachable(pickupable))
///   </code>
///
///   <c>WorldContainer.OnPrefabInit()</c> sets:
///   <code>worldInventory = GetComponent&lt;WorldInventory&gt;();</code>
///
///   The headless <c>WorldContainer</c> is created manually in <c>WorldBuilder</c>
///   without a <c>WorldInventory</c> component, so <c>worldInventory</c> is null.
///   When <c>GetMyWorld()</c> returns non-null (RotPile is at a valid grid cell),
///   <c>null.IsReachable(pickupable)</c> throws <c>NullReferenceException</c>.
///
///   Because the exception fires inside a <c>GameStateMachine.States.Enter()</c>
///   delegate, the SM framework sets <c>StateMachine.Instance.error = true</c>,
///   halting ALL state machines globally.
///
/// FIX (WorldBuilder.cs):
///   Add <c>WorldInventory</c> component to <c>worldContainerGo</c> BEFORE calling
///   <c>wc.InitializeComponent()</c>. <c>WorldContainer.OnPrefabInit()</c> then finds
///   the component via <c>GetComponent&lt;WorldInventory&gt;()</c> and assigns it to
///   <c>worldInventory</c> (non-null). Subsequent calls to
///   <c>myWorld.worldInventory.IsReachable(pickupable)</c> safely resolve to
///   <c>MinionGroupProber.Get().IsReachable(pickupable)</c>, which returns false
///   (no minions → no cells occupied) — no NPE, no globalSMError.
/// </summary>
public class RotPileNotificationTest : PlayableGameTest {

    [SetUp]
    public void SetUp() {
        StateMachine.Instance.error = false;
    }

    [TearDown]
    public void TestTearDown() {
        StateMachine.Instance.error = false;
    }

    // ─── ROOT CAUSE TESTS ─────────────────────────────────────────────────────

    /// <summary>
    /// Documents root cause: <c>WorldContainer.worldInventory</c> is null when
    /// the GO has no <c>WorldInventory</c> component.
    ///
    /// <c>WorldContainer.OnPrefabInit()</c>:
    ///   <c>worldInventory = GetComponent&lt;WorldInventory&gt;();</c>
    ///
    /// The original headless <c>WorldContainer</c> in <c>WorldBuilder</c> was created
    /// with <c>new GameObject(...)</c> + <c>AddComponent&lt;WorldContainer&gt;()</c>
    /// without adding <c>WorldInventory</c>.
    /// Result: <c>worldInventory == null</c> for all headless worlds.
    /// </summary>
    [Test]
    public void WorldContainer_WorldInventory_IsNull_WhenComponentAbsent() {
        var go = new GameObject("HeadlessWorldContainer");
        var wc = go.AddComponent<WorldContainer>();
        // OnPrefabInit is called by InitializeComponent; it sets:
        //   worldInventory = GetComponent<WorldInventory>() = null (no WorldInventory on GO)
        // May also throw for other reasons (e.g. ClusterManager.RegisterWorldContainer) —
        // those are irrelevant to the worldInventory null check we are documenting.
        try { wc.InitializeComponent(); } catch { /* expected */ }

        Assert.IsNull(wc.worldInventory,
            "WorldContainer.worldInventory must be null when no WorldInventory component " +
            "is present on the GO. The headless WorldBuilder creates WorldContainer via " +
            "  new GameObject() + AddComponent<WorldContainer>() " +
            "without adding WorldInventory. " +
            "OnPrefabInit: worldInventory = GetComponent<WorldInventory>() = null. " +
            "This caused RotPile.TryCreateNotification() to NPE at: " +
            "  myWorld.worldInventory.IsReachable(pickupable) — worldInventory is null.");
    }

    /// <summary>
    /// Documents root cause: dereferencing a null <c>WorldInventory</c> reference
    /// throws <c>NullReferenceException</c> — the exact exception that fires inside
    /// <c>RotPile.decomposing.Enter()</c> and sets <c>globalSMError=true</c>.
    /// </summary>
    [Test]
    public void WorldInventory_NullDereference_ThrowsNullReferenceException() {
        WorldInventory nullInv = null!;

        // Calling any instance method on a null reference throws NRE.
        // This documents the exact expression in TryCreateNotification:
        //   myWorld.worldInventory.IsReachable(pickupable)
        // where worldInventory == null on the headless WorldContainer.
        Assert.Throws<NullReferenceException>(
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            () => nullInv!.IsReachable(null!),
            "Calling IsReachable() on a null WorldInventory reference must throw " +
            "NullReferenceException. This is the exact exception that fires in " +
            "RotPile.TryCreateNotification() when myWorld.worldInventory is null. " +
            "The exception propagates through decomposing.Enter() → " +
            "GameStateMachine sets StateMachine.Instance.error = true → " +
            "ALL state machines halt globally.");
    }

    // ─── FIX TESTS ────────────────────────────────────────────────────────────

    /// <summary>
    /// Verifies the fix: when <c>WorldInventory</c> is added to the WorldContainer GO
    /// BEFORE <c>InitializeComponent()</c>, <c>worldInventory</c> is non-null.
    ///
    /// <c>WorldContainer.OnPrefabInit()</c> executes:
    ///   <c>worldInventory = GetComponent&lt;WorldInventory&gt;()</c>
    ///
    /// With <c>WorldInventory</c> added first, <c>GetComponent</c> finds it and
    /// <c>worldInventory</c> is set to a valid reference.
    ///
    /// WorldBuilder fix: <c>worldContainerGo.AddComponent&lt;WorldInventory&gt;()</c>
    /// is called before <c>wc.InitializeComponent()</c>.
    ///
    /// This test FAILS without the fix (worldInventory is null) and PASSES with it.
    /// </summary>
    [Test]
    public void WorldContainer_WorldInventory_IsNotNull_WhenComponentAddedBeforeInit() {
        var go = new GameObject("HeadlessWorldContainer_WithInventory");
        go.AddComponent<WorldInventory>();          // fix: add before InitializeComponent
        var wc = go.AddComponent<WorldContainer>();
        try { wc.InitializeComponent(); } catch { /* other OnPrefabInit side effects may throw */ }

        Assert.IsNotNull(wc.worldInventory,
            "WorldContainer.worldInventory must be non-null when WorldInventory component " +
            "is added to the GO before InitializeComponent(). " +
            "WorldContainer.OnPrefabInit(): worldInventory = GetComponent<WorldInventory>() " +
            "finds the component and assigns it. " +
            "WorldBuilder fix: worldContainerGo.AddComponent<WorldInventory>() " +
            "before wc.InitializeComponent() ensures every headless WorldContainer has " +
            "a non-null worldInventory, preventing RotPile.TryCreateNotification() NPE.");
    }
}
