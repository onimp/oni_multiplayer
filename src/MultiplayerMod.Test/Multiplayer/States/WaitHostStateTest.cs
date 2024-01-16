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
public class WaitHostStateTest : AbstractChoreTest {

    [OneTimeSetUp]
    public static void OneTimeSetUp() {
        var di = (DependencyContainer) Runtime.Instance.Dependencies;
        di.Register(new DependencyInfo(nameof(StatesManager), typeof(StatesManager), false));
    }

    [SetUp]
    public void SetUp() {
        SetUpGame();

        Grid.BuildMasks[410] = Grid.BuildFlags.Solid;

        StateMachine.Instance.error = false;
    }

    [Test, TestCaseSource(nameof(GetTransitionTestArgs))]
    public void ParametersBeingInjectedInSmi(Type choreType, Func<object[]> choreArgsFunc, Func<object[]> _) {
        var sm = Singleton<StateMachineManager>.Instance.CreateStateMachine(GetStatesType(choreType));
        Runtime.Instance.Dependencies.Get<StatesManager>().InjectWaitHostState(sm);

        var chore = CreateChore(choreType, choreArgsFunc.Invoke());
        var state = (dynamic) Runtime.Instance.Dependencies.Get<StatesManager>().GetWaitHostState(chore);

        Assert.NotNull(state.TransitionAllowed);
        Assert.NotNull(state.TargetState);
        Assert.NotNull(state.ParametersArgs);
    }

    [Test, TestCaseSource(nameof(GetTransitionTestArgs))]
    public void ParametersCanBeEdited(Type choreType, Func<object[]> choreArgsFunc, Func<object[]> _) {
        var sm = Singleton<StateMachineManager>.Instance.CreateStateMachine(GetStatesType(choreType));
        var statesManager = Runtime.Instance.Dependencies.Get<StatesManager>();
        statesManager.InjectWaitHostState(sm);
        var chore = CreateChore(choreType, choreArgsFunc.Invoke());
        var smi = statesManager.GetSmi(chore);
        var state = (dynamic) statesManager.GetWaitHostState(chore);

        state.TargetState.Set("TestValue", smi);

        Assert.AreEqual("TestValue", state.TargetState.Get(smi));
    }

    [Test, TestCaseSource(nameof(GetTransitionTestArgs))]
    public void StateDoesNotChangeIfNotAllowed(Type choreType, Func<object[]> choreArgsFunc, Func<object[]> _) {
        var sm = Singleton<StateMachineManager>.Instance.CreateStateMachine(GetStatesType(choreType));
        var statesManager = Runtime.Instance.Dependencies.Get<StatesManager>();
        statesManager.InjectWaitHostState(sm);
        var chore = CreateChore(choreType, choreArgsFunc.Invoke());
        var smi = statesManager.GetSmi(chore);
        var state = (dynamic) statesManager.GetWaitHostState(chore);
        chore.Begin(
            new Chore.Precondition.Context {
                consumerState = new ChoreConsumerState(target.GetComponent<ChoreConsumer>())
            }
        );

        smi.GoTo(state);

        Assert.AreEqual(state, smi.GetCurrentState());
    }

    [Test, TestCaseSource(nameof(GetTransitionTestArgs))]
    public void StateDoesNotChangeIfNotWaitingButAllowed(
        Type choreType,
        Func<object[]> choreArgsFunc,
        Func<object[]> _
    ) {
        var sm = Singleton<StateMachineManager>.Instance.CreateStateMachine(GetStatesType(choreType));
        var statesManager = Runtime.Instance.Dependencies.Get<StatesManager>();
        statesManager.InjectWaitHostState(sm);
        var chore = CreateChore(choreType, choreArgsFunc.Invoke());
        var smi = statesManager.GetSmi(chore);
        var state = (dynamic) statesManager.GetWaitHostState(chore);
        var currentState = smi.GetCurrentState();

        state.AllowTransition(smi, "root", new Dictionary<int, object>());

        Assert.AreEqual(currentState, smi.GetCurrentState());
    }

    [Test, TestCaseSource(nameof(GetTransitionTestArgs))]
    public void StateChangesIfAllowedAndWaiting(Type choreType, Func<object[]> choreArgsFunc, Func<object[]> _) {
        var sm = Singleton<StateMachineManager>.Instance.CreateStateMachine(GetStatesType(choreType));
        var statesManager = Runtime.Instance.Dependencies.Get<StatesManager>();
        statesManager.InjectWaitHostState(sm);
        var chore = CreateChore(choreType, choreArgsFunc.Invoke());
        var smi = statesManager.GetSmi(chore);
        var state = (dynamic) statesManager.GetWaitHostState(chore);
        var currentState = smi.GetCurrentState();

        smi.GoTo(state);
        state.AllowTransition(smi, "root", new Dictionary<int, object>());

        Assert.AreNotEqual(currentState, smi.GetCurrentState());
        Assert.AreEqual("root", smi.GetCurrentState().name);
    }

    [Test, TestCaseSource(nameof(GetTransitionTestArgs))]
    public void StateChangesIfBeginWaitingAndAllowed(Type choreType, Func<object[]> choreArgsFunc, Func<object[]> _) {
        var sm = Singleton<StateMachineManager>.Instance.CreateStateMachine(GetStatesType(choreType));
        var statesManager = Runtime.Instance.Dependencies.Get<StatesManager>();
        statesManager.InjectWaitHostState(sm);
        var chore = CreateChore(choreType, choreArgsFunc.Invoke());
        var smi = statesManager.GetSmi(chore);
        var state = (dynamic) statesManager.GetWaitHostState(chore);
        var currentState = smi.GetCurrentState();

        state.AllowTransition(smi, "root", new Dictionary<int, object>());
        smi.GoTo(state);

        Assert.AreNotEqual(currentState, smi.GetCurrentState());
        Assert.AreEqual("root", smi.GetCurrentState().name);
    }

    private Type GetStatesType(Type choreType) => choreType.GetNestedTypes().Single(type => type.Name == "States");
}
