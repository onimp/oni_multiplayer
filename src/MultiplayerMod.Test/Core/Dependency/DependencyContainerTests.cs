using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;
using NUnit.Framework;

namespace MultiplayerMod.Test.Core.Dependency;

[TestFixture]
public class DependencyContainerTests {

    [Test]
    public void ResolvesSimpleDependencies() {
        var container = new DependencyContainer();
        container.Register(CreateInfo<DependencyA>());
        var instance = container.Get<DependencyA>();
        Assert.AreSame(expected: instance, actual: container.Get<IDependencyA>());
        Assert.AreSame(expected: instance, actual: container.Get<IDependencyAb>());
    }

    [Test]
    public void ResolvesListOfDependencies() {
        var container = new DependencyContainer();
        container.Register(CreateInfo<DependencyA>());
        container.Register(CreateInfo<DependencyB>());
        var dependencyAb = container.Get<List<IDependencyAb>>();
        Assert.AreEqual(expected: 2, actual: dependencyAb.Count);
        Assert.Contains(container.Get<DependencyA>(), dependencyAb);
        Assert.Contains(container.Get<DependencyB>(), dependencyAb);
    }

    [Test]
    public void InstantiateWithDependencies() {
        var container = new DependencyContainer();
        container.Register(CreateInfo<DependencyA>());
        container.Register(CreateInfo<DependencyB>());
        container.Register(CreateInfo<DependencyInConstructor>());
        var dependencyInConstructor = container.Get<DependencyInConstructor>();
        Assert.AreSame(expected: container.Get<DependencyA>(), actual: dependencyInConstructor.AInstance);
        Assert.AreSame(expected: container.Get<DependencyB>(), actual: dependencyInConstructor.BInstance);
    }

    [Test]
    public void InjectDependenciesIntoFields() {
        var container = new DependencyContainer();
        container.Register(CreateInfo<DependencyA>());
        container.Register(CreateInfo<DependencyB>());
        var instance = container.Inject(new DependencyInFields());
        Assert.AreSame(expected: container.Get<IDependencyA>(), actual: instance.AInstance);
        Assert.AreSame(expected: container.Get<IDependencyB>(), actual: instance.BInstance);
    }

    [Test]
    public void InjectDependenciesIntoProperties() {
        var container = new DependencyContainer();
        container.Register(CreateInfo<DependencyA>());
        container.Register(CreateInfo<DependencyB>());
        var instance = container.Inject(new DependencyInProperties());
        Assert.AreSame(expected: container.Get<IDependencyA>(), actual: instance.AInstance);
        Assert.AreSame(expected: container.Get<IDependencyB>(), actual: instance.BInstance);
    }

    [Test]
    public void MustInjectIntoStaticProperty() {
        var container = new DependencyContainer();
        container.Register(CreateInfo<DependencyA>());
        container.Inject(typeof(StaticPropertyContainer));
        Assert.AreSame(expected: container.Get<IDependencyA>(), actual: StaticPropertyContainer.DependencyA);
    }

    [Test]
    public void MustInjectIntoStaticField() {
        var container = new DependencyContainer();
        container.Register(CreateInfo<DependencyA>());
        container.Inject(typeof(StaticFieldContainer));
        Assert.AreSame(expected: container.Get<IDependencyA>(), actual: StaticFieldContainer.DependencyA);
    }

    [Test]
    public void RegistersSingleton() {
        var container = new DependencyContainer();
        container.RegisterSingleton("Hello");
        Assert.AreSame(expected: "Hello", actual: container.Get<string>());
    }

    [Test]
    public void MustFailOnMissingDependency() {
        var container = new DependencyContainer();
        container.Register(CreateInfo<DependencyA>());
        container.Register(CreateInfo<DependencyInConstructor>());
        try {
            container.Get<DependencyInConstructor>();
        } catch (DependencyInstantiationException exception) {
            Assert.AreSame(expected: typeof(MissingDependencyException), actual: UnwindCause(exception).GetType());
        }
    }

    [Test]
    public void MustFailOnAlreadyRegisteredDependency() {
        var container = new DependencyContainer();
        container.Register(CreateInfo<DependencyA>());
        Assert.Throws<DependencyAlreadyRegisteredException>(() => container.Register(CreateInfo<DependencyA>()));
    }

    [Test]
    public void MustFailOnCircularDependency() {
        var container = new DependencyContainer();
        container.Register(CreateInfo<CircularDependencyA>());
        container.Register(CreateInfo<CircularDependencyB>());
        try {
            container.Get<CircularDependencyB>();
        } catch (DependencyInstantiationException exception) {
            Assert.AreSame(
                expected: typeof(DependencyIsInstantiatingException),
                actual: UnwindCause(exception).GetType()
            );
        }
    }

    [Test]
    public void MustFailOnAmbiguousDependency() {
        var container = new DependencyContainer();
        container.Register(CreateInfo<DependencyA>());
        container.Register(CreateInfo<DependencyB>());
        Assert.Throws<AmbiguousDependencyException>(() => container.Get<IDependencyAb>());
    }

    [Test]
    public void MustAssignDependenciesToTypeOnce() {
        var container = new DependencyContainer();
        container.Register(CreateInfo<ChildDependency>());
        Assert.DoesNotThrow(() => container.Get<ChildDependency>());
        Assert.DoesNotThrow(() => container.Get<IDependencyA>());
        Assert.DoesNotThrow(() => container.Get<IDependencyB>());
    }

    public class StaticPropertyContainer {
        [InjectDependency]
        public static IDependencyA DependencyA { get; private set; } = null!;
    }

    public class StaticFieldContainer {
        [InjectDependency]
        public static IDependencyA DependencyA = null!;
    }

    private static DependencyInfo CreateInfo<T>() => new(typeof(T).FullName!, typeof(T), false);

    private static Exception UnwindCause(Exception exception) {
        var current = exception;
        while (current.InnerException != null)
            current = current.InnerException;
        return current;
    }

    [UsedImplicitly]
    public class DependencyA : IDependencyA, IDependencyAb { }

    [UsedImplicitly]
    public class DependencyB : IDependencyB, IDependencyAb { }

    [UsedImplicitly]
    public class DependencyInConstructor {
        public readonly IDependencyA AInstance;
        public readonly IDependencyB BInstance;

        public DependencyInConstructor(IDependencyA dependencyA, IDependencyB dependencyB) {
            AInstance = dependencyA;
            BInstance = dependencyB;
        }
    }

    [UsedImplicitly]
    public class CircularDependencyA {
        public CircularDependencyA(CircularDependencyB _) { }
    }

    [UsedImplicitly]
    public class CircularDependencyB {
        public CircularDependencyB(CircularDependencyA _) { }
    }

    [UsedImplicitly]
    public class DependencyInFields {

        [InjectDependency]
        public readonly IDependencyA AInstance = null!;

        [InjectDependency]
        public readonly IDependencyB BInstance = null!;

    }

    [UsedImplicitly]
    public class DependencyInProperties {

        [UsedImplicitly]
        [InjectDependency]
        public IDependencyA AInstance { get; set; } = null!;

        [UsedImplicitly]
        [InjectDependency]
        public IDependencyB BInstance { get; set; } = null!;

    }

    [UsedImplicitly]
    public class ChildDependency : ParentDependency { }

    public class ParentDependency : IDependencyA, IDependencyB { }

    public interface IDependencyA { }

    public interface IDependencyB { }

    public interface IDependencyAb { }

}
