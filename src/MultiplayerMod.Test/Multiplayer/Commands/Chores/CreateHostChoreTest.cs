using System;
using System.Linq;
using MultiplayerMod.Game.Chores;
using MultiplayerMod.Multiplayer.Commands.Chores;
using MultiplayerMod.Network;
using MultiplayerMod.Platform.Steam.Network.Messaging;
using MultiplayerMod.Test.Game.Chores;
using NUnit.Framework;

namespace MultiplayerMod.Test.Multiplayer.Commands.Chores;

[TestFixture]
public class CreateHostChoreTest : AbstractChoreTest {

    [Test, TestCaseSource(nameof(GetTestArgs))]
    public void ExecutionTest(Type choreType, Func<object?[]> expectedArgsFunc) {
        var arg = new CreateNewChoreArgs(choreType, expectedArgsFunc.Invoke());

        var command = new CreateHostChore(arg);

        command.Execute(null!);
    }

    [Test, TestCaseSource(nameof(GetTestArgs))]
    public void SerializationTest(Type choreType, Func<object?[]> expectedArgsFunc) {
        var arg = new CreateNewChoreArgs(choreType, expectedArgsFunc.Invoke());
        var command = new CreateHostChore(arg);
        var messageFactory = new NetworkMessageFactory();
        var messageProcessor = new NetworkMessageProcessor();
        NetworkMessage? networkMessage = null;

        foreach (var messageHandle in messageFactory.Create(command, MultiplayerCommandOptions.SkipHost).ToArray()) {
            networkMessage = messageProcessor.Process(1u, messageHandle);
        }

        Assert.AreEqual(command.GetType(), networkMessage?.Command.GetType());
        Assert.AreEqual(command.ChoreType, ((CreateHostChore) networkMessage!.Command).ChoreType);
        Assert.AreEqual(command.Args, ((CreateHostChore) networkMessage!.Command).Args);
    }
}
