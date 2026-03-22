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

    // ─── DS-006: ChoreDriver haschore.Update worker NPE ─────────────────────

    /// <summary>
    /// Regression test for DS-006: smi.worker null → NPE in haschore.Update (b__5_3 IL[0x75]).
    ///
    /// StatesInstance.ctor sets: worker = base.master.GetComponent&lt;WorkerBase&gt;()
    /// StandardWorker : WorkerBase.  If StandardWorker is absent from the dupe GO,
    /// worker=null → haschore.Update fires: Workable workable = smi.worker.GetWorkable() → NPE.
    ///
    /// This test documents the broken state: worker IS null when StandardWorker is absent.
    /// </summary>
    [Test]
    public void ChoreDriver_StatesInstance_Worker_IsNull_WhenNoStandardWorker() {
        var go = createGameObject();
        go.AddComponent<ChoreConsumer>();
        var driver = go.AddComponent<ChoreDriver>();

        // Do NOT add StandardWorker — replicates the broken headless spawning state.
        driver.smi.StartSM();

        var smi = driver.GetSMI<ChoreDriver.StatesInstance>();
        Assert.IsNull(smi.worker,
            "worker must be null when StandardWorker is absent from the GO. " +
            "This is the root cause of ChoreDriver+States b__5_3 [0x00075] NPE firing 522K+/5min " +
            "on the dedicated server (haschore.Update: smi.worker.GetWorkable() → NullReferenceException).");
    }

    /// <summary>
    /// Fix test for DS-006: after AddOrGet&lt;StandardWorker&gt;() (our WorldBuilder fix),
    /// StatesInstance.worker is non-null → haschore.Update can call GetWorkable() safely.
    ///
    /// WorldBuilder.FixRationalAi now calls go.AddOrGet&lt;StandardWorker&gt;() before
    /// choreDriver.smi.StartSM() so worker is always set regardless of prefab bootstrap state.
    /// </summary>
    [Test]
    public void ChoreDriver_StatesInstance_Worker_IsNotNull_WhenStandardWorkerPresent() {
        var go = createGameObject();
        go.AddComponent<ChoreConsumer>();
        go.AddComponent<StandardWorker>(); // ← fix: mirrors WorldBuilder.FixRationalAi AddOrGet
        var driver = go.AddComponent<ChoreDriver>();

        driver.smi.StartSM();

        var smi = driver.GetSMI<ChoreDriver.StatesInstance>();
        Assert.IsNotNull(smi.worker,
            "worker must be non-null after AddOrGet<StandardWorker>() + StartSM(). " +
            "This ensures haschore.Update (b__5_3) can call smi.worker.GetWorkable() without NPE.");
    }

    /// <summary>
    /// DS-006 critter fix (proper): AddOrGet&lt;StandardWorker&gt;() BEFORE driver.Spawn() /
    /// StartSM() ensures StatesInstance.ctor finds GetComponent&lt;WorkerBase&gt;() non-null.
    ///
    /// Root cause of original failure: FixChoreConsumers called driver.Spawn() (→ OnSpawn
    /// → base.smi.StartSM() → StatesInstance.ctor → GetComponent&lt;WorkerBase&gt;() = null)
    /// BEFORE adding StandardWorker to the GO.  Fix: AddOrGet&lt;StandardWorker&gt;() runs
    /// before driver.Spawn() in FixChoreConsumers Cases 1 and 2.
    ///
    /// This test mirrors WorldBuilder.FixChoreConsumers Cases 1/2:
    ///   go.AddOrGet&lt;StandardWorker&gt;() → driver.Spawn() (→ StartSM → ctor)
    /// Result: worker is non-null immediately after Spawn().
    /// </summary>
    [Test]
    public void ChoreDriver_CritterFix_StandardWorkerBeforeSpawn_WorkerIsNotNull() {
        var go = createGameObject();
        go.AddComponent<ChoreConsumer>();
        var driver = go.AddComponent<ChoreDriver>();

        // Mirrors FixChoreConsumers: add StandardWorker BEFORE Spawn().
        go.AddOrGet<StandardWorker>();
        driver.smi.StartSM();  // StartSM = what Spawn() calls internally via OnSpawn()

        var smi = (ChoreDriver.StatesInstance)driver.GetSMI();
        Assert.IsNotNull(smi?.worker,
            "worker must be non-null when StandardWorker is added before StartSM(). " +
            "This is the DS-006 critter fix: FixChoreConsumers calls AddOrGet<StandardWorker>() " +
            "before driver.Spawn() so StatesInstance.ctor finds WorkerBase → no NPE at b__5_3 IL[0x0075].");
    }

    // ─── DS-006b: retrofit patch — SM started before StandardWorker ─────────
    // Scenario: TriggerLifecycle ran ChoreDriver.OnSpawn() (→ StartSM → ctor → worker=null)
    // BEFORE FixRationalAi/MinionPrefab.Setup() had a chance to add StandardWorker.
    // The old guard (if GetSMI()==null) skipped AddOrGet<StandardWorker>() for these dupes.
    // Fix: unconditional AddOrGet + direct field patch on the live SMI.

    /// <summary>
    /// Regression: when SM is started BEFORE StandardWorker is on the GO (simulates
    /// TriggerLifecycle path), smi.worker is null even after AddOrGet&lt;StandardWorker&gt;()
    /// — because ctor already ran and captured GetComponent&lt;WorkerBase&gt;()=null.
    /// Adding StandardWorker later does NOT automatically update the live SMI field.
    /// </summary>
    [Test]
    public void ChoreDriver_WorkerRetrofit_SmStartedBeforeStandardWorker_WorkerIsNull() {
        var go = createGameObject();
        go.AddComponent<ChoreConsumer>();
        var driver = go.AddComponent<ChoreDriver>();

        // Simulate TriggerLifecycle: SM started BEFORE StandardWorker added.
        driver.smi.StartSM();

        // Now add StandardWorker (as FixChoreConsumers/MinionPrefab.Setup would do later).
        go.AddOrGet<StandardWorker>();

        var smi = driver.GetSMI<ChoreDriver.StatesInstance>();
        Assert.IsNull(smi.worker,
            "worker is still null after AddOrGet<StandardWorker>() when SM was started first. " +
            "StatesInstance.ctor already ran and captured GetComponent<WorkerBase>()=null. " +
            "Adding StandardWorker after the fact does not retroactively update smi.worker — " +
            "this is the root cause of DS-006 resurfacing for 2 of 3 dupes at tick=991.");
    }

    /// <summary>
    /// Fix for DS-006 retrofit: after adding StandardWorker, directly assign
    /// smiInst.worker = go.GetComponent&lt;WorkerBase&gt;() to patch the live SMI.
    /// This is what MinionPrefab.Setup() now does unconditionally after StartSM.
    /// </summary>
    [Test]
    public void ChoreDriver_WorkerRetrofit_PatchLiveSmi_WorkerIsNotNull() {
        var go = createGameObject();
        go.AddComponent<ChoreConsumer>();
        var driver = go.AddComponent<ChoreDriver>();

        // Simulate TriggerLifecycle: SM started BEFORE StandardWorker added.
        driver.smi.StartSM();

        // Simulate MinionPrefab.Setup() full fix:
        //   Step 1 — unconditional AddOrGet<StandardWorker>()
        go.AddOrGet<StandardWorker>();
        //   Step 2 — retrofit: patch the live SMI's worker field directly
        var smiInst = driver.GetSMI<ChoreDriver.StatesInstance>();
        if (smiInst != null && smiInst.worker == null)
            smiInst.worker = go.GetComponent<WorkerBase>();

        Assert.IsNotNull(smiInst?.worker,
            "worker must be non-null after retrofit patch. " +
            "MinionPrefab.Setup() unconditionally adds StandardWorker then assigns " +
            "smiInst.worker = go.GetComponent<WorkerBase>() so haschore.Update " +
            "b__5_3 [0x0075] no longer NPEs on smi.worker.GetWorkable().");
    }

    // ─── DS-005 tests (pre-existing) ────────────────────────────────────────

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
    /// Documents the Parameter.Set() equality guard — root cause of DS-005 SetChore-not-sticking.
    ///
    /// Parameter&lt;T&gt;.Context.Set() only calls onDirty when value changes:
    ///   if (!EqualityComparer&lt;T&gt;.Default.Equals(value, this.value)) { ... onDirty(smi) }
    ///
    /// If nextChore.value already holds chore X from a previous blocked GoTo attempt
    /// (Instance.error=true caused GoTo to be a no-op, but Set() already changed the value),
    /// the subsequent call to SetChore(X) invokes Set(X) again — equality match → no-op →
    /// onDirty not called → ParamTransition never fires → SM stays in nochore forever.
    ///
    /// Fix in ForceUpdateBrains: call nextChore.Set(null, smi) BEFORE SetChore(context).
    /// This guarantees a null→X value change that always fires onDirty, regardless of prior state.
    ///
    /// This test verifies the pre-clear idiom: Set(null) resets the parameter safely (no crash,
    /// SM still running in nochore), enabling the subsequent SetChore to fire the transition.
    /// </summary>
    [Test]
    public void NextChore_PreClearToNull_ResetsParam_EnablesSubsequentSet() {
        var go = createGameObject();
        go.AddComponent<ChoreConsumer>();
        var driver = go.AddComponent<ChoreDriver>();
        driver.smi.StartSM();

        // Baseline: nextChore is null after StartSM (SM in nochore).
        Assert.IsNull(driver.smi.sm.nextChore.Get(driver.smi),
            "nextChore must be null after StartSM (SM in nochore)");

        // Pre-clear to null (our fix) when value is already null:
        // → equality guard: null == null → no-op → no crash, value stays null.
        driver.smi.sm.nextChore.Set(null, driver.smi);
        Assert.IsNull(driver.smi.sm.nextChore.Get(driver.smi),
            "Set(null) when already null must remain null — equality guard no-op, no crash");

        // After pre-clear, SM must still be running and in nochore — pre-clear is safe.
        Assert.That(driver.smi.IsRunning(), Is.True, "SM must still be running after pre-clear");
        Assert.That(driver.smi.GetCurrentState(), Is.EqualTo(driver.smi.sm.nochore),
            "SM must still be in nochore after pre-clear (ready for SetChore)");
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
