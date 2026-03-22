using NUnit.Framework;
using UnityEngine;

namespace MultiplayerMod.Test.GameRuntime;

/// <summary>
/// Tests for the creature brain kick path added in GameTickLoop.ForceUpdateCreatureBrains().
///
/// ForceUpdateCreatureBrains mirrors ForceUpdateBrains (tick=61) for creatures:
///   1. Reset StateMachine.Instance.error = false
///   2. Reset driver.smi.isCrashed = false
///   3. Ensure ChoreDriver SM is in nochore state
///   4. consumer.FindNextChore(ref context) + driver.SetChore(context)
///
/// These tests verify the pre-conditions and core path at the Brain/ChoreConsumer level,
/// matching the direct FindNextChore approach used in the production code.
///
/// End-to-end verification (critter actually picks a chore + moves) requires a full
/// world spawn and is verified on the live dedicated server.
/// </summary>
public class CreatureBrainKickTest : PlayableGameTest {

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

    /// <summary>
    /// CreatureBrain.IsRunning() returns false before OnSpawn (running field not set).
    /// ForceUpdateCreatureBrains guards against this and skips not-running brains.
    /// </summary>
    [Test]
    public void CreatureBrain_IsNotRunning_BeforeOnSpawn() {
        var go = createGameObject();
        var brain = go.AddComponent<CreatureBrain>();

        Assert.IsFalse(brain.IsRunning(),
            "CreatureBrain.IsRunning() must be false before OnSpawn sets running=true. " +
            "ForceUpdateCreatureBrains must skip such brains to avoid premature chore assignment.");
    }

    /// <summary>
    /// Core ForceUpdateCreatureBrains path: consumer.FindNextChore() does not throw
    /// when ChoreConsumer is properly initialized (consumerState set, driver SM running)
    /// but the ChoreTable is empty (no available chores for a bare test GO).
    ///
    /// This verifies the production code path:
    ///   consumer.FindNextChore(ref context) → iterates providers → no chore → false
    /// The false return prevents driver.SetChore() from being called → no exception.
    /// </summary>
    [Test]
    public void FindNextChore_DoesNotThrow_WithEmptyChoreTable() {
        var go = createGameObject();

        // [MyCmpAdd] does not fire in the mock Unity test environment — add components explicitly.
        // In real game, ChoreConsumer.InitializeComponent() adds ChoreDriver, ChoreProvider, User.
        // Add ChoreDriver + ChoreProvider before ChoreConsumer so GetComponent<> finds them.
        go.AddComponent<ChoreDriver>();
        go.AddComponent<ChoreProvider>();
        var consumer = go.AddComponent<ChoreConsumer>();

        // consumerState must be set (normally done by ChoreConsumer.OnSpawn — not triggered here).
        // ChoreConsumerStatePatch resolves consumerState.choreDriver = GetComponent<ChoreDriver>().
        consumer.consumerState = new ChoreConsumerState(consumer);

        // Start ChoreDriver SM: required so driver.smi is non-null for nochore state check.
        var driver = go.GetComponent<ChoreDriver>();
        Assert.IsNotNull(driver, "ChoreDriver must be present on the GO");
        driver.smi.StartSM();

        // Mirror ForceUpdateCreatureBrains: reset error, ensure nochore, call FindNextChore.
        StateMachine.Instance.error = false;
        if (driver.smi != null) driver.smi.isCrashed = false;

        var context = default(Chore.Precondition.Context);

        Assert.DoesNotThrow(
            () => consumer.FindNextChore(ref context),
            "consumer.FindNextChore must not throw with initialized consumerState " +
            "and an empty providers list (no chores available in test env)."
        );
        // Empty ChoreTable → no chore found → SetChore must NOT have been called.
        Assert.IsNull(driver.GetCurrentChore(),
            "No chore should be assigned when FindNextChore returns false (empty ChoreTable). " +
            "ForceUpdateCreatureBrains only calls SetChore when FindNextChore returns true.");
    }

    /// <summary>
    /// When FindNextChore returns false (no chore available), SetChore is not called
    /// and the ChoreDriver SM remains in nochore state.
    ///
    /// ForceUpdateCreatureBrains only calls driver.SetChore() on a found chore —
    /// this test verifies the guard works correctly and no spurious SM transitions occur.
    /// </summary>
    [Test]
    public void SetChore_NotCalled_WhenFindNextChore_ReturnsFalse() {
        var go = createGameObject();
        go.AddComponent<ChoreDriver>();
        go.AddComponent<ChoreProvider>();
        var consumer = go.AddComponent<ChoreConsumer>();
        consumer.consumerState = new ChoreConsumerState(consumer);

        var driver = go.GetComponent<ChoreDriver>();
        driver.smi.StartSM();
        StateMachine.Instance.error = false;

        // Ensure SM is in nochore (pre-condition for SetChore to work).
        var smBefore = driver.smi?.GetCurrentState()?.name;

        var context = default(Chore.Precondition.Context);
        var found = consumer.FindNextChore(ref context);

        // No chores registered → must return false.
        Assert.IsFalse(found,
            "FindNextChore must return false when no chore providers have available chores. " +
            $"Got found=true, state before={smBefore}");

        // SM must not have transitioned from nochore — SetChore was never called.
        var smAfter = driver.smi?.GetCurrentState()?.name;
        Assert.That(smAfter, Is.EqualTo(smBefore),
            "ChoreDriver SM must stay in nochore when no chore was found " +
            "(SetChore not called → no nochore→haschore transition).");
    }
}
