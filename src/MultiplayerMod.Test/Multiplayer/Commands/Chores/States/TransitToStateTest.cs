using System;
using System.Collections.Generic;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Game.Chores.Types;
using MultiplayerMod.ModRuntime;
using MultiplayerMod.Multiplayer.Chores.Commands.States;
using MultiplayerMod.Multiplayer.Commands;
using MultiplayerMod.Multiplayer.Objects;
using MultiplayerMod.Multiplayer.Objects.Extensions;
using MultiplayerMod.Multiplayer.States;
using MultiplayerMod.Test.Game.Chores;
using NUnit.Framework;

namespace MultiplayerMod.Test.Multiplayer.Commands.Chores.States;

[TestFixture]
public class TransitToStateTest : AbstractChoreTest {

    [OneTimeSetUp]
    public static void OneTimeSetUp() {
        var di = (DependencyContainer) Runtime.Instance.Dependencies;
        di.Register(new DependencyInfo(nameof(StatesManager), typeof(StatesManager), false));
    }

    [SetUp]
    public void SetUp() {
        CreateTestData();
    }

    [Test, TestCaseSource(nameof(MoveToTestArgs))]
    public void ExecutionTest(
        Type choreType,
        Func<object[]> createChoreArgsFunc,
        Func<Dictionary<int, object?>> stateTransitionArgsFunc,
        StateTransitionConfig config
    ) {
        var chore = CreateChore(choreType, createChoreArgsFunc.Invoke());
        chore.Register(new MultiplayerId(Guid.NewGuid()));
        var smi = (StateMachine.Instance) chore.GetType().GetProperty("smi").GetValue(chore);
        smi.stateMachine.GetState("root").transitions?.Clear();
        var stateName = config.StateToMonitorName != "root" ? "root" : "random";
        var command = SerializeDeserializeCommand(new TransitToState(chore, stateName));

        command.Execute(new MultiplayerCommandContext(null, new MultiplayerCommandRuntimeAccessor(Runtime.Instance)));

        Assert.AreEqual(smi.stateMachine.GetState(stateName), smi.GetCurrentState());
    }

    [Test, TestCaseSource(nameof(MoveToTestArgs))]
    public void SerializationTest(
        Type choreType,
        Func<object[]> createChoreArgsFunc,
        Func<Dictionary<int, object?>> stateTransitionArgsFunc,
        StateTransitionConfig config
    ) {
        var chore = CreateChore(choreType, createChoreArgsFunc.Invoke());
        chore.Register(new MultiplayerId(Guid.NewGuid()));
        var command = new TransitToState(chore, config.StateToMonitorName != "root" ? "root" : "random");

        var networkCommand = SerializeDeserializeCommand(command);

        Assert.AreEqual(command.GetType(), networkCommand.GetType());
        Assert.AreEqual(command.ChoreId, ((TransitToState) networkCommand).ChoreId);
        Assert.AreEqual(command.State, ((TransitToState) networkCommand).State);
    }

}
