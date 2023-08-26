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
    public void MainContextChange() {
        var container = CreateContainer();
        var manager = container.Get<ExecutionContextManager>();

        Assert.AreEqual(expected: ExecutionLevel.System, actual: manager.EffectiveContext.Level);
        manager.MainContext = new ExecutionContext(ExecutionLevel.Runtime);
        Assert.AreEqual(expected: ExecutionLevel.Runtime, actual: manager.EffectiveContext.Level);
    }

    [Test]
    public void BasicOverride() {
        var container = CreateContainer();
        var manager = container.Get<ExecutionContextManager>();
        manager.UsingLevel(
            ExecutionLevel.Command,
            () => { Assert.AreEqual(expected: ExecutionLevel.Command, actual: manager.EffectiveContext.Level); }
        );
        Assert.AreEqual(expected: ExecutionLevel.System, actual: manager.EffectiveContext.Level);
    }

    [Test]
    public void RestoreEmptyContextException() {
        var container = CreateContainer();
        var manager = container.Get<ExecutionContextManager>();
        Assert.Throws<ExecutionContextIntegrityFailureException>(() => manager.RestoreContext());
    }

    [Test]
    public void ManualGuardedOverrideStaleContextException() {
        var container = CreateContainer();
        var manager = container.Get<ExecutionContextManager>();
        manager.OverrideContext(new ExecutionContext(ExecutionLevel.Runtime));
        UnityTestRuntime.NextFrame();
        Assert.Throws<ExecutionContextIntegrityFailureException>(
            () => manager.OverrideContext(new ExecutionContext(ExecutionLevel.Runtime))
        );
    }

    [Test]
    public void ManualGuardedOverride() {
        var container = CreateContainer();
        var manager = container.Get<ExecutionContextManager>();
        manager.OverrideContext(new ExecutionContext(ExecutionLevel.Runtime));
        Assert.AreEqual(expected: ExecutionLevel.Runtime, actual: manager.EffectiveContext.Level);
        manager.RestoreContext();
        UnityTestRuntime.NextFrame();
        Assert.AreEqual(expected: ExecutionLevel.System, actual: manager.EffectiveContext.Level);
    }

    private static DependencyContainer CreateContainer() {
        var container = new DependencyContainer();
        container.Register<ExecutionContextManager>();
        return container;
    }

}
