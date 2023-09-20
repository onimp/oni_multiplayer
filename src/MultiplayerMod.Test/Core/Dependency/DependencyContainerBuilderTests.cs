using MultiplayerMod.Core.Dependency;
using NUnit.Framework;

namespace MultiplayerMod.Test.Core.Dependency;

[TestFixture]
public class DependencyContainerBuilderTests {

    [Test]
    public void ContainerReferencesAvailable() {
        var container = new DependencyContainerBuilder().Build();
        var containerByInterface = container.Get<IDependencyContainer>();
        var injector = container.Get<IDependencyInjector>();
        var self = container.Get<DependencyContainer>();
        Assert.AreSame(expected: container, actual: self);
        Assert.AreSame(expected: container, actual: containerByInterface);
        Assert.AreSame(expected: container, actual: injector);
    }

}
