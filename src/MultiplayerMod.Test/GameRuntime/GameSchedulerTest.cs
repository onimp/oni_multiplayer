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

    private static readonly FieldInfo SchedulerField =
        typeof(GameScheduler).GetField("scheduler", BindingFlags.NonPublic | BindingFlags.Instance)!;

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
}

