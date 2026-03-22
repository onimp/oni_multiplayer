using NUnit.Framework;
using UnityEngine;

namespace MultiplayerMod.Test.GameRuntime;

/// <summary>
/// Tests for DS-005 direct FindNextChore fix.
///
/// ForceUpdateBrains replaced brain.UpdateBrain() with direct consumer.FindNextChore() + driver.SetChore().
/// Motivation: Brain.UpdateBrain() calls IsRunning() first — if false (brain not fully started via
/// OnSpawn in the fallback path) it returns immediately without assigning any chore.
/// Direct FindNextChore bypasses that guard entirely.
/// </summary>
public class FindNextChoreTest : PlayableGameTest {

    /// <summary>
    /// Baseline: Brain.IsRunning() returns false before OnSpawn (running field not set yet).
    /// This is the condition that makes brain.UpdateBrain() a no-op in the fallback path.
    /// </summary>
    [Test]
    public void Brain_IsRunning_IsFalse_BeforeOnSpawn() {
        var go = createGameObject();
        var brain = go.AddComponent<Brain>();

        Assert.IsFalse(brain.IsRunning(),
            "Brain.IsRunning() must be false before OnSpawn sets running=true");
    }

    /// <summary>
    /// ChoreConsumer.consumerState is null before OnSpawn runs.
    /// FindNextChore calls consumerState.Refresh() internally — requires OnSpawn to have completed.
    /// In the fallback path ForceUpdateBrains only calls FindNextChore on real game dupes
    /// whose ChoreConsumer.OnSpawn() has already run (only RationalAi's OnSpawn failed).
    /// This test documents the pre-condition: consumerState must be non-null before FindNextChore.
    /// </summary>
    [Test]
    public void ChoreConsumer_ConsumerState_IsNull_BeforeOnSpawn() {
        var go = createGameObject();
        var consumer = go.AddComponent<ChoreConsumer>();

        // consumerState is assigned in OnSpawn() only — must be null before that fires
        Assert.IsNull(consumer.consumerState,
            "consumerState must be null before ChoreConsumer.OnSpawn() runs — " +
            "FindNextChore requires consumerState to be initialized before being called");
    }

    /// <summary>
    /// UpdateBrain skips chore assignment when brain is not running (IsRunning() == false).
    /// This is the root cause of the DS-005 chore stall — the fix bypasses UpdateBrain entirely.
    /// </summary>
    [Test]
    public void UpdateBrain_IsNoOp_WhenBrainNotRunning() {
        var go = createGameObject();
        go.AddComponent<ChoreConsumer>();
        var driver = go.AddComponent<ChoreDriver>();
        driver.smi.StartSM(); // ensure SM is running so SetChore could work if called
        var brain = go.AddComponent<Brain>();

        // Brain is not running — UpdateBrain should be a no-op (does not throw, does not assign chore)
        Assert.IsFalse(brain.IsRunning());
        Assert.DoesNotThrow(
            () => brain.UpdateBrain(),
            "UpdateBrain must not throw even when brain is not running");

        // Chore is still null (no-op confirmed)
        Assert.IsNull(driver.GetCurrentChore(),
            "No chore should be assigned after UpdateBrain no-op when brain is not running");
    }
}
