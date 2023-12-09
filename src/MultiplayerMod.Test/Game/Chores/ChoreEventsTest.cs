using System;
using MultiplayerMod.Game.Chores;
using NUnit.Framework;

namespace MultiplayerMod.Test.Game.Chores;

[TestFixture]
public class ChoreEventsTest : AbstractChoreTest {

    [SetUp]
    public new void SetUp() {
        Patches.Add(typeof(ChoreEvents));

        base.SetUp();
    }

    [Test, TestCaseSource(nameof(GetTestArgs))]
    public void TestEventFiring(Type choreType, Func<object?[]> expectedArgsFunc) {
        CreateNewChoreArgs? firedArgs = null;
        ChoreEvents.CreateNewChore += args => firedArgs = args;

        CreateChore(choreType, expectedArgsFunc.Invoke());

        Assert.NotNull(firedArgs);
        Assert.AreEqual(choreType, firedArgs!.ChoreType);
        var expected = expectedArgsFunc.Invoke();
        Assert.AreEqual(expected, firedArgs.Args);
    }
}
