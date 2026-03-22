using NUnit.Framework;
using UnityEngine;

namespace MultiplayerMod.Test.GameRuntime;

/// <summary>
/// Tests for DS-005 ChoreDriver SM bootstrap fix.
///
/// ChoreDriver extends StateMachineComponent&lt;StatesInstance&gt;.
/// OnSpawn() calls base.smi.StartSM() — but in the FixRationalAi fallback path
/// OnSpawn never runs (BaseOnSpawn failed) → SM not started → GetSMI() == null
/// → ChoreDriver.SetChore() calls smi.sm.nextChore.Set() → NPE → chore never assigned.
///
/// Fix: in the fallback path, start ChoreDriver SM explicitly:
///   choreDriver.smi.StartSM()   (mirrors what ChoreDriver.OnSpawn() does)
/// </summary>
public class ChoreDriverSmTest : PlayableGameTest {

    /// <summary>
    /// Baseline: freshly added ChoreDriver has no SMI (GetSMI() == null).
    /// Verifies the broken state that the fix addresses.
    /// </summary>
    [Test]
    public void ChoreDriver_BeforeStart_GetSMI_IsNull() {
        var go = createGameObject();
        go.AddComponent<ChoreConsumer>(); // required by StatesInstance ctor
        var driver = go.AddComponent<ChoreDriver>();

        Assert.IsNull(driver.GetSMI(),
            "ChoreDriver.GetSMI() must be null before StartSM is called (SM not yet started)");
    }

    /// <summary>
    /// After calling smi.StartSM() (our fix), GetSMI() returns non-null —
    /// the SM is running and SetChore() can transition nochore→haschore.
    /// </summary>
    [Test]
    public void ChoreDriver_AfterSmStart_GetSMI_IsNotNull() {
        var go = createGameObject();
        go.AddComponent<ChoreConsumer>(); // required by StatesInstance ctor
        var driver = go.AddComponent<ChoreDriver>();

        // Mirrors what ChoreDriver.OnSpawn() and our fallback fix both do.
        driver.smi.StartSM();

        Assert.IsNotNull(driver.GetSMI(),
            "ChoreDriver.GetSMI() must not be null after smi.StartSM()");
    }

    /// <summary>
    /// Guard-then-start idiom: if GetSMI() != null (BaseOnSpawn succeeded and
    /// OnSpawn already ran), calling StartSM again is skipped — no double-start.
    /// </summary>
    [Test]
    public void ChoreDriver_GuardedStart_SkipsIfAlreadyRunning() {
        var go = createGameObject();
        go.AddComponent<ChoreConsumer>();
        var driver = go.AddComponent<ChoreDriver>();

        driver.smi.StartSM(); // first start
        var smiAfterFirst = driver.GetSMI();

        // Second guarded start — must be a no-op (GetSMI() != null → skip)
        if (driver.GetSMI() == null)
            driver.smi.StartSM();

        Assert.AreSame(smiAfterFirst, driver.GetSMI(),
            "SMI reference must be identical after a guarded (no-op) second start");
    }

    /// <summary>
    /// Root cause regression test for DS-005 SetChore-not-sticking.
    ///
    /// StateMachine.Instance.error is a global static bool. Any GoTo() failure during
    /// dupe spawn (e.g. PlayAnim with null animController in headless) sets it true,
    /// making ALL subsequent GoTo() calls silent no-ops:
    ///   if (App.IsExiting || Instance.error || ...) return;
    ///
    /// Fix in ForceUpdateBrains: reset Instance.error=false per-brain before FindNextChore.
    /// This allows ChoreDriver's nochore→haschore ParamTransition in SetChore to proceed.
    ///
    /// Test: with error=true, GoTo(null) that normally stops SM is a no-op (SM stays running).
    ///       After resetting error=false, GoTo(null) works normally (SM stops).
    /// </summary>
    [Test]
    public void StateMachineErrorFlag_WhenTrue_BlocksGoTo_ResettingItRestoresBehavior() {
        var saved = StateMachine.Instance.error;
        try {
            var go = createGameObject();
            go.AddComponent<ChoreConsumer>();
            var driver = go.AddComponent<ChoreDriver>();
            driver.smi.StartSM();
            Assert.That(driver.smi.IsRunning(), Is.True, "SM must be running after StartSM");

            // Simulate Instance.error=true as set by GoTo failures during headless spawn.
            // StopSM also checks: if (Instance.error) return; — so it becomes a no-op too.
            StateMachine.Instance.error = true;
            driver.smi.StopSM("test-error-flag");
            Assert.That(driver.smi.IsRunning(), Is.True,
                "StopSM (and GoTo) must be a no-op when Instance.error=true");

            // After resetting the flag (our fix), StopSM works normally and stops the SM.
            StateMachine.Instance.error = false;
            driver.smi.StopSM("test-after-reset");
            Assert.That(driver.smi.IsRunning(), Is.False,
                "StopSM must work normally when Instance.error=false");
        } finally {
            StateMachine.Instance.error = saved;
        }
    }

    /// <summary>
    /// Verifies the distinction between the global static Instance.error and the
    /// per-instance isCrashed field on StateMachine.Instance.
    ///
    /// Error() sets BOTH: Instance.error (static) AND this.isCrashed (per-instance).
    /// The GoTo guard only checks Instance.error (static), not isCrashed.
    /// ForceUpdateBrains resets both per-brain for a clean SetChore attempt.
    ///
    /// Test: isCrashed=true alone (with Instance.error=false) does NOT block StopSM/GoTo.
    ///       This confirms isCrashed is informational only — the real GoTo guard is Instance.error.
    /// </summary>
    [Test]
    public void StateMachinePerInstanceCrashed_DoesNotBlockGoTo_OnlyGlobalErrorDoes() {
        var savedError = StateMachine.Instance.error;
        try {
            var go = createGameObject();
            go.AddComponent<ChoreConsumer>();
            var driver = go.AddComponent<ChoreDriver>();
            driver.smi.StartSM();
            Assert.That(driver.smi.IsRunning(), Is.True);

            // Set per-instance isCrashed=true but leave global error=false.
            // isCrashed is NOT in GoTo guard, so SM operations must still work.
            driver.smi.isCrashed = true;
            StateMachine.Instance.error = false;

            driver.smi.StopSM("test-isCrashed-not-blocking");
            Assert.That(driver.smi.IsRunning(), Is.False,
                "isCrashed=true must NOT block StopSM — guard only checks global Instance.error");

            // Resetting isCrashed is safe and required alongside Instance.error reset in
            // ForceUpdateBrains to ensure a fully clean SM state for SetChore.
            driver.smi.isCrashed = false;
            Assert.That(driver.smi.isCrashed, Is.False, "isCrashed must be settable to false");
        } finally {
            StateMachine.Instance.error = savedError;
        }
    }
}
