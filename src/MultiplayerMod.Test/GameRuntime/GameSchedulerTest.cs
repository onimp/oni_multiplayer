using NUnit.Framework;

namespace MultiplayerMod.Test.GameRuntime;

/// <summary>
/// Tests for GameScheduler initialization and tick behaviour in headless (DS) environment.
///
/// Root problem: IdleChore.Begin() calls GameScheduler.Instance.Schedule("IdleMove", ...)
/// to register a timed callback. In headless the Unity MonoBehaviour Update() loop never runs,
/// so GameScheduler.Update() (private void Update()) is never called → Scheduler never dequeues
/// entries → IdleMove callback never fires → Navigator.IsMoving stays False.
///
/// Fix (GameTickLoop.cs): call GameScheduler.Instance?.GetScheduler()?.Update() each frame,
/// mirroring the private Unity callback.
/// Fix (WorldBuilder.cs): defensive re-init guard if GameScheduler.Instance is null after
/// the Awake("GameScheduler", ...) call.
/// </summary>
public class GameSchedulerTest : PlayableGameTest {

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
        bool called = false;
        var handle = GameScheduler.Instance.Schedule(
            "test-idle-move", 0f, _ => called = true);

        Assert.IsNotNull(handle,
            "Schedule() must return a non-null SchedulerHandle — null means Scheduler was not initialised");

        // Clean up — cancel the pending entry so it doesn't interfere with later tests.
        handle.ClearScheduler();
    }

    /// <summary>
    /// Verifies that GetScheduler().Update() dequeues and fires scheduled callbacks.
    /// This mirrors what GameTickLoop now calls each frame to replace the private
    /// Unity MonoBehaviour Update() that never runs in headless.
    ///
    /// Scheduler.previousTime starts at float.NegativeInfinity, so the first Update()
    /// call always proceeds regardless of GameClock time.
    /// An entry scheduled at offset=0 fires when entry.time (= clock.GetTime() + 0) <= clock.GetTime().
    /// </summary>
    [Test]
    public void GameScheduler_GetSchedulerUpdate_FiresCallbackScheduledAtZeroOffset() {
        bool callbackFired = false;
        GameScheduler.Instance.Schedule(
            "test-idle-move-callback", 0f, _ => callbackFired = true);

        // Mirror of GameTickLoop: call the Scheduler's Update() directly.
        GameScheduler.Instance.GetScheduler().Update();

        Assert.IsTrue(callbackFired,
            "Scheduler.Update() must fire the callback scheduled at offset=0. " +
            "Without this, IdleMove callback never fires and dupes never move.");
    }
}
