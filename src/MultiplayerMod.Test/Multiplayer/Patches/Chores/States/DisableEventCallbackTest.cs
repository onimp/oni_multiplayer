﻿using System;
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
public class DisableEventCallbackTest : AbstractChoreTest {

    [OneTimeSetUp]
    public static void OneTimeSetUp() {
        var di = (DependencyContainer) Runtime.Instance.Dependencies;
        di.Register(new DependencyInfo(nameof(StatesManager), typeof(StatesManager), false));
    }

    [SetUp]
    public void SetUp() {
        CreateTestData(new HashSet<Type> { typeof(DisableEventCallback) });
        Singleton<StateMachineManager>.Instance.Clear();
    }

    [Test, TestCaseSource(nameof(EventHandlerTestArgs))]
    public void ClientMustDisableEventCalls(
        Type choreType,
        Func<object[]> choreArgsFunc,
        Func<Dictionary<int, object?>> stateTransitionArgsFunc,
        StateTransitionConfig config
    ) {
        Runtime.Instance.Dependencies.Get<MultiplayerGame>().Refresh(MultiplayerMode.Client);
        var chore = CreateChore(choreType, choreArgsFunc.Invoke());

        var statesManager = Runtime.Instance.Dependencies.Get<StatesManager>();
        var smi = statesManager.GetSmi(chore);
        Assert.IsEmpty(config.GetMonitoredState(smi.stateMachine).events);
    }

    [Test, TestCaseSource(nameof(EventHandlerTestArgs))]
    public void HostMustKeepEventCalls(
        Type choreType,
        Func<object[]> choreArgsFunc,
        Func<Dictionary<int, object?>> stateTransitionArgsFunc,
        StateTransitionConfig config
    ) {
        Runtime.Instance.Dependencies.Get<MultiplayerGame>().Refresh(MultiplayerMode.Host);
        var chore = CreateChore(choreType, choreArgsFunc.Invoke());

        var statesManager = Runtime.Instance.Dependencies.Get<StatesManager>();
        var smi = statesManager.GetSmi(chore);
        Assert.IsNotEmpty(config.GetMonitoredState(smi.stateMachine).events);
    }

}
