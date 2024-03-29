using System;
using System.Collections.Generic;
using MultiplayerMod.Game.Chores;
using NUnit.Framework;

namespace MultiplayerMod.Test.Game.Chores;

[TestFixture]
public class ChoreEventsTest : AbstractChoreTest {

    [SetUp]
    public void SetUp() {
        CreateTestData(new HashSet<Type> { typeof(ChoreEvents) });
    }

    [Test, TestCaseSource(nameof(GetCreationTestArgs))]
    public void TestEventFiring(Type choreType, Func<object[]> choreArgsFunc) {
        CreateNewChoreArgs? firedArgs = null;
        ChoreEvents.CreateNewChore += args => firedArgs = args;
        var expected = choreArgsFunc.Invoke();

        CreateChore(choreType, expected);

        Assert.NotNull(firedArgs);
        Assert.NotNull(firedArgs!.ChoreId);
        Assert.AreEqual(choreType, firedArgs!.ChoreType);
        Assert.AreNotEqual(0, firedArgs!.ChoreId.UuidA);
        Assert.AreNotEqual(0, firedArgs!.ChoreId.UuidB);
        Assert.AreEqual(expected, firedArgs.Args);
    }
}
