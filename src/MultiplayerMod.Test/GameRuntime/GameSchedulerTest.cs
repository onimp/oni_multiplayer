using System;
using System.Reflection;
using NUnit.Framework;

namespace MultiplayerMod.Test.GameRuntime;

/// <summary>
/// Tests for GameScheduler initialization and tick behaviour in headless (DS) environment.
///
/// Root problem (NPE #1 — IdleMove never fires):
///   IdleChore.Begin() calls GameScheduler.Instance.Schedule("IdleMove", ...) to register
///   a timed callback. In headless the Unity MonoBehaviour Update() loop never runs, so
///   GameScheduler.Update() (private void Update()) is never called → Scheduler never
///   dequeues entries → IdleMove callback never fires → Navigator.IsMoving stays False.
///   Fix (GameTickLoop.cs): call GameScheduler.Instance?.GetScheduler()?.Update() each frame.
///
/// Root problem (NPE #2 — 15164 crashes in 90s):
///   StateMachineManager.Clear() stores and then calls scheduler.FreeResources() on the
///   SAME Scheduler object that GameScheduler.OnPrefabInit() passed to RegisterScheduler().
///   FreeResources() sets entries=null and clock=null on that Scheduler:
///     • Scheduler.Update(): Count => entries.Count → NPE every frame
///     • GameScheduler.Schedule(): clock.GetTime() in the private Schedule() overload → NPE
///       (e.g. ClothingWearer.OnSpawn calls Schedule("ApplySpawnClothes", 2f, ...))
///   Fix (WorldBuilder.cs): after StateMachineManager.Clear(), replace GameScheduler's
///   internal 'scheduler' field via reflection with a fresh Scheduler instance and
///   re-register it with StateMachineManager.
/// </summary>
public class GameSchedulerTest : PlayableGameTest {

    // BindingFlags.Public is required: AssemblyExposer rewrites private→public in the
    // exposed DLL used at runtime. NonPublic alone → GetField returns null → NPE.
    // See WorldBuilder line ~96: same pattern used for all other reflection field access.
    private static readonly FieldInfo SchedulerField =
        typeof(GameScheduler).GetField("scheduler",
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)!;

    // ──────────────────────────────────────────────────────────────────────────────
    // NPE #1: IdleMove never fires (GameTickLoop fix)
    // ──────────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// PlayableGameTest.SetUpUnityAndGame() adds GameScheduler via AddComponent and assigns
    /// Instance. Verifies the Instance is set so that IdleChore.Schedule() can proceed.
    /// </summary>
    [Test]
    public void GameScheduler_Instance_IsNotNull_AfterGameSetup() {
        Assert.IsNotNull(GameScheduler.Instance,
            "GameScheduler.Instance must not be null after PlayableGameTest setup — " +
            "IdleChore.Begin() calls Instance.Schedule() and NPEs if null");
    }

    /// <summary>
    /// GameScheduler.Schedule() must return a non-null SchedulerHandle.
    /// If the Scheduler is not initialized (e.g. OnPrefabInit never ran), GetScheduler()
    /// returns null and the call would throw NullReferenceException.
    /// </summary>
    [Test]
    public void GameScheduler_Schedule_ReturnsNonNullHandle() {
        var handle = GameScheduler.Instance.Schedule(
            "test-idle-move", 0f, _ => { });

        Assert.IsNotNull(handle,
            "Schedule() must return a non-null SchedulerHandle — null means Scheduler was not initialised");

        handle.ClearScheduler();
    }

    /// <summary>
    /// Verifies that GetScheduler().Update() dequeues and fires scheduled callbacks.
    /// This mirrors what GameTickLoop now calls each frame to replace the private
    /// Unity MonoBehaviour Update() that never runs in headless.
    /// Scheduler.previousTime starts at float.NegativeInfinity → first Update() always fires.
    /// </summary>
    [Test]
    public void GameScheduler_GetSchedulerUpdate_FiresCallbackScheduledAtZeroOffset() {
        bool callbackFired = false;
        GameScheduler.Instance.Schedule(
            "test-idle-move-callback", 0f, _ => callbackFired = true);

        GameScheduler.Instance.GetScheduler().Update();

        Assert.IsTrue(callbackFired,
            "Scheduler.Update() must fire the callback scheduled at offset=0. " +
            "Without this, IdleMove callback never fires and dupes never move.");
    }

