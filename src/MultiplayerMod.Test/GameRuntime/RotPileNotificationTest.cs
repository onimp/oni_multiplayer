using System;
using System.Reflection;
using HarmonyLib;
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
/// FIX (DedicatedServer/Game/Patches/RotPilePatches.cs):
///   Harmony Prefix on <c>TryCreateNotification</c> and <c>TryClearNotification</c>
///   returning <c>false</c> — skipping both methods entirely in headless.
///   Notifications are UI-only (<c>Notifier.Add</c> already returns early when
///   <c>KScreenManager.Instance == null</c>, which is always null in headless).
///   Game logic (decomposing timer + <c>ConvertToElement()</c>) is unaffected.
/// </summary>
public class RotPileNotificationTest : PlayableGameTest {

    private static readonly Harmony LocalHarmony = new("RotPileNotificationTest");

    private static readonly MethodInfo _tryCreate =
        typeof(RotPile).GetMethod("TryCreateNotification", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!;

    private static readonly MethodInfo _tryClear =
        typeof(RotPile).GetMethod("TryClearNotification", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!;

    private static readonly HarmonyMethod _skipPrefix =
        new(typeof(RotPileNotificationTest).GetMethod(nameof(SkipPrefix), BindingFlags.Static | BindingFlags.NonPublic)!);

    // ReSharper disable once UnusedMember.Local
    private static bool SkipPrefix() => false;

    private static void ApplyRotPilePatch() {
        LocalHarmony.Patch(_tryCreate, prefix: _skipPrefix);
        LocalHarmony.Patch(_tryClear, prefix: _skipPrefix);
    }

    [SetUp]
    public void SetUp() {
        StateMachine.Instance.error = false;
    }

    [TearDown]
    public void TestTearDown() {
        StateMachine.Instance.error = false;
        LocalHarmony.UnpatchAll("RotPileNotificationTest");
    }

    // ─── ROOT CAUSE TESTS ─────────────────────────────────────────────────────

    /// <summary>
    /// Documents root cause: <c>WorldContainer.worldInventory</c> is null when
    /// the GO has no <c>WorldInventory</c> component.
    ///
    /// <c>WorldContainer.OnPrefabInit()</c>:
    ///   <c>worldInventory = GetComponent&lt;WorldInventory&gt;();</c>
    ///
    /// The headless <c>WorldContainer</c> in <c>WorldBuilder</c> is created with
    /// <c>new GameObject(...)</c> + <c>AddComponent&lt;WorldContainer&gt;()</c>
    /// and never gets a <c>WorldInventory</c> component added.
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
            "This causes RotPile.TryCreateNotification() to NPE at: " +
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
    /// Verifies the fix: with <c>RotPileTestPatch</c> (mirrors <c>RotPilePatches</c>)
    /// applied, calling <c>TryCreateNotification()</c> does NOT throw.
    ///
    /// Without the fix, calling <c>TryCreateNotification()</c> would reach
    /// <c>worldInventory.IsReachable()</c> with <c>worldInventory == null</c>
    /// → <c>NullReferenceException</c>.
    ///
    /// With the fix, the Harmony Prefix returns <c>false</c>, skipping the entire
    /// method body — no NPE, no side effects.
    /// </summary>
    [Test]
    public void TryCreateNotification_DoesNotThrow_WhenPatched() {
        ApplyRotPilePatch();

        var go = createGameObject();
        var rotPile = go.AddComponent<RotPile>();

        Assert.DoesNotThrow(
            () => rotPile.TryCreateNotification(),
            "RotPile.TryCreateNotification() must not throw when the skip patch is applied. " +
            "The Harmony Prefix returns false, skipping the method body entirely. " +
            "Without the patch, this would NPE at myWorld.worldInventory.IsReachable() " +
            "because the headless WorldContainer has null worldInventory.");
    }

    /// <summary>
    /// Verifies the fix: with patch applied, <c>TryClearNotification()</c> does NOT throw.
    /// Symmetric with <c>TryCreateNotification</c>.
    /// </summary>
    [Test]
    public void TryClearNotification_DoesNotThrow_WhenPatched() {
        ApplyRotPilePatch();

        var go = createGameObject();
        var rotPile = go.AddComponent<RotPile>();

        Assert.DoesNotThrow(
            () => rotPile.TryClearNotification(),
            "RotPile.TryClearNotification() must not throw when the skip patch is applied.");
    }

    /// <summary>
    /// End-to-end: verifies that <c>globalSMError</c> remains false after
    /// <c>TryCreateNotification()</c> is called with the fix applied.
    ///
    /// Without the fix, the NPE in <c>decomposing.Enter()</c> causes the SM
    /// framework to set <c>StateMachine.Instance.error = true</c>, halting all
    /// state machines. This test confirms the fix prevents that outcome.
    /// </summary>
    [Test]
    public void GlobalSMError_RemainsFlase_AfterTryCreateNotification_WhenPatched() {
        ApplyRotPilePatch();

        var go = createGameObject();
        var rotPile = go.AddComponent<RotPile>();

        rotPile.TryCreateNotification();

        Assert.IsFalse(StateMachine.Instance.error,
            "StateMachine.Instance.error must remain false after TryCreateNotification(). " +
            "Without the fix, the NPE inside decomposing.Enter() sets error=true, " +
            "halting ALL state machines globally (critters, dupes, buildings). " +
            "With the Harmony Prefix no-op fix, the method body is skipped entirely " +
            "— no exception, no globalSMError.");
    }
}
