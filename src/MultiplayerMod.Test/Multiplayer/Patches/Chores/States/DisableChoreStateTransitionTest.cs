using System;
using System.Collections.Generic;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Events;
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
        di.Register(new DependencyInfo(nameof(EventDispatcher), typeof(EventDispatcher), false));
        di.Register(new DependencyInfo(nameof(FakeStatesManager), typeof(FakeStatesManager), false));
    }

    [SetUp]
    public void SetUp() {
        SetUpGame(new HashSet<Type> { typeof(DisableChoreStateTransition) });
    }

    [Test, TestCaseSource(nameof(GetTransitionTestArgs))]
    public void ClientMustDisableTransitionViaManager(Type choreType, Func<object[]> choreArgsFunc, Func<object[]> _) {
        CreateChore(choreType, choreArgsFunc.Invoke());

        Assert.True(Runtime.Instance.Dependencies.Get<FakeStatesManager>().WasCalled);
    }

    private class FakeStatesManager : StatesManager {
        public bool WasCalled;

        public override void DisableChoreStateTransition(StateMachine.BaseState stateToBeSynced) {
            WasCalled = true;
        }
    }

}
