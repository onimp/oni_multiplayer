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
        CreateTestData();

        Grid.BuildMasks[410] = Grid.BuildFlags.Solid;

        StateMachine.Instance.error = false;
    }

    private static IEnumerable<object[]> TestArgs() {
        return GetTransitionOnExitTestArgs()
            .GroupBy(it => it[0])
            .Select(group => group.First())
            .Select(it => new[] { it[0], it[1] });
    }

    [Test, TestCaseSource(nameof(TestArgs))]
    public void ParametersBeingInjectedInSmi(
        Type choreType,
        Func<object[]> choreArgsFunc
    ) {
        var sm = Singleton<StateMachineManager>.Instance.CreateStateMachine(GetStatesType(choreType));
        Runtime.Instance.Dependencies.Get<StatesManager>().InjectWaitHostState(sm);

        var chore = CreateChore(choreType, choreArgsFunc.Invoke());
        var state = (dynamic) Runtime.Instance.Dependencies.Get<StatesManager>().GetWaitHostState(chore);

        Assert.NotNull(state.TransitionAllowed);
        Assert.NotNull(state.TargetState);
        Assert.NotNull(state.ParametersArgs);
    }

    [Test, TestCaseSource(nameof(TestArgs))]
    public void ParametersCanBeEdited(Type choreType, Func<object[]> choreArgsFunc) {
        var sm = Singleton<StateMachineManager>.Instance.CreateStateMachine(GetStatesType(choreType));
        var statesManager = Runtime.Instance.Dependencies.Get<StatesManager>();
        statesManager.InjectWaitHostState(sm);
        var chore = CreateChore(choreType, choreArgsFunc.Invoke());
        var smi = statesManager.GetSmi(chore);
        var state = (dynamic) statesManager.GetWaitHostState(chore);

        state.TargetState.Set("TestValue", smi);

        Assert.AreEqual("TestValue", state.TargetState.Get(smi));
    }

    [Test, TestCaseSource(nameof(TestArgs))]
    public void StateDoesNotChangeIfNotAllowed(Type choreType, Func<object[]> choreArgsFunc) {
        var sm = Singleton<StateMachineManager>.Instance.CreateStateMachine(GetStatesType(choreType));
        var statesManager = Runtime.Instance.Dependencies.Get<StatesManager>();
        statesManager.InjectWaitHostState(sm);
        var chore = CreateChore(choreType, choreArgsFunc.Invoke());
        var smi = statesManager.GetSmi(chore);
        var state = (dynamic) statesManager.GetWaitHostState(chore);
        smi.GetStateMachine().GetState("root").enterActions?.Clear();

        smi.GoTo(state);

        Assert.AreEqual(state, smi.GetCurrentState());
    }

    [Test, TestCaseSource(nameof(TestArgs))]
    public void StateDoesNotChangeIfNotWaitingButAllowed(
        Type choreType,
        Func<object[]> choreArgsFunc
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

    [Test, TestCaseSource(nameof(TestArgs))]
    public void StateChangesIfAllowedAndWaiting(Type choreType, Func<object[]> choreArgsFunc) {
        var sm = Singleton<StateMachineManager>.Instance.CreateStateMachine(GetStatesType(choreType));
        var statesManager = Runtime.Instance.Dependencies.Get<StatesManager>();
        statesManager.InjectWaitHostState(sm);
        var chore = CreateChore(choreType, choreArgsFunc.Invoke());
        var smi = statesManager.GetSmi(chore);
        var state = (dynamic) statesManager.GetWaitHostState(chore);
        var currentState = smi.GetCurrentState();
        smi.GetStateMachine().GetState("root").enterActions?.Clear();

        smi.GoTo(state);
        state.AllowTransition(smi, "root", new Dictionary<int, object>());

        Assert.AreNotEqual(currentState, smi.GetCurrentState());
        Assert.AreEqual("root", smi.GetCurrentState().name);
    }

    [Test, TestCaseSource(nameof(TestArgs))]
    public void StateChangesIfBeginWaitingAndAllowed(Type choreType, Func<object[]> choreArgsFunc) {
        var sm = Singleton<StateMachineManager>.Instance.CreateStateMachine(GetStatesType(choreType));
        var statesManager = Runtime.Instance.Dependencies.Get<StatesManager>();
        statesManager.InjectWaitHostState(sm);
        var chore = CreateChore(choreType, choreArgsFunc.Invoke());
        var smi = statesManager.GetSmi(chore);
        var waitState = (dynamic) statesManager.GetWaitHostState(chore);
        smi.GetStateMachine().GetState("root").enterActions?.Clear();

        waitState.AllowTransition(smi, "root", new Dictionary<int, object>());
        smi.GoTo(waitState);

        Assert.AreEqual("root", smi.GetCurrentState().name);
    }

    private Type GetStatesType(Type choreType) => choreType.GetNestedTypes().Single(type => type.Name == "States");
}
