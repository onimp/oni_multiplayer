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
}
