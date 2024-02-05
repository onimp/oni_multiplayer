using System;
using System.Collections.Generic;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Scheduling;
using MultiplayerMod.ModRuntime;
using MultiplayerMod.Multiplayer;
using MultiplayerMod.Multiplayer.Patches.Chores;
using MultiplayerMod.Test.Game.Chores;
using NUnit.Framework;

namespace MultiplayerMod.Test.Multiplayer.Patches.Chores;

[TestFixture]
public class DisableClientChoreCreationPatchTest : AbstractChoreTest {

    [OneTimeSetUp]
    public static void OneTimeSetUp() {
        Runtime.Instance.Dependencies.Get<MultiplayerGame>().Refresh(MultiplayerMode.Client);
        var di = (DependencyContainer) Runtime.Instance.Dependencies;
        di.Register(new DependencyInfo(nameof(UnityTaskScheduler), typeof(UnityTaskScheduler), false));
    }

    [SetUp]
    public void SetUp() {
        CreateTestData(new HashSet<Type>() { typeof(DisableClientChoreCreationPatch) });
    }

    [Test, TestCaseSource(nameof(GetCreationTestArgs))]
    public void ClientChoresMustBeCancelled(Type choreType, Func<object[]> choreArgsFunc) {
        var chore = CreateChore(choreType, choreArgsFunc.Invoke());
        chore.addToDailyReport = false;
        var provider = chore.provider;
        Runtime.Instance.Dependencies.Get<UnityTaskScheduler>().Tick();

        Assert.Null(chore.provider);
        Assert.AreEqual(0, provider.choreWorldMap[chore.gameObject.GetMyParentWorldId()].Count);
    }
}