    // ──────────────────────────────────────────────────────────────────────────────
    // NPE #2: StateMachineManager.Clear() destroys shared Scheduler (WorldBuilder fix)
    // ──────────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Regression guard for [0x00908] NPE: BindingFlags.NonPublic alone misses the
    /// 'scheduler' field when AssemblyExposer has rewritten it to public. The fix adds
    /// BindingFlags.Public so the field is found regardless of visibility.
    /// This test would fail (SchedulerField == null) with the broken NonPublic-only flags.
    /// </summary>
    [Test]
    public void GameScheduler_SchedulerFieldReflection_ReturnsNonNull() {
        Assert.IsNotNull(SchedulerField,
            "GetField('scheduler', Public|NonPublic|Instance) must return non-null. " +
            "If null → SetValue NPEs at runtime (regression from 8641d9b): " +
            "BindingFlags.NonPublic alone misses the field when AssemblyExposer made it public.");

        var schedulerValue = SchedulerField.GetValue(GameScheduler.Instance);
        Assert.IsNotNull(schedulerValue,
            "GameScheduler.scheduler field value must not be null — field initializer must have run");
    }

    /// <summary>
    /// Verifies that FreeResources() on the Scheduler makes Update() throw — this is the
    /// exact failure state that StateMachineManager.Clear() causes in headless:
    ///   StateMachineManager.RegisterScheduler(scheduler) stores GameScheduler's Scheduler.
    ///   StateMachineManager.Clear() → scheduler.FreeResources() → entries=null, clock=null.
    ///   Next call to Scheduler.Update() → entries.Count → NullReferenceException.
    /// </summary>
    [Test]
    public void GameScheduler_AfterFreeResources_UpdateThrows() {
        var scheduler = GameScheduler.Instance.GetScheduler();
        Assert.IsNotNull(scheduler, "GetScheduler() must return non-null before the test");

        // Enqueue one entry so Update() gets past the early-exit 'if (Count == 0) return'.
        GameScheduler.Instance.Schedule("probe", 0f, _ => { });

        // Simulate StateMachineManager.Clear():
        scheduler.FreeResources(); // sets entries=null, clock=null

        Assert.Throws<NullReferenceException>(
            () => scheduler.Update(),
            "Scheduler.Update() must throw after FreeResources() — documents the broken state " +
            "that StateMachineManager.Clear() creates for GameScheduler's Scheduler.");

        // Restore GameScheduler to a working state so TearDown doesn't encounter leftovers.
        var fresh = new Scheduler(new GameScheduler.GameSchedulerClock());
        SchedulerField.SetValue(GameScheduler.Instance, fresh);
    }

    /// <summary>
    /// Verifies the WorldBuilder fix: after StateMachineManager.Clear() destroys the shared
    /// Scheduler, replacing it via reflection with a fresh instance restores all operations:
    ///   • GetScheduler().Update() does not throw
    ///   • Schedule() returns a valid handle
    ///   • Update() fires the scheduled callback
    /// This is exactly what WorldBuilder does after StateMachineManager.Instance.Clear().
    /// </summary>
    [Test]
    public void GameScheduler_AfterFreeResources_ReplacingSchedulerViaReflection_RestoresAll() {
        // Simulate StateMachineManager.Clear() destroying the scheduler.
        var old = GameScheduler.Instance.GetScheduler();
        old?.FreeResources();

        // Apply the WorldBuilder fix: replace with a fresh Scheduler.
        var freshScheduler = new Scheduler(new GameScheduler.GameSchedulerClock());
        SchedulerField.SetValue(GameScheduler.Instance, freshScheduler);
        // (In WorldBuilder, StateMachineManager.Instance.RegisterScheduler(freshScheduler) also runs.)

        // After fix: Update() must not throw.
        Assert.DoesNotThrow(
            () => GameScheduler.Instance.GetScheduler().Update(),
            "Scheduler.Update() must not throw after reflection-based scheduler replacement");

        // After fix: Schedule() must return a valid handle.
        bool fired = false;
        var handle = GameScheduler.Instance.Schedule("post-fix-probe", 0f, _ => fired = true);
        Assert.IsNotNull(handle, "Schedule() must return non-null after fix");

        // After fix: Update() must fire the scheduled callback.
        GameScheduler.Instance.GetScheduler().Update();
        Assert.IsTrue(fired, "Callback must fire after fix — verifies clock and entries are functional");

        handle.ClearScheduler();
    }

