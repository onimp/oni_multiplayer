using MultiplayerMod.ModRuntime;
using MultiplayerMod.ModRuntime.Context;
using MultiplayerMod.ModRuntime.StaticCompatibility;
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
        var manager = runtime.Dependencies.Get<ExecutionContextManager>();
        manager.Replace(new ExecutionContext(ExecutionLevel.Multiplayer));

        var executed = false;
        Execution.RunIfPossible(ExecutionLevel.Gameplay, () => executed = true);

        Assert.IsFalse(executed);
    }

    [Test]
    public void RequiredLevelEqualsToCurrent() {
        var runtime = new Runtime();
        var manager = runtime.Dependencies.Get<ExecutionContextManager>();
        manager.Replace(new ExecutionContext(ExecutionLevel.Gameplay));

        var executed = false;
        Execution.RunIfPossible(ExecutionLevel.Gameplay, () => executed = true);

        Assert.IsTrue(executed);
    }

    [Test]
    public void RequiredLevelLowerThanCurrent() {
        var runtime = new Runtime();
        var manager = runtime.Dependencies.Get<ExecutionContextManager>();
        manager.Replace(new ExecutionContext(ExecutionLevel.Gameplay));

        var executed = false;
        Execution.RunIfPossible(ExecutionLevel.Multiplayer, () => executed = true);

        Assert.IsTrue(executed);
    }

}
