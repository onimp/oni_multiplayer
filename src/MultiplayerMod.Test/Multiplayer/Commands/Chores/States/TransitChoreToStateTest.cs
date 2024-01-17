using System;
using System.Collections.Generic;
using System.Linq;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Events;
using MultiplayerMod.Game.Chores;
using MultiplayerMod.Game.Chores.States;
using MultiplayerMod.ModRuntime;
using MultiplayerMod.Multiplayer.Commands;
using MultiplayerMod.Multiplayer.Commands.Chores.States;
using MultiplayerMod.Multiplayer.Objects;
using MultiplayerMod.Multiplayer.States;
using MultiplayerMod.Network;
using MultiplayerMod.Platform.Steam.Network.Messaging;
using MultiplayerMod.Test.Game.Chores;
using NUnit.Framework;

namespace MultiplayerMod.Test.Multiplayer.Commands.Chores.States;

[TestFixture]
public class TransitChoreToStateTest : AbstractChoreTest {

    [OneTimeSetUp]
    public static void OneTimeSetUp() {
        var di = (DependencyContainer) Runtime.Instance.Dependencies;
        di.Register(new DependencyInfo(nameof(EventDispatcher), typeof(EventDispatcher), false));
        di.Register(new DependencyInfo(nameof(FakeStatesManager), typeof(FakeStatesManager), false));
    }

    [SetUp]
    public void SetUp() {
        CreateTestData();
    }

    [Test, TestCaseSource(nameof(GetTransitionOnExitTestArgs))]
    public void ExecutionTest(
        Type choreType,
        Func<object[]> createChoreArgsFunc,
        Func<Dictionary<int, object?>> stateTransitionArgsFunc,
        ChoreList.StateTransitionConfig config
    ) {
        var chore = CreateChore(choreType, createChoreArgsFunc.Invoke());
        chore.Register(new MultiplayerId(Guid.NewGuid()));
        var arg = new ChoreTransitStateArgs(
            chore,
            config.StateToMonitorName,
            stateTransitionArgsFunc.Invoke()
        );
        var command = new TransitChoreToState(arg);

        command.Execute(new MultiplayerCommandContext(null, new MultiplayerCommandRuntimeAccessor(Runtime.Instance)));

        Assert.True(Runtime.Instance.Dependencies.Get<FakeStatesManager>().WasCalled);
    }

    [Test, TestCaseSource(nameof(GetTransitionOnExitTestArgs))]
    public void SerializationTest(
        Type choreType,
        Func<object[]> createChoreArgsFunc,
        Func<Dictionary<int, object?>> stateTransitionArgsFunc,
        ChoreList.StateTransitionConfig config
    ) {
        var chore = CreateChore(choreType, createChoreArgsFunc.Invoke());
        chore.Register(new MultiplayerId(Guid.NewGuid()));
        var arg = new ChoreTransitStateArgs(
            chore,
            config.StateToMonitorName,
            stateTransitionArgsFunc.Invoke()
        );
        var command = new TransitChoreToState(arg);
        var messageFactory = new NetworkMessageFactory();
        var messageProcessor = new NetworkMessageProcessor();
        NetworkMessage? networkMessage = null;

        foreach (var messageHandle in messageFactory.Create(command, MultiplayerCommandOptions.SkipHost).ToArray()) {
            networkMessage = messageProcessor.Process(1u, messageHandle);
        }

        Assert.AreEqual(command.GetType(), networkMessage?.Command.GetType());
        Assert.AreEqual(command.ChoreId, ((TransitChoreToState) networkMessage!.Command).ChoreId);
        Assert.AreEqual(command.TargetState, ((TransitChoreToState) networkMessage.Command).TargetState);
        Assert.AreEqual(command.Args.Keys, ((TransitChoreToState) networkMessage.Command).Args.Keys);
    }

    private class FakeStatesManager : StatesManager {
        public bool WasCalled;

        public override void AllowTransition(Chore chore, string? targetState, Dictionary<int, object?> args) {
            WasCalled = true;
        }
    }
}
