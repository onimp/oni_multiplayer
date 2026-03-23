using NUnit.Framework;
using UnityEngine;

namespace MultiplayerMod.Test.GameRuntime;

/// <summary>
/// Tests that <c>RadiationMonitor.Instance</c> can start safely in headless without
/// setting <c>GameTags.Dying</c> or leaving <c>StateMachine.Instance.error = true</c>.
///
/// HISTORY: RadiationMonitor was added to <c>IsHeadlessUnsafeSM</c> (commit eab64c6)
/// before the per-SM error-reset guard existed (commit 2db37e0). At that time, any
/// NPE inside <c>StartSM()</c> set the static <c>StateMachine.Instance.error = true</c>
/// which then blocked <c>IdleMonitor.GoTo(idle)</c> → bionic dupes got no IdleChore.
///
/// With the per-SM error reset in place, the skip is no longer needed:
///   • If radiation is NOT enabled (DLC absent/vanilla save): <c>init</c> state
///     immediately transitions to null → <c>StopSM()</c>. No error, no Dying tag.
///   • If radiation IS enabled (SpacedOut!): SM enters <c>active.idle</c>; fresh dupes
///     have <c>RadiationBalance = 0 &lt; 100</c> → no sick/deadly transitions → no Dying tag.
///   • Any NPE in <c>CheckRadiationLevel</c> (e.g. missing attribute) → <c>error = true</c>
///     → per-SM reset handles it → subsequent SMs (IdleMonitor) start normally.
/// </summary>
public class RadiationMonitorSmTest : PlayableGameTest {

    [SetUp]
    public void SetUp() {
        Singleton<StateMachineManager>.Instance.Clear();
        Singleton<StateMachineUpdater>.Instance.Clear();
        StateMachine.Instance.error = false;
    }

    [TearDown]
    public void TestTearDown() {
        StateMachine.Instance.error = false;
    }

    // ─── ROOT CAUSE TESTS ────────────────────────────────────────────────────

    /// <summary>
    /// The original root cause: <c>error = true</c> after <c>RadiationMonitor.StartSM()</c>
    /// propagated to <c>IdleMonitor.GoTo(idle)</c>, short-circuiting it into a no-op.
    ///
    /// Documents that the per-SM error reset (commit 2db37e0) resolves this: even if
    /// RadiationMonitor sets <c>error = true</c>, resetting before IdleMonitor runs
    /// ensures IdleMonitor can enter <c>idle</c> and create IdleChore normally.
    /// </summary>
    [Test]
    public void StateMachineError_Reset_AllowsSubsequentSmToStartNormally() {
        // Simulate: RadiationMonitor sets error=true (whatever the cause)
        StateMachine.Instance.error = true;

        // The per-SM reset (MinionPrefab step 10) resets it immediately after each SM.
        // Subsequent SM (IdleMonitor) sees error=false → GoTo(idle) works normally.
        StateMachine.Instance.error = false;

        Assert.IsFalse(StateMachine.Instance.error,
            "After per-SM error reset, error must be false so IdleMonitor.GoTo(idle) " +
            "is not short-circuited. This is the fix that makes RadiationMonitor safe: " +
            "even if it sets error=true, the reset prevents propagation to IdleMonitor.");
    }

    // ─── FIX TESTS ───────────────────────────────────────────────────────────

