using MultiplayerMod.Core.Dependency;
using MultiplayerMod.ModRuntime.Context;
using MultiplayerMod.Test.Environment.Unity;
using NUnit.Framework;

namespace MultiplayerMod.Test.ModRuntime;

[TestFixture]
public class ExecutionContextManagerTests {

    [OneTimeSetUp]
    public static void SetUp() => UnityTestRuntime.Install();

    [OneTimeTearDown]
    public static void TearDown() => UnityTestRuntime.Uninstall();

    [Test]
    public void BaseContextChange() {
        var container = CreateContainer();
        var manager = container.Get<ExecutionContextManager>();

        Assert.AreEqual(expected: ExecutionLevel.System, actual: manager.Context.Level);
        manager.BaseContext = new ExecutionContext(ExecutionLevel.Game);
        Assert.AreEqual(expected: ExecutionLevel.Game, actual: manager.Context.Level);
    }

    [Test]
    public void BasicOverride() {
        var container = CreateContainer();
        var manager = container.Get<ExecutionContextManager>();
        var levelManager = container.Get<ExecutionLevelManager>();
        levelManager.RunUsingLevel(
            ExecutionLevel.Command,
            () => { Assert.AreEqual(expected: ExecutionLevel.Command, actual: manager.Context.Level); }
        );
        Assert.AreEqual(expected: ExecutionLevel.System, actual: manager.Context.Level);
    }

    [Test]
    public void RestoreEmptyContextException() {
        var container = CreateContainer();
        var manager = container.Get<ExecutionContextManager>();
        Assert.Throws<ExecutionContextIntegrityFailureException>(() => manager.LeaveOverrideSection());
    }

    [Test]
    public void ManualGuardedOverrideStaleContextException() {
        var container = CreateContainer();
        var manager = container.Get<ExecutionContextManager>();
        manager.EnterOverrideSection(new ExecutionContext(ExecutionLevel.Game));
        UnityTestRuntime.NextFrame();
        Assert.Throws<ExecutionContextIntegrityFailureException>(
            () => manager.EnterOverrideSection(new ExecutionContext(ExecutionLevel.Game))
        );
    }

    [Test]
    public void ManualGuardedOverride() {
        var container = CreateContainer();
        var manager = container.Get<ExecutionContextManager>();
        manager.EnterOverrideSection(new ExecutionContext(ExecutionLevel.Game));
        Assert.AreEqual(expected: ExecutionLevel.Game, actual: manager.Context.Level);
        manager.LeaveOverrideSection();
        UnityTestRuntime.NextFrame();
        Assert.AreEqual(expected: ExecutionLevel.System, actual: manager.Context.Level);
    }

    private static DependencyContainer CreateContainer() => new DependencyContainerBuilder()
        .AddType<ExecutionContextManager>()
        .AddType<ExecutionLevelManager>()
        .Build();

}
