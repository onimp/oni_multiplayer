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
    public void RootContextChange() {
        var container = CreateContainer();
        var manager = container.Get<ExecutionContextManager>();

        Assert.AreEqual(expected: ExecutionLevel.System, actual: manager.Context.Level);
        manager.Replace(new ExecutionContext(ExecutionLevel.Runtime));
        Assert.AreEqual(expected: ExecutionLevel.Runtime, actual: manager.Context.Level);
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
        Assert.Throws<ExecutionContextIntegrityFailureException>(() => manager.Pop());
    }

    [Test]
    public void ManualGuardedOverrideStaleContextException() {
        var container = CreateContainer();
        var manager = container.Get<ExecutionContextManager>();
        manager.Push(new ExecutionContext(ExecutionLevel.Runtime));
        UnityTestRuntime.NextFrame();
        Assert.Throws<ExecutionContextIntegrityFailureException>(
            () => manager.Push(new ExecutionContext(ExecutionLevel.Runtime))
        );
    }

    [Test]
    public void ManualGuardedOverride() {
        var container = CreateContainer();
        var manager = container.Get<ExecutionContextManager>();
        manager.Push(new ExecutionContext(ExecutionLevel.Runtime));
        Assert.AreEqual(expected: ExecutionLevel.Runtime, actual: manager.Context.Level);
        manager.Pop();
        UnityTestRuntime.NextFrame();
        Assert.AreEqual(expected: ExecutionLevel.System, actual: manager.Context.Level);
    }

    private static DependencyContainer CreateContainer() {
        var container = new DependencyContainer();
        container.Register<ExecutionContextManager>();
        container.Register<ExecutionLevelManager>();
        return container;
    }

}
