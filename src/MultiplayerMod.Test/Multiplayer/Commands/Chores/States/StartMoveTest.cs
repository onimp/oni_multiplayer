using System;
using System.Collections.Generic;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Events;
using MultiplayerMod.Core.Reflection;
using MultiplayerMod.Game.Chores.States;
using MultiplayerMod.Game.Chores.Types;
using MultiplayerMod.ModRuntime;
using MultiplayerMod.Multiplayer.Commands;
using MultiplayerMod.Multiplayer.Commands.Chores.States;
using MultiplayerMod.Multiplayer.Objects;
using MultiplayerMod.Multiplayer.Objects.Extensions;
using MultiplayerMod.Multiplayer.States;
using MultiplayerMod.Test.Game.Chores;
using NUnit.Framework;

namespace MultiplayerMod.Test.Multiplayer.Commands.Chores.States;

[TestFixture]
public class StartMoveTest : AbstractChoreTest {

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
        var sm = smi.stateMachine;
        var state = config.GetMonitoredState(sm);
        state.enterActions.Clear();
        state.transitions.Clear();
        var cellOffsets = new[] { new CellOffset(1, 1) };
        const int cell = 12;
        var arg = new MoveToArgs(chore, config.StateToMonitorName, cell, cellOffsets);
        var command = SerializeDeserializeCommand(new StartMove(arg));

        command.Execute(new MultiplayerCommandContext(null, new MultiplayerCommandRuntimeAccessor(Runtime.Instance)));

        var target = sm.GetFieldValue("stateTarget");
        var navigator = (Navigator) target.GetType().GetMethod("Get")
            .MakeGenericMethod(typeof(Navigator))
            .Invoke(target, new object[] { smi });
        Assert.AreEqual(state, smi.GetCurrentState());
        Assert.AreEqual(cell, Grid.PosToCell(navigator.targetLocator));
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
        var arg = new MoveToArgs(chore, config.StateToMonitorName, 12, new[] { new CellOffset(1, 1) });
        var command = new StartMove(arg);

        var networkCommand = SerializeDeserializeCommand(command);

        Assert.AreEqual(command.GetType(), networkCommand.GetType());
        Assert.AreEqual(command.ChoreId, ((StartMove) networkCommand).ChoreId);
        Assert.AreEqual(command.TargetState, ((StartMove) networkCommand).TargetState);
        Assert.AreEqual(command.Cell, ((StartMove) networkCommand).Cell);
        Assert.AreEqual(command.Offsets, ((StartMove) networkCommand).Offsets);
    }

}
