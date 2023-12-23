using System;
using System.Linq;
using MultiplayerMod.Game.Chores;
using MultiplayerMod.Multiplayer.Commands.Chores;
using MultiplayerMod.Multiplayer.Objects;
using MultiplayerMod.Network;
using MultiplayerMod.Platform.Steam.Network.Messaging;
using MultiplayerMod.Test.Game.Chores;
using NUnit.Framework;

namespace MultiplayerMod.Test.Multiplayer.Commands.Chores;

[TestFixture]
public class CreateHostChoreTest : AbstractChoreTest {

    [SetUp]
    public void SetUp() {
        SetUpGame();
    }

    [Test, TestCaseSource(nameof(GetCreationTestArgs))]
    public void ExecutionTest(Type choreType, Func<object[]> choreArgsFunc) {
        var choreId = new MultiplayerId(Guid.NewGuid());
        var arg = new CreateNewChoreArgs(choreId, choreType, choreArgsFunc.Invoke());
        var command = new CreateHostChore(arg);

        command.Execute(null!);
        var chore = ChoreObjects.GetChore(choreId);

        Assert.NotNull(chore);
    }

    [Test, TestCaseSource(nameof(GetCreationTestArgs))]
    public void SerializationTest(Type choreType, Func<object[]> choreArgsFunc) {
        var arg = new CreateNewChoreArgs(new MultiplayerId(Guid.NewGuid()), choreType, choreArgsFunc.Invoke());
        var command = new CreateHostChore(arg);
        var messageFactory = new NetworkMessageFactory();
        var messageProcessor = new NetworkMessageProcessor();
        NetworkMessage? networkMessage = null;

        foreach (var messageHandle in messageFactory.Create(command, MultiplayerCommandOptions.SkipHost).ToArray()) {
            networkMessage = messageProcessor.Process(1u, messageHandle);
        }

        Assert.AreEqual(command.GetType(), networkMessage?.Command.GetType());
        Assert.AreEqual(command.ChoreId, ((CreateHostChore) networkMessage!.Command).ChoreId);
        Assert.AreEqual(command.ChoreType, ((CreateHostChore) networkMessage!.Command).ChoreType);
        Assert.AreEqual(command.Args, ((CreateHostChore) networkMessage!.Command).Args);
    }
}
