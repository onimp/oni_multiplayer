using MultiplayerMod.Core.Dependency;
using MultiplayerMod.ModRuntime.Context;
using MultiplayerMod.Test.Environment.Unity;
using NUnit.Framework;

namespace MultiplayerMod.Test.ModRuntime;

[TestFixture]
public class ExecutionLevelTests {

    [OneTimeSetUp]
    public static void SetUp() => UnityTestRuntime.Install();

    [OneTimeTearDown]
    public static void TearDown() => UnityTestRuntime.Uninstall();

    [Test]
    public void RequiredLevelHigherThanCurrent() {
        var container = CreateContainer();
        var manager = container.Get<ExecutionLevelManager>();
        manager.BaseLevel = ExecutionLevel.Multiplayer;

        var executed = false;
        manager.RunIfLevelIsActive(ExecutionLevel.Game, () => executed = true);

        Assert.IsFalse(executed);
    }

    [Test]
    public void RequiredLevelEqualsToCurrent() {
        var container = CreateContainer();
        var manager = container.Get<ExecutionLevelManager>();
        manager.BaseLevel = ExecutionLevel.Game;

        var executed = false;
        manager.RunIfLevelIsActive(ExecutionLevel.Game, () => executed = true);

        Assert.IsTrue(executed);
    }

    [Test]
    public void RequiredLevelLowerThanCurrent() {
        var container = CreateContainer();
        var manager = container.Get<ExecutionLevelManager>();
        manager.BaseLevel = ExecutionLevel.Game;

        var executed = false;
        manager.RunIfLevelIsActive(ExecutionLevel.Multiplayer, () => executed = true);

        Assert.IsTrue(executed);
    }

    [Test]
    public void RunActionWithTargetExecutionLevel() {
        var container = CreateContainer();
        var manager = container.Get<ExecutionLevelManager>();
        manager.BaseLevel = ExecutionLevel.Game;

        var executedLevel = ExecutionLevel.System;
        manager.RunIfLevelIsActive(
            ExecutionLevel.Multiplayer,
            ExecutionLevel.Command,
            () => executedLevel = container.Get<ExecutionContextManager>().Context.Level
        );

        Assert.AreEqual(expected: ExecutionLevel.Command, actual: executedLevel);
    }

    [Test]
    public void AllLevelsUnderTargetOrTargetItselfAreActive() {
        var container = CreateContainer();
        var manager = container.Get<ExecutionLevelManager>();
        manager.BaseLevel = ExecutionLevel.Component;
        Assert.IsTrue(manager.LevelIsActive(ExecutionLevel.System));
        Assert.IsTrue(manager.LevelIsActive(ExecutionLevel.Multiplayer));
        Assert.IsTrue(manager.LevelIsActive(ExecutionLevel.Component));
        Assert.IsFalse(manager.LevelIsActive(ExecutionLevel.Command));
        Assert.IsFalse(manager.LevelIsActive(ExecutionLevel.Game));
    }

    private static DependencyContainer CreateContainer() => new DependencyContainerBuilder()
        .AddType<ExecutionContextManager>()
        .AddType<ExecutionLevelManager>()
        .Build();

}
