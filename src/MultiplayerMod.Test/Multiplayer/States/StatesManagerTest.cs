using System;
using System.Collections.Generic;
using System.Linq;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Game.Chores.Types;
using MultiplayerMod.ModRuntime;
using MultiplayerMod.Multiplayer.States;
using MultiplayerMod.Test.Game.Chores;
using NUnit.Framework;

namespace MultiplayerMod.Test.Multiplayer.States;

[TestFixture]
public class StatesManagerTest : AbstractChoreTest {

    [OneTimeSetUp]
    public static void OneTimeSetUp() {
        var di = (DependencyContainer) Runtime.Instance.Dependencies;
        di.Register(new DependencyInfo(nameof(StatesManager), typeof(StatesManager), false));
    }

    [SetUp]
    public void SetUp() {
        CreateTestData();
    }

    [Test, TestCaseSource(nameof(ExitTestArgs))]
    public void AllowTransitionPreparesWaitHostState(
        Type choreType,
        Func<object[]> choreArgsFunc,
        Func<Dictionary<int, object?>> stateTransitionArgsFunc,
        StateTransitionConfig config
    ) {
        var sm = Singleton<StateMachineManager>.Instance.CreateStateMachine(GetStatesType(choreType));
        var statesManager = Runtime.Instance.Dependencies.Get<StatesManager>();
        statesManager.InjectWaitHostState(sm);
        var expectedDictionary = stateTransitionArgsFunc.Invoke();
        var chore = CreateChore(choreType, choreArgsFunc.Invoke());
        var smi = statesManager.GetSmi(chore);

        statesManager.AllowTransition(
            chore,
            config.StateToMonitorName,
            expectedDictionary
        );

        var waitHostState = (dynamic) statesManager.GetWaitHostState(chore);
        Assert.True(waitHostState.TransitionAllowed.Get(smi));
        Assert.AreEqual(config.StateToMonitorName, waitHostState.TargetState.Get(smi));
        Assert.AreEqual(expectedDictionary, waitHostState.ParametersArgs.Get(smi));
    }

    [Test, TestCaseSource(nameof(ExitTestArgs))]
    public void DisableChoreStateTransition(
        Type choreType,
        Func<object[]> choreArgsFunc,
        Func<Dictionary<int, object?>> stateTransitionArgsFunc,
        StateTransitionConfig config
    ) {
        var sm = Singleton<StateMachineManager>.Instance.CreateStateMachine(GetStatesType(choreType));
        var statesManager = Runtime.Instance.Dependencies.Get<StatesManager>();
        statesManager
            .ReplaceWithWaitState(sm.GetState("root"));
        var chore = CreateChore(choreType, choreArgsFunc.Invoke());
        var smi = statesManager.GetSmi(chore);

        smi.GoTo("root." + config.StateToMonitorName);

        var waitHostState = (dynamic) statesManager.GetWaitHostState(chore);
        Assert.AreEqual(waitHostState, smi.GetCurrentState());
    }

    private Type GetStatesType(Type choreType) => choreType.GetNestedTypes().Single(type => type.Name == "States");
}
