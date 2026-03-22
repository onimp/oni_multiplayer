using System.Runtime.Serialization;
using NUnit.Framework;
using UnityEngine;

namespace MultiplayerMod.Test.GameRuntime;

/// <summary>
/// Tests for DS-005 SetChore sticking fix.
///
/// ChoreDriver.SetChore() internally calls context.chore.IsValid() before setting nextChore.
/// StandardChoreBase.IsValid() checks: provider != null AND gameObject.GetMyWorldId() != -1.
/// In headless mode GetMyWorldId() may return -1 (Grid.WorldIdx not populated for the dupe's cell)
/// → IsValid() returns false → SetChore exits without setting nextChore
/// → no nochore→haschore SM transition → GetCurrentChore() stays null.
///
/// Fix (ForceUpdateBrains): check IsValid() before calling SetChore.
/// If false → bypass by setting driver.context + driver.smi.sm.nextChore directly,
/// which triggers the ParamTransition(nextChore, haschore) synchronously
/// → BeginChore() → currentChore.Set() → GetCurrentChore() returns the chore.
/// </summary>
public class SetChoreBypassTest : PlayableGameTest {

    /// <summary>
    /// Baseline: StandardChoreBase.IsValid() returns false when provider is null.
    /// This is the condition that makes SetChore() silently exit without assigning the chore.
    ///
    /// Note: IdleChore.StatesInstance NPEs in test env (requires Navigator/KBatchedAnimController).
    /// We use FormatterServices.GetUninitializedObject to bypass the constructor entirely —
    /// the resulting instance has provider=null (default for reference fields), which is exactly
    /// the IsValid()=false condition we want to exercise.
    /// </summary>
    [Test]
    public void IsValid_ReturnsFalse_WhenProviderIsNull() {
        // Create chore without running the ctor (avoids IdleChore.StatesInstance NPE in test env).
        var idleChore = (IdleChore)FormatterServices.GetUninitializedObject(typeof(IdleChore));

        // provider is null (uninitialized default) — IsValid() must return false
        Assert.IsNull(idleChore.provider, "provider must be null after GetUninitializedObject");
        Assert.IsFalse(idleChore.IsValid(),
            "IsValid() must return false when provider is null — SetChore will skip assignment");
    }

    /// <summary>
    /// SetChore() is a no-op when chore.IsValid() returns false.
    /// GetCurrentChore() stays null — the chore is never assigned.
    /// This is the root cause of DS-005 chore stall in headless mode.
    /// </summary>
    [Test]
    public void SetChore_DoesNotAssign_WhenChoreIsInvalid() {
        var go = createGameObject();
        go.AddComponent<ChoreConsumer>();
        var driver = go.AddComponent<ChoreDriver>();
        driver.smi.StartSM();

        // Create an invalid chore (provider=null → IsValid()=false) without running ctor.
        var invalidChore = (IdleChore)FormatterServices.GetUninitializedObject(typeof(IdleChore));
        Assert.IsFalse(invalidChore.IsValid(), "pre-condition: chore must be invalid");

        var ctx = new Chore.Precondition.Context { chore = invalidChore };

        // SetChore's IsValid guard silently drops the assignment — must not throw
        Assert.DoesNotThrow(() => driver.SetChore(ctx),
            "SetChore must not throw even when IsValid() is false");

        Assert.IsNull(driver.GetCurrentChore(),
            "GetCurrentChore() must stay null — SetChore skipped assignment due to IsValid()=false");
    }

    /// <summary>
    /// Verify that driver.context (private field exposed by AssemblyExposer) is settable
    /// and driver.smi.sm.nextChore parameter is accessible for the bypass path.
    /// These are the two writes the IsValid-bypass performs when SetChore's guard blocks assignment.
    /// </summary>
    [Test]
    public void BypassFields_AreAccessible_ForDirectAssignment() {
        var go = createGameObject();
        go.AddComponent<ChoreConsumer>();
        var driver = go.AddComponent<ChoreDriver>();
        driver.smi.StartSM();

        var defaultCtx = default(Chore.Precondition.Context);

        // Verify driver.context is assignable (AssemblyExposer makes the private field public).
        Assert.DoesNotThrow(() => { driver.context = defaultCtx; },
            "driver.context must be assignable — required by bypass path");

        // Verify nextChore parameter on the SM is accessible.
        Assert.IsNotNull(driver.smi.sm.nextChore,
            "driver.smi.sm.nextChore must be non-null after SM start — required by bypass path");

        // Setting nextChore to null is a safe no-op (it's already null in nochore state).
        Assert.DoesNotThrow(() => driver.smi.sm.nextChore.Set(null, driver.smi),
            "nextChore.Set(null) must not throw — bypass path sets it to the found chore");
    }
}
