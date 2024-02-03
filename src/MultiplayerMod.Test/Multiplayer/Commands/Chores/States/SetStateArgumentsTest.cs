using System;
using System.Collections.Generic;
using System.Linq;
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
public class SetStateArgumentsTest : AbstractChoreTest {

    [OneTimeSetUp]
    public static void OneTimeSetUp() {
        var di = (DependencyContainer) Runtime.Instance.Dependencies;
        di.Register(new DependencyInfo(nameof(EventDispatcher), typeof(EventDispatcher), false));
        di.Register(new DependencyInfo(nameof(StatesManager), typeof(StatesManager), false));
    }

    [SetUp]
    public void SetUp() {
        CreateTestData();
    }

    private static IEnumerable<object[]> TestArgs() {
        return UpdateTestArgs().Union(EventHandlerTestArgs());
    }

    [Test, TestCaseSource(nameof(TestArgs))]
    public void ExecutionTest(
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
        var command = SerializeDeserializeCommand(new SetStateArguments(arg));

        command.Execute(new MultiplayerCommandContext(null, new MultiplayerCommandRuntimeAccessor(Runtime.Instance)));
    }

    [Test, TestCaseSource(nameof(TestArgs))]
    public void SerializationTest(
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
        var command = new SetStateArguments(arg);

        var networkCommand = SerializeDeserializeCommand(command);

        Assert.AreEqual(command.GetType(), networkCommand.GetType());
        Assert.AreEqual(command.ChoreId, ((SetStateArguments) networkCommand).ChoreId);
        Assert.AreEqual(command.Args.Keys, ((SetStateArguments) networkCommand).Args.Keys);
    }

}
