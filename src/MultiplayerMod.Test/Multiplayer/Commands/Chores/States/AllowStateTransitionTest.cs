using System;
using System.Collections.Generic;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Events;
using MultiplayerMod.Game.Chores.States;
using MultiplayerMod.Game.Chores.Types;
using MultiplayerMod.ModRuntime;
using MultiplayerMod.Multiplayer.Commands;
using MultiplayerMod.Multiplayer.Commands.Chores.States;
using MultiplayerMod.Multiplayer.Objects;
using MultiplayerMod.Multiplayer.States;
using MultiplayerMod.Test.Game.Chores;
using NUnit.Framework;

namespace MultiplayerMod.Test.Multiplayer.Commands.Chores.States;

[TestFixture]
public class AllowStateTransitionTest : AbstractChoreTest {

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

    [Test, TestCaseSource(nameof(EnterTestArgs))]
    public void ExecutionTestEnter(
        Type choreType,
        Func<object[]> createChoreArgsFunc,
        Func<Dictionary<int, object?>> stateTransitionArgsFunc,
        StateTransitionConfig config
    ) {
        var chore = CreateChore(choreType, createChoreArgsFunc.Invoke());
        chore.Register(new MultiplayerId(Guid.NewGuid()));
        var arg = new ChoreTransitStateArgs(
            chore,
            config.StateToMonitorName,
            stateTransitionArgsFunc.Invoke()
        );
        var command = SerializeDeserializeCommand(AllowStateTransition.EnterTransition(arg));

        command.Execute(new MultiplayerCommandContext(null, new MultiplayerCommandRuntimeAccessor(Runtime.Instance)));

        Assert.AreEqual(
            config.StateToMonitorName + "_" + StatesManager.ContinuationName,
            Runtime.Instance.Dependencies.Get<FakeStatesManager>().TransitToState
        );
    }

    [Test, TestCaseSource(nameof(ExitTestArgs))]
    public void ExecutionTestExit(
        Type choreType,
        Func<object[]> createChoreArgsFunc,
        Func<Dictionary<int, object?>> stateTransitionArgsFunc,
        StateTransitionConfig config
    ) {
        var chore = CreateChore(choreType, createChoreArgsFunc.Invoke());
        chore.Register(new MultiplayerId(Guid.NewGuid()));
        var arg = new ChoreTransitStateArgs(
            chore,
            config.StateToMonitorName,
            stateTransitionArgsFunc.Invoke()
        );
        var command = SerializeDeserializeCommand(AllowStateTransition.ExitTransition(arg));

        command.Execute(new MultiplayerCommandContext(null, new MultiplayerCommandRuntimeAccessor(Runtime.Instance)));

        Assert.AreEqual(
            config.StateToMonitorName,
            Runtime.Instance.Dependencies.Get<FakeStatesManager>().TransitToState
        );
    }

    [Test, TestCaseSource(nameof(TransitionTestArgs))]
    public void ExecutionTestTransition(
        Type choreType,
        Func<object[]> createChoreArgsFunc,
        Func<Dictionary<int, object?>> stateTransitionArgsFunc,
        StateTransitionConfig config
    ) {
        var chore = CreateChore(choreType, createChoreArgsFunc.Invoke());
        chore.Register(new MultiplayerId(Guid.NewGuid()));
        var arg = new ChoreTransitStateArgs(
            chore,
            config.StateToMonitorName,
            stateTransitionArgsFunc.Invoke()
        );
        var command = SerializeDeserializeCommand(AllowStateTransition.ExitTransition(arg));

        command.Execute(new MultiplayerCommandContext(null, new MultiplayerCommandRuntimeAccessor(Runtime.Instance)));

        Assert.AreEqual(
            config.StateToMonitorName,
            Runtime.Instance.Dependencies.Get<FakeStatesManager>().TransitToState
        );
    }

    [Test, TestCaseSource(nameof(EnterTestArgs))]
    public void SerializationTestEnter(
        Type choreType,
        Func<object[]> createChoreArgsFunc,
        Func<Dictionary<int, object?>> stateTransitionArgsFunc,
        StateTransitionConfig config
    ) {
        var chore = CreateChore(choreType, createChoreArgsFunc.Invoke());
        chore.Register(new MultiplayerId(Guid.NewGuid()));
        var arg = new ChoreTransitStateArgs(
            chore,
            config.StateToMonitorName,
            stateTransitionArgsFunc.Invoke()
        );
        var command = AllowStateTransition.EnterTransition(arg);

        var networkCommand = SerializeDeserializeCommand(command);

        Assert.AreEqual(command.GetType(), networkCommand.GetType());
        Assert.AreEqual(command.ChoreId, ((AllowStateTransition) networkCommand).ChoreId);
        Assert.AreEqual(command.TargetState, ((AllowStateTransition) networkCommand).TargetState);
        Assert.AreEqual(command.Args.Keys, ((AllowStateTransition) networkCommand).Args.Keys);
    }

    [Test, TestCaseSource(nameof(ExitTestArgs))]
    public void SerializationTestExit(
        Type choreType,
        Func<object[]> createChoreArgsFunc,
        Func<Dictionary<int, object?>> stateTransitionArgsFunc,
        StateTransitionConfig config
    ) {
        var chore = CreateChore(choreType, createChoreArgsFunc.Invoke());
        chore.Register(new MultiplayerId(Guid.NewGuid()));
        var arg = new ChoreTransitStateArgs(
            chore,
            config.StateToMonitorName,
            stateTransitionArgsFunc.Invoke()
        );
        var command = AllowStateTransition.ExitTransition(arg);

        var networkCommand = SerializeDeserializeCommand(command);

        Assert.AreEqual(command.GetType(), networkCommand.GetType());
        Assert.AreEqual(command.ChoreId, ((AllowStateTransition) networkCommand).ChoreId);
        Assert.AreEqual(command.TargetState, ((AllowStateTransition) networkCommand).TargetState);
        Assert.AreEqual(command.Args.Keys, ((AllowStateTransition) networkCommand).Args.Keys);
    }

    [Test, TestCaseSource(nameof(TransitionTestArgs))]
    public void SerializationTestTransition(
        Type choreType,
        Func<object[]> createChoreArgsFunc,
        Func<Dictionary<int, object?>> stateTransitionArgsFunc,
        StateTransitionConfig config
    ) {
        var chore = CreateChore(choreType, createChoreArgsFunc.Invoke());
        chore.Register(new MultiplayerId(Guid.NewGuid()));
        var arg = new ChoreTransitStateArgs(
            chore,
            config.StateToMonitorName,
            stateTransitionArgsFunc.Invoke()
        );
        var command = AllowStateTransition.ExitTransition(arg);

        var networkCommand = SerializeDeserializeCommand(command);

        Assert.AreEqual(command.GetType(), networkCommand.GetType());
        Assert.AreEqual(command.ChoreId, ((AllowStateTransition) networkCommand).ChoreId);
        Assert.AreEqual(command.TargetState, ((AllowStateTransition) networkCommand).TargetState);
        Assert.AreEqual(command.Args.Keys, ((AllowStateTransition) networkCommand).Args.Keys);
    }

    private class FakeStatesManager : StatesManager {
        public string? TransitToState;

        public override void AllowTransition(Chore chore, string? targetState, Dictionary<int, object?> args) {
            TransitToState = targetState;
        }
    }
}
