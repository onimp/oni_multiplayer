using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MultiplayerMod.Core.Dependency;

public class DependencyContainerBuilder {

    private readonly List<Assembly> assemblies = new();
    private readonly List<Type> includeTypes = new();
    private readonly List<object> singletons = new();

    public DependencyContainerBuilder ScanAssembly(Assembly assembly) {
        assemblies.Add(assembly);
        return this;
    }

    public DependencyContainerBuilder AddType<T>() => AddType(typeof(T));

    public DependencyContainerBuilder AddType(Type type) {
        includeTypes.Add(type);
        return this;
    }

    public DependencyContainerBuilder AddSingleton(object singleton) {
        singletons.Add(singleton);
        return this;
    }

    public DependencyContainer Build() {
        var container = new DependencyContainer();
        container.RegisterSingleton(container);
        singletons.ForEach(it => container.RegisterSingleton(it));
        var types = assemblies.SelectMany(it => it.DefinedTypes)
            .Where(it => it.GetCustomAttribute<DependencyAttribute>() != null)
            .Union(includeTypes);
        foreach (var type in types) {
            var attribute = type.GetCustomAttribute<DependencyAttribute>();
            var dependencyInfo = new DependencyInfo(attribute?.Name ?? type.FullName!, type, attribute?.Lazy ?? false);
            container.Register(dependencyInfo);
        }
        container.PreInstantiate();
        return container;
    }

}
