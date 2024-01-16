using System;
using System.Collections.Generic;
using System.Linq;
using MultiplayerMod.Core.Dependency;
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
        SetUpGame();

    }

    [Test, TestCaseSource(nameof(GetTransitionTestArgs))]
    public void AllowTransitionPreparesWaitHostState(
        Type choreType,
        Func<object[]> choreArgsFunc,
        Func<object[]> stateTransitionArgsFunc
    ) {
        var sm = Singleton<StateMachineManager>.Instance.CreateStateMachine(GetStatesType(choreType));
        var statesManager = Runtime.Instance.Dependencies.Get<StatesManager>();
        statesManager.InjectWaitHostState(sm);
        var stateTransitionArgs = stateTransitionArgsFunc.Invoke();
        var chore = CreateChore(choreType, choreArgsFunc.Invoke());
        var smi = statesManager.GetSmi(chore);

        statesManager.AllowTransition(
            chore,
            (string?) stateTransitionArgs[0],
            (Dictionary<int, object>) stateTransitionArgs[1]
        );

        var waitHostState = (dynamic) statesManager.GetWaitHostState(chore);
        Assert.True(waitHostState.TransitionAllowed.Get(smi));
        Assert.AreEqual(stateTransitionArgs[0], waitHostState.TargetState.Get(smi));
        Assert.AreEqual(stateTransitionArgs[1], waitHostState.ParametersArgs.Get(smi));
    }

    [Test, TestCaseSource(nameof(GetTransitionTestArgs))]
    public void DisableChoreStateTransition(
        Type choreType,
        Func<object[]> choreArgsFunc,
        Func<object[]> stateTransitionArgsFunc
    ) {
        var sm = Singleton<StateMachineManager>.Instance.CreateStateMachine(GetStatesType(choreType));
        var statesManager = Runtime.Instance.Dependencies.Get<StatesManager>();
        statesManager
            .DisableChoreStateTransition(sm.GetState("root"));
        var chore = CreateChore(choreType, choreArgsFunc.Invoke());
        var smi = statesManager.GetSmi(chore);

        chore.Begin(
            new Chore.Precondition.Context {
                consumerState = new ChoreConsumerState(target.GetComponent<ChoreConsumer>())
            }
        );

        var waitHostState = (dynamic) statesManager.GetWaitHostState(chore);
        Assert.AreEqual(waitHostState, smi.GetCurrentState());
    }

    private Type GetStatesType(Type choreType) => choreType.GetNestedTypes().Single(type => type.Name == "States");
}
