using System;
using System.Collections.Generic;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Game.Chores.Types;
using MultiplayerMod.ModRuntime;
using MultiplayerMod.Multiplayer;
using MultiplayerMod.Multiplayer.Patches.Chores.States;
using MultiplayerMod.Multiplayer.States;
using MultiplayerMod.Test.Game.Chores;
using NUnit.Framework;

namespace MultiplayerMod.Test.Multiplayer.Patches.Chores.States;

[TestFixture]
public class DisableChoreStateTransitionTest : AbstractChoreTest {

    [OneTimeSetUp]
    public static void OneTimeSetUp() {
        Runtime.Instance.Dependencies.Get<MultiplayerGame>().Refresh(MultiplayerMode.Client);

        var di = (DependencyContainer) Runtime.Instance.Dependencies;
        di.Register(new DependencyInfo(nameof(FakeStatesManager), typeof(FakeStatesManager), false));
    }

    [SetUp]
    public void SetUp() {
        CreateTestData(new HashSet<Type> { typeof(DisableChoreStateTransition) });
        Singleton<StateMachineManager>.Instance.Clear();
    }

    [Test, TestCaseSource(nameof(EnterTestArgs))]
    public void ClientStateEnter_DisablesTransition(
        Type choreType,
        Func<object[]> choreArgsFunc,
        Func<Dictionary<int, object?>> stateTransitionArgsFunc,
        StateTransitionConfig config
    ) {
        var statesManager = Runtime.Instance.Dependencies.Get<FakeStatesManager>();
        statesManager.CalledWithStates.Clear();

        var chore = CreateChore(choreType, choreArgsFunc.Invoke());

        var smi = statesManager.GetSmi(chore);
        var expectedState = config.GetMonitoredState(smi.stateMachine);
        Assert.Contains(expectedState, statesManager.CalledWithStates);
    }

    [Test, TestCaseSource(nameof(ExitTestArgs))]
    public void ClientStateExit_DisablesTransition(
        Type choreType,
        Func<object[]> choreArgsFunc,
        Func<Dictionary<int, object?>> stateTransitionArgsFunc,
        StateTransitionConfig config
    ) {
        var statesManager = Runtime.Instance.Dependencies.Get<FakeStatesManager>();
        statesManager.CalledWithStates.Clear();

        var chore = CreateChore(choreType, choreArgsFunc.Invoke());

        var smi = statesManager.GetSmi(chore);
        var expectedState = config.GetMonitoredState(smi.stateMachine);
        Assert.Contains(expectedState, statesManager.CalledWithStates);
    }

    private class FakeStatesManager : StatesManager {
        public readonly List<StateMachine.BaseState> CalledWithStates = new();

        public override void ReplaceWithWaitState(StateMachine.BaseState stateToBeSynced) {
            CalledWithStates.Add(stateToBeSynced);
        }
    }

}
