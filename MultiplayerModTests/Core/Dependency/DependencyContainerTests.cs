using MultiplayerMod.Core.Dependency;
using NUnit.Framework;

// ReSharper disable ClassNeverInstantiated.Global

namespace MultiplayerModTests.Core.Dependency;

[TestFixture]
public class DependencyContainerTests {

    [Test]
    public void SelfReferenceAvailable() {
        var container = new DependencyContainer();
        var self = container.Resolve<DependencyContainer>();
        Assert.AreSame(expected: container, actual: self);
    }

    [Test]
    public void SimpleInstantiation() {
        var container = new DependencyContainer();
        container.Register<ISimpleInterface, SimpleClass>();
        var instance = container.Resolve<ISimpleInterface>();
        Assert.NotNull(instance);
    }

    [Test]
    public void DependencyIsAvailableAsClass() {
        var container = new DependencyContainer();
        container.Register<ISimpleInterface, SimpleClass>();
        Assert.AreSame(expected: container.Resolve<ISimpleInterface>(), actual: container.Resolve<SimpleClass>());
    }

    [Test]
    public void DependencyRegisteredAsClass() {
        var container = new DependencyContainer();
        container.Register<SimpleClass>();
        Assert.NotNull(container.Resolve<SimpleClass>());
    }

    [Test]
    public void InstantiationWithDependenciesInConstructor() {
        var container = new DependencyContainer();
        container.Register<ISimpleInterface, SimpleClass>();
        var instance = container.Resolve<ClassWithConstructorDependencies>();
        Assert.AreSame(expected: container.Resolve<ISimpleInterface>(), actual: instance.Interface);
    }

    [Test]
    public void InstantiationWithDependenciesInProperties() {
        var container = new DependencyContainer();
        container.Register<ISimpleInterface, SimpleClass>();
        var instance = container.Resolve<ClassWithPropertyDependencies>();
        Assert.AreSame(expected: container.Resolve<ISimpleInterface>(), actual: instance.Interface);
    }

    [Test]
    public void InstantiationWithDependenciesInFields() {
        var container = new DependencyContainer();
        container.Register<ISimpleInterface, SimpleClass>();
        var instance = container.Resolve<ClassWithFieldDependencies>();
        Assert.AreSame(expected: container.Resolve<ISimpleInterface>(), actual: instance.Interface);
    }

    [Test]
    public void NestedDependencies() {
        var container = new DependencyContainer();
        container.Register<ISimpleInterface, SimpleClass>();
        container.Register<INestedDependenciesInterface, NestedDependenciesClass>();
        var instance = container.Resolve<ClassWithComplexDependencies>();
        var nested = (NestedDependenciesClass) instance.Nested;
        Assert.AreSame(expected: container.Resolve<INestedDependenciesInterface>(), actual: instance.Nested);
        Assert.AreSame(expected: container.Resolve<ISimpleInterface>(), actual: instance.Simple);
        Assert.AreSame(expected: container.Resolve<ISimpleInterface>(), actual: nested.Interface);
    }

    [Test]
    public void NonSingletonInstantiation() {
        var container = new DependencyContainer();
        container.Register<ISimpleInterface, SimpleClass>();
        var instance = container.Resolve<ClassWithConstructorDependencies>();
        Assert.AreSame(expected: container.Resolve<ISimpleInterface>(), actual: instance.Interface);
        Assert.Throws(typeof(MissingDependencyException), () => container.Get<ClassWithConstructorDependencies>());
    }

    [Test]
    public void InjectDependencies() {
        var container = new DependencyContainer();
        container.Register<ISimpleInterface, SimpleClass>();
        var instance = container.Inject(new ClassWithFieldDependencies());
        Assert.Throws(typeof(MissingDependencyException), () => container.Get<ClassWithFieldDependencies>());
        Assert.AreSame(expected: container.Resolve<ISimpleInterface>(), actual: instance.Interface);
    }

    [Test]
    public void MixedInstantiation() {
        var container = new DependencyContainer();
        container.Register<ISimpleInterface, SimpleClass>();
        var instance = container.Resolve<ClassWithMixedDependencies>();
        Assert.AreSame(expected: container.Resolve<ISimpleInterface>(), actual: instance.Interface);
        Assert.NotNull(instance.Dependency);
        Assert.Throws(typeof(MissingDependencyException), () => container.Get<NonSingletonDependency>());
    }

    public interface ISimpleInterface { }

    public class SimpleClass : ISimpleInterface { }

    public interface INestedDependenciesInterface { }

    public class NestedDependenciesClass : INestedDependenciesInterface {
        public ISimpleInterface Interface { get; }

        public NestedDependenciesClass(ISimpleInterface @interface) {
            Interface = @interface;
        }
    }

    public class ClassWithConstructorDependencies {
        public ISimpleInterface? Interface { get; }

        public ClassWithConstructorDependencies(ISimpleInterface @interface) {
            Interface = @interface;
        }
    }

    public class ClassWithPropertyDependencies {

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        [Dependency] public ISimpleInterface? Interface { get; set; }

    }

    public class ClassWithFieldDependencies {

        // ReSharper disable once UnassignedField.Global
        [Dependency] public ISimpleInterface? Interface;

    }

    public class NonSingletonDependency { }

    public class ClassWithMixedDependencies {

        public ISimpleInterface? Interface;
        public NonSingletonDependency? Dependency;

        public ClassWithMixedDependencies(ISimpleInterface? @interface, NonSingletonDependency? dependency) {
            Interface = @interface;
            Dependency = dependency;
        }

    }

    public class ClassWithComplexDependencies {

        public ISimpleInterface Simple;
        public INestedDependenciesInterface Nested;

        public ClassWithComplexDependencies(ISimpleInterface simple, INestedDependenciesInterface nested) {
            Simple = simple;
            Nested = nested;
        }

    }

}