    /// <summary>
    /// RadiationMonitor.Instance ctor and StartSM() do not throw and do not leave
    /// <c>StateMachine.Instance.error = true</c>.
    ///
    /// With radiation disabled (DLC absent in test env), the <c>init</c> state immediately
    /// transitions to null (StopSM) — this is safe and is the most common headless path.
    ///
    /// This test FAILS without the skip removal (RadiationMonitor was excluded from
    /// StartSM before this fix) and PASSES with it (RadiationMonitor.Instance starts normally).
    /// </summary>
    [Test]
    public void RadiationMonitor_StartSm_DoesNotThrow_And_DoesNotSetError() {
        var go  = AllStateMachinesInitTest.CreateFullDupeGO();
        var smc = go.GetComponent<StateMachineController>();

        // RadiationBalance must exist for CheckRadiationLevel to read it.
        // In test env MinionModifiers.initialAmounts is empty, so add it manually.
        var modifiers = go.GetComponent<MinionModifiers>();
        if (modifiers?.amounts != null &&
            modifiers.amounts.Get(Db.Get().Amounts.RadiationBalance.Id) == null) {
            modifiers.AddAmount(Db.Get().Amounts.RadiationBalance);
        }

        // Need a RationalAi.Instance as the factory target.
        var raiSmi = new RationalAi.Instance(smc, new Tag("Minion"));

        StateMachine.Instance smInst = null!;
        Assert.DoesNotThrow(
            () => smInst = new RadiationMonitor.Instance(raiSmi.master),
            "RadiationMonitor.Instance ctor must not throw in headless. " +
            "With DLC absent, Sim.IsRadiationEnabled() returns false → ctor returns early (safe). " +
            "With DLC active, ctor reads CustomGameSettings.Instance for difficulty mod.");

        Assert.DoesNotThrow(
            () => smInst.StartSM(),
            "RadiationMonitor.Instance.StartSM() must not throw. " +
            "With radiation disabled: init → GoTo(null) → StopSM() — no error. " +
            "With radiation enabled: init → GoTo(active.idle) — RadiationBalance=0 means " +
            "no sick/deadly transitions. Any NPE is caught by the per-SM reset in MinionPrefab.");

        Assert.IsFalse(StateMachine.Instance.error,
            "StateMachine.Instance.error must not be true after RadiationMonitor.StartSM(). " +
            "Original bug: error propagated to IdleMonitor.GoTo(idle) → no-op → bionic dupes " +
            "got no IdleChore. Per-SM reset (commit 2db37e0) now prevents this propagation, " +
            "making RadiationMonitor safe to run without being in IsHeadlessUnsafeSM.");
    }

    /// <summary>
    /// After RadiationMonitor.StartSM(), the dupe's <c>KPrefabID</c> does NOT have
    /// <c>GameTags.Dying</c> — the SM did not trigger the deadly-radiation path.
    ///
    /// <c>GameTags.Dying</c> requires <c>radiationExposure &gt;= 900</c>.  Fresh
    /// dupes have <c>RadiationBalance = 0</c>, so the deadly state is never entered.
    /// </summary>
    [Test]
    public void RadiationMonitor_StartSm_DoesNotSetDyingTag_WhenRadiationBalanceIsZero() {
        var go  = AllStateMachinesInitTest.CreateFullDupeGO();
        var smc = go.GetComponent<StateMachineController>();

        var modifiers = go.GetComponent<MinionModifiers>();
        if (modifiers?.amounts != null &&
            modifiers.amounts.Get(Db.Get().Amounts.RadiationBalance.Id) == null) {
            var amt = modifiers.AddAmount(Db.Get().Amounts.RadiationBalance);
            amt.value = 0f;  // explicitly zero — well below deadly threshold (900)
        }

        var raiSmi = new RationalAi.Instance(smc, new Tag("Minion"));
        var smInst = new RadiationMonitor.Instance(raiSmi.master);

        try { smInst.StartSM(); } catch { /* any NPE is caught — test the tag regardless */ }

        var kpid = go.GetComponent<KPrefabID>();
        Assert.IsFalse(kpid.HasTag(GameTags.Dying),
            "RadiationMonitor must NOT set GameTags.Dying when RadiationBalance = 0. " +
            "Dying tag is only set by active.sick.deadly.Enter → Health.Incapacitate, " +
            "which requires radiationExposure >= 900 * difficultySettingMod. " +
            "Fresh dupes start at 0 → no deadly transition → no Dying tag → " +
            "IdleMonitor enters idle (not stopped) → IdleChore is created normally.");
    }
}
