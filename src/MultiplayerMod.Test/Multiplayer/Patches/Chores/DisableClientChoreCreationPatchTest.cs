using System;
using System.Collections.Generic;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Scheduling;
using MultiplayerMod.Game.Chores;
using MultiplayerMod.ModRuntime;
using MultiplayerMod.Multiplayer;
using MultiplayerMod.Multiplayer.Patches.Chores;
using MultiplayerMod.Test.Game.Chores;
using NUnit.Framework;

namespace MultiplayerMod.Test.Multiplayer.Patches.Chores;

[TestFixture]
public class DisableClientChoreCreationPatchTest : AbstractChoreTest {

    [SetUp]
    public void SetUp() {
        SetUpGame(new HashSet<Type>() { typeof(DisableClientChoreCreationPatch) });

        Runtime.Instance.Dependencies.Get<MultiplayerGame>().Refresh(MultiplayerMode.Client);
        var di = (DependencyContainer) Runtime.Instance.Dependencies;
        di.Register(new DependencyInfo(nameof(UnityTaskScheduler), typeof(UnityTaskScheduler), false));
    }

    [Test, TestCaseSource(nameof(GetCreationTestArgs))]
    public void ClientChoresMustBeCancelled(Type choreType, Func<object[]> expectedArgsFunc) {
        var chore = CreateChore(choreType, expectedArgsFunc.Invoke());
        var provider = chore.provider;
        Runtime.Instance.Dependencies.Get<UnityTaskScheduler>().Tick();

        Assert.Null(chore.provider);
        Assert.AreEqual(0, provider.choreWorldMap[chore.gameObject.GetMyParentWorldId()].Count);
    }
}
