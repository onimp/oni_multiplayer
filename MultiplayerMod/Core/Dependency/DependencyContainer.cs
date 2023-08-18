using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using MultiplayerMod.Core.Extensions;

namespace MultiplayerMod.Core.Dependency;

public class DependencyContainer {

    private readonly ConcurrentDictionary<Type, Lazy<object>> instances = new();

    public DependencyContainer() {
        Register(this);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Register<T>() where T : class => TryRegister<T>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Register<I, T>() where T : class, I => TryRegister<I, T>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Register<T>(T instance) where T : notnull => TryRegister(instance);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Register<I, T>(T instance) where T : class, I => TryRegister<I, T>(instance);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Resolve<T>() where T : notnull => (T) Resolve(typeof(T));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Inject<T>(T instance) where T : notnull => InjectDependencies(instance);

    public object Resolve(Type type) {
        instances.TryGetValue(type, out var instance);
        return instance == null ? Instantiate(type) : instance.Value;
    }

    public T Get<T>() where T : notnull {
        instances.TryGetValue(typeof(T), out var singleton);
        if (singleton == null)
            throw new MissingDependencyException($"Type {typeof(T)} is not registered");

        return (T) singleton.Value;
    }

    private void TryRegister<T>() where T : notnull {
        var initializer = new Lazy<object>(() => Instantiate(typeof(T)));
        RegisterInitializer(typeof(T), initializer);
    }

    private void TryRegister<I, T>() where T : class, I {
        var initializer = new Lazy<object>(() => Instantiate(typeof(T)));
        RegisterInitializer(typeof(I), initializer);
        RegisterInitializer(typeof(T), initializer);
    }

    private T TryRegister<T>(T instance) where T : notnull {
        RegisterInitializer(typeof(T), new Lazy<object>(() => instance));
        return instance;
    }

    private T TryRegister<I, T>(T instance) where T : class, I {
        var initializer = new Lazy<object>(() => instance);
        RegisterInitializer(typeof(I), initializer);
        RegisterInitializer(typeof(T), initializer);
        return instance;
    }

    private void RegisterInitializer(Type type, Lazy<object> initializer) {
        if (!instances.TryAdd(type, initializer))
            throw new TypeAlreadyRegisteredException(type);
    }

    private object Instantiate(Type type) {
        if (!type.IsClass)
            throw new DependencyContainerException($"A type {type} is not a class.");

        var constructors = type.GetConstructors();
        if (constructors.Length != 1)
            throw new DependencyContainerException($"A type {type} is expected to have one public constructor.");

        var constructor = constructors[0];
        var arguments = constructor.GetParameters()
            .Select(parameter => Resolve(parameter.ParameterType))
            .ToArray();

        return InjectDependencies(constructor.Invoke(arguments));
    }

    private T InjectDependencies<T>(T instance) where T : notnull {
        var type = instance.GetType();
        type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(property => property.GetCustomAttribute<DependencyAttribute>() != null)
            .ForEach(property => property.SetValue(instance, Resolve(property.PropertyType)));
        type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(field => field.GetCustomAttribute<DependencyAttribute>() != null)
            .ForEach(field => field.SetValue(instance, Resolve(field.FieldType)));
        return instance;
    }

}
