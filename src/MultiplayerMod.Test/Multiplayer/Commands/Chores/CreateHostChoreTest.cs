using System;
using MultiplayerMod.Game.Chores;
using MultiplayerMod.Multiplayer.Commands.Chores;
using MultiplayerMod.Multiplayer.Objects;
using MultiplayerMod.Test.Game.Chores;
using NUnit.Framework;

namespace MultiplayerMod.Test.Multiplayer.Commands.Chores;

[TestFixture]
public class CreateHostChoreTest : AbstractChoreTest {

    [SetUp]
    public void SetUp() {
        CreateTestData();
    }

    [Test, TestCaseSource(nameof(GetCreationTestArgs))]
    public void ExecutionTest(Type choreType, Func<object[]> choreArgsFunc) {
        var choreId = new MultiplayerId(Guid.NewGuid());
        var arg = new CreateNewChoreArgs(choreId, choreType, choreArgsFunc.Invoke());
        var command = SerializeDeserializeCommand(new CreateHostChore(arg));

        command.Execute(null!);
        var chore = ChoreObjects.GetChore(choreId);

        Assert.NotNull(chore);
    }

    [Test, TestCaseSource(nameof(GetCreationTestArgs))]
    public void SerializationTest(Type choreType, Func<object[]> choreArgsFunc) {
        var arg = new CreateNewChoreArgs(new MultiplayerId(Guid.NewGuid()), choreType, choreArgsFunc.Invoke());
        var command = new CreateHostChore(arg);

        var networkCommand = SerializeDeserializeCommand(command);

        Assert.AreEqual(command.GetType(), networkCommand.GetType());
        Assert.AreEqual(command.ChoreId, ((CreateHostChore) networkCommand).ChoreId);
        Assert.AreEqual(command.ChoreType, ((CreateHostChore) networkCommand).ChoreType);
        Assert.AreEqual(command.Args, ((CreateHostChore) networkCommand).Args);
    }
}
