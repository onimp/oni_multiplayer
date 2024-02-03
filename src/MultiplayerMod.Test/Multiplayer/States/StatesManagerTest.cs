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

    protected static IEnumerable<object[]> TestCases() {
        return ExitTestArgs().Union(EnterTestArgs());
    }

    [Test, TestCaseSource(nameof(TestCases))]
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

    [Test]
    public void AddContinuationState_CopiesArgumentsFromOriginal() {
        var statesManager = Runtime.Instance.Dependencies.Get<StatesManager>();
        var stateToBeSynced =
            new GameStateMachine<AggressiveChore.States, AggressiveChore.StatesInstance, AggressiveChore, object>.State {
                defaultState = new StateMachine.BaseState(),
                enterActions = new List<StateMachine.Action> { new() },
                exitActions = new List<StateMachine.Action> { new() },
                updateActions = new List<StateMachine.UpdateAction> { new() },
                events = new List<StateEvent> {
                    new GameStateMachine<AggressiveChore.States, AggressiveChore.StatesInstance, AggressiveChore, object>.GameEvent(GameHashes.Absorb, null, null, null)
                },
                sm = new AggressiveChore.States()
            };

        var createdState = statesManager.AddContinuationState(stateToBeSynced);

        Assert.AreEqual(stateToBeSynced.defaultState, createdState.defaultState);
        Assert.Null(createdState.enterActions);
        Assert.AreEqual(stateToBeSynced.exitActions, createdState.exitActions);
        Assert.Null(createdState.updateActions);
        Assert.Null(createdState.events);
    }

    [Test, TestCaseSource(nameof(EnterTestArgs))]
    public void EnterArgs_DisableChoreStateTransition(
        Type choreType,
        Func<object[]> choreArgsFunc,
        Func<Dictionary<int, object?>> stateTransitionArgsFunc,
        StateTransitionConfig config
    ) {
        var sm = Singleton<StateMachineManager>.Instance.CreateStateMachine(GetStatesType(choreType));
        sm.GetState("root").events?.Clear();
        sm.GetState("root").enterActions?.Clear();
        var statesManager = Runtime.Instance.Dependencies.Get<StatesManager>();
        var stateToBeSynced = config.GetMonitoredState(sm);
        stateToBeSynced.enterActions.Clear();
        statesManager.AddAndTransitToWaiStateUponEnter(stateToBeSynced);
        var chore = CreateChore(choreType, choreArgsFunc.Invoke());
        var smi = statesManager.GetSmi(chore);

        smi.GoTo(stateToBeSynced);

        var waitHostState = (dynamic) statesManager.GetWaitHostState(chore);
        Assert.AreEqual(waitHostState, smi.GetCurrentState());
    }

    [Test, TestCaseSource(nameof(ExitTestArgs))]
    public void ExitArgs_DisableChoreStateTransition(
        Type choreType,
        Func<object[]> choreArgsFunc,
        Func<Dictionary<int, object?>> stateTransitionArgsFunc,
        StateTransitionConfig config
    ) {
        var sm = Singleton<StateMachineManager>.Instance.CreateStateMachine(GetStatesType(choreType));
        sm.defaultState = sm.GetState("root");
        sm.GetState("root.delivering")?.enterActions?.Clear();
        var statesManager = Runtime.Instance.Dependencies.Get<StatesManager>();
        var stateToBeSynced = config.GetMonitoredState(sm);
        stateToBeSynced.enterActions.Clear();
        statesManager.AddAndTransitToWaiStateUponEnter(stateToBeSynced);
        var chore = CreateChore(choreType, choreArgsFunc.Invoke());
        var smi = statesManager.GetSmi(chore);
        chore.Begin(
            new Chore.Precondition.Context {
                consumerState = new ChoreConsumerState(Minion.GetComponent<ChoreConsumer>()),
                data = PickupableGameObject.GetComponent<Pickupable>()
            }
        );

        smi.GoTo(stateToBeSynced);

        var waitHostState = (dynamic) statesManager.GetWaitHostState(chore);
        Assert.AreEqual(waitHostState, smi.GetCurrentState());
    }

    private Type GetStatesType(Type choreType) => choreType.GetNestedTypes().Single(type => type.Name == "States");
}