    // ──────────────────────────────────────────────────────────────────────────────
    // NPE #3: GameSchedulerClock frozen (IdleMove never fires)
    // ──────────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Verifies that GameClock.GetTime() starts at its OnPrefabInit seed value (50f).
    /// Documents the pre-fix state: without Sim33ms being called, GetTime() never
    /// advances from 50f → IdleMove (scheduled at 50+Random(5,15)) never triggers.
    /// </summary>
    [Test]
    public void GameClock_GetTime_StartsAt50f_OnPrefabInitSeed() {
        // GameClock.OnPrefabInit() sets timeSinceStartOfCycle = 50f, cycle = 0.
        // GetTime() = timeSinceStartOfCycle + cycle * 600f = 50f.
        Assert.That(GameClock.Instance.GetTime(), Is.EqualTo(50f),
            "GameClock.GetTime() must return 50f after OnPrefabInit " +
            "(timeSinceStartOfCycle=50, cycle=0). Documents the frozen-clock pre-fix state.");
    }

    /// <summary>
    /// Verifies that GameClock.Sim33ms(dt) advances GetTime() — the fix calls this
    /// each subtick in GameTickLoop to mirror SimAndRenderScheduler.sim33ms bucket.
    ///
    /// Root cause: SimAndRenderScheduler.sim33ms bucket (which calls GameClock.Sim33ms)
    /// is never driven in headless. GetTime() stays frozen at 50f. IdleMove is scheduled
    /// at GetTime()+Random(5,15)=55-65f. Scheduler.Update() condition (time>=entry.time)
    /// is never true → IdleMove never fires → dupe never moves.
    ///
    /// Fix (GameTickLoop.cs): call GameClock.Instance?.Sim33ms(SubTickTime) each subtick.
    /// </summary>
    [Test]
    public void GameClock_Sim33ms_AdvancesGetTime() {
        var before = GameClock.Instance.GetTime();

        // Simulate 60 subticks worth of clock advancement (≈ 1 second of game time).
        const float subTickTime = 1f / 60f;
        for (int i = 0; i < 60; i++)
            GameClock.Instance.Sim33ms(subTickTime);

        var after = GameClock.Instance.GetTime();

        Assert.That(after, Is.GreaterThan(before),
            "GameClock.GetTime() must increase after Sim33ms calls. " +
            "Without this, IdleMove scheduler deadline is never reached.");
        Assert.That(after, Is.EqualTo(before + 1f).Within(0.001f),
            "60 subticks × (1/60)s = 1s total clock advance expected");
    }

    /// <summary>
    /// Verifies the full IdleMove trigger chain: after enough Sim33ms calls to advance
    /// the clock past the scheduled time, Scheduler.Update() fires the callback.
    ///
    /// IdleMove delay = Random(5,15) seconds. We advance 20 seconds to be safe.
    /// GameSchedulerClock.GetTime() → GameClock.Instance.GetTime() → advances with Sim33ms.
    /// </summary>
    [Test]
    public void GameSchedulerClock_AfterSim33msAdvance_SchedulerFiresDelayedCallback() {
        // Schedule at a 2-second delay (deterministic, smaller than Random(5,15)).
        const float delay = 2f;
        bool fired = false;
        var handle = GameScheduler.Instance.Schedule("idle-move-sim", delay, _ => fired = true);
        Assert.IsNotNull(handle);

        // Advance clock by 3 seconds (> 2s delay) via the same path GameTickLoop uses.
        const float subTickTime = 1f / 60f;
        for (int i = 0; i < 180; i++) {           // 180 × (1/60) = 3.0 s
            GameClock.Instance.Sim33ms(subTickTime);
            GameScheduler.Instance.GetScheduler().Update();
        }

        Assert.IsTrue(fired,
            "Callback scheduled at +2s must fire after 3s of Sim33ms + Scheduler.Update() calls. " +
            "This is the IdleMove trigger chain — if this fails, dupes never move in headless.");

        if (!fired) handle.ClearScheduler();
    }
}
