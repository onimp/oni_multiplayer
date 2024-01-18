using System;
using System.Collections.Generic;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Game.Chores;
using MultiplayerMod.ModRuntime;
using MultiplayerMod.Multiplayer;
using MultiplayerMod.Multiplayer.Patches.Chores.States;
using MultiplayerMod.Multiplayer.States;
using MultiplayerMod.Test.Game.Chores;
using NUnit.Framework;

namespace MultiplayerMod.Test.Multiplayer.Patches.Chores.States;

[TestFixture]
public class DisableUpdateCallbackTest : AbstractChoreTest {

    [OneTimeSetUp]
    public static void OneTimeSetUp() {
        var di = (DependencyContainer) Runtime.Instance.Dependencies;
        di.Register(new DependencyInfo(nameof(StatesManager), typeof(StatesManager), false));
    }

    [SetUp]
    public void SetUp() {
        CreateTestData(new HashSet<Type> { typeof(DisableUpdateCallback) });
        Singleton<StateMachineManager>.Instance.Clear();
    }

    private static IEnumerable<object[]> TestArgs() {
        return GetTransitionTestArgs(ChoreList.StateTransitionConfig.TransitionTypeEnum.Update);
    }

    [Test, TestCaseSource(nameof(TestArgs))]
    public void ClientMustDisableUpdateCalls(
        Type choreType,
        Func<object[]> choreArgsFunc,
        Func<Dictionary<int, object?>> stateTransitionArgsFunc,
        ChoreList.StateTransitionConfig config
    ) {
        Runtime.Instance.Dependencies.Get<MultiplayerGame>().Refresh(MultiplayerMode.Client);
        var chore = CreateChore(choreType, choreArgsFunc.Invoke());

        var statesManager = Runtime.Instance.Dependencies.Get<StatesManager>();
        var smi = statesManager.GetSmi(chore);
        Assert.IsEmpty(config.GetMonitoredState(smi.stateMachine).updateActions);
    }

    [Test, TestCaseSource(nameof(TestArgs))]
    public void HostMustKeepUpdateCalls(
        Type choreType,
        Func<object[]> choreArgsFunc,
        Func<Dictionary<int, object?>> stateTransitionArgsFunc,
        ChoreList.StateTransitionConfig config
    ) {
        Runtime.Instance.Dependencies.Get<MultiplayerGame>().Refresh(MultiplayerMode.Host);
        var chore = CreateChore(choreType, choreArgsFunc.Invoke());

        var statesManager = Runtime.Instance.Dependencies.Get<StatesManager>();
        var smi = statesManager.GetSmi(chore);
        Assert.IsNotEmpty(config.GetMonitoredState(smi.stateMachine).updateActions);
    }

}
