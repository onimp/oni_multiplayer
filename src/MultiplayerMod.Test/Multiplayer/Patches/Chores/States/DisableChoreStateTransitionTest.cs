using System;
using System.Collections.Generic;
using System.Linq;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Events;
using MultiplayerMod.Game.Chores;
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
    }

    private static IEnumerable<object[]> TestArgs() {
        return GetTransitionTestArgs(ChoreList.StateTransitionConfig.TransitionTypeEnum.Exit)
            .GroupBy(it => it[0])
            .Select(
                group => {
                    var allConfigs = group.Select(it => it[3]).Count();
                    return new[] { group.First()[0], group.First()[1], allConfigs };
                }
            );
    }

    [Test, TestCaseSource(nameof(TestArgs))]
    public void ClientMustDisableTransitionViaManager(
        Type choreType,
        Func<object[]> choreArgsFunc,
        int configCounts
    ) {
        Runtime.Instance.Dependencies.Get<FakeStatesManager>().CalledTimes = 0;

        CreateChore(choreType, choreArgsFunc.Invoke());

        Assert.AreEqual(configCounts, Runtime.Instance.Dependencies.Get<FakeStatesManager>().CalledTimes);
    }

    private class FakeStatesManager : StatesManager {
        public int CalledTimes;

        public override void ReplaceWithWaitState(StateMachine.BaseState stateToBeSynced) {
            CalledTimes++;
        }
    }

}
