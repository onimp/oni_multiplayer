using MultiplayerMod.ModRuntime;
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
        var runtime = new Runtime();
        var manager = runtime.Dependencies.Get<ExecutionLevelManager>();
        manager.BaseLevel = ExecutionLevel.Multiplayer;

        var executed = false;
        manager.RunIfLevelIsActive(ExecutionLevel.Game, () => executed = true);

        Assert.IsFalse(executed);
    }

    [Test]
    public void RequiredLevelEqualsToCurrent() {
        var runtime = new Runtime();
        var manager = runtime.Dependencies.Get<ExecutionLevelManager>();
        manager.BaseLevel = ExecutionLevel.Game;

        var executed = false;
        manager.RunIfLevelIsActive(ExecutionLevel.Game, () => executed = true);

        Assert.IsTrue(executed);
    }

    [Test]
    public void RequiredLevelLowerThanCurrent() {
        var runtime = new Runtime();
        var manager = runtime.Dependencies.Get<ExecutionLevelManager>();
        manager.BaseLevel = ExecutionLevel.Game;

        var executed = false;
        manager.RunIfLevelIsActive(ExecutionLevel.Multiplayer, () => executed = true);

        Assert.IsTrue(executed);
    }

    [Test]
    public void RunActionWithTargetExecutionLevel() {
        var runtime = new Runtime();
        var manager = runtime.Dependencies.Get<ExecutionLevelManager>();
        manager.BaseLevel = ExecutionLevel.Game;

        var executedLevel = ExecutionLevel.System;
        manager.RunIfLevelIsActive(
            ExecutionLevel.Multiplayer,
            ExecutionLevel.Command,
            () => executedLevel = runtime.ExecutionContext.Level
        );

        Assert.AreEqual(expected: ExecutionLevel.Command, actual: executedLevel);
    }

    [Test]
    public void AllLevelsUnderTargetOrTargetItselfAreActive() {
        var runtime = new Runtime();
        var manager = runtime.Dependencies.Get<ExecutionLevelManager>();
        manager.BaseLevel = ExecutionLevel.Component;
        Assert.IsTrue(manager.LevelIsActive(ExecutionLevel.System));
        Assert.IsTrue(manager.LevelIsActive(ExecutionLevel.Multiplayer));
        Assert.IsTrue(manager.LevelIsActive(ExecutionLevel.Component));
        Assert.IsFalse(manager.LevelIsActive(ExecutionLevel.Command));
        Assert.IsFalse(manager.LevelIsActive(ExecutionLevel.Game));
    }

}
