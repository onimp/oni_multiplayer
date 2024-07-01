using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MultiplayerMod.Core.Collections;
using MultiplayerMod.Core.Extensions;
using MultiplayerMod.Core.Logging;

namespace MultiplayerMod.Core.Dependency;

public class DependencyContainer : IDependencyContainer, IDependencyInjector {

    private readonly Logging.Logger log = LoggerFactory.GetLogger<DependencyContainer>();

    private readonly Dictionary<string, DependencyInfo> dependencies = new();
    private readonly Dictionary<string, object> instances = new();
    private readonly HashSet<string> instantiatingInstances = new();
    private readonly Dictionary<Type, List<DependencyInfo>> dependenciesByType = new();

    private const BindingFlags injectionMemberBindingFlags =
        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

    private const BindingFlags injectionStaticMemberBindingFlags =
        BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

    public IEnumerable<DependencyInfo> RegisteredDependencies => dependencies.Values;

    public void Register(DependencyInfo info) {
        if (dependencies.ContainsKey(info.Name))
            throw new DependencyAlreadyRegisteredException(info.Name);

        log.Trace(() => $"Registering dependency \"{info.Name}\"...");
        dependencies[info.Name] = info;
        ResolveDependencyTypes(info.Type).ForEach(it => RegisterDependencyByType(it, info));
    }

    private IEnumerable<Type> ResolveDependencyTypes(Type type) {
        var result = new List<Type> { type };
        var baseType = type.BaseType;
        while (baseType != null && baseType != typeof(object)) {
            result.Add(baseType);
            baseType = baseType.BaseType;
        }
        result.AddRange(type.GetInterfaces());
        return result;
    }

    private void RegisterDependencyByType(Type type, DependencyInfo info) {
        if (!dependenciesByType.TryGetValue(type, out var dependenceInfos)) {
            dependenceInfos = new List<DependencyInfo>();
            dependenciesByType[type] = dependenceInfos;
        }
        log.Trace(() => $"Mapping dependency \"{info.Name}\" to type \"{type}\"...");
        dependenceInfos.Add(info);
    }

    public void RegisterSingleton(object singleton) => RegisterSingleton(singleton.GetType().FullName!, singleton);

    public void RegisterSingleton(string name, object singleton) {
        var info = new DependencyInfo(name, singleton.GetType(), false);
        Register(info);
        instances[info.Name] = singleton;
    }

    public T Get<T>() where T : notnull => (T) Get(typeof(T));

    public Deconstructable<T1, T2> Get<T1, T2>() where T1 : notnull where T2 : notnull => new(Get<T1>(), Get<T2>());

    public object Get(Type type) {
        var targetType = type;
        var resolveMultiple = type.IsGenericType && type.GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>));
        if (resolveMultiple)
            targetType = type.GetGenericArguments()[0];

        if (!dependenciesByType.TryGetValue(targetType, out var dependencyInfos)) {
            if (resolveMultiple)
                return Activator.CreateInstance(typeof(List<>).MakeGenericType(targetType));
            throw new MissingDependencyException(targetType);
        }

        if (!resolveMultiple && dependencyInfos.Count > 1)
            throw new AmbiguousDependencyException(targetType, dependencyInfos);

        return resolveMultiple
            ? dependencyInfos.Select(GetInstance).ToTypedList(targetType)
            : GetInstance(dependencyInfos[0]);
    }

    public void PreInstantiate() => RegisteredDependencies.Where(it => !it.Lazy).ForEach(it => GetInstance(it));

    public T Inject<T>(T instance) where T : notnull {
        var type = instance.GetType();
        InjectMembers(type, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, instance);
        return instance;
    }

    public void Inject(Type type) => InjectMembers(
        type,
        BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic,
        instance: null
    );

    private void InjectMembers(Type type, BindingFlags bindingFlags, object? instance) => type.GetMembers(bindingFlags)
        .Where(member => member.GetCustomAttribute<InjectDependencyAttribute>() != null)
        .ForEach(member => {
            switch (member) {
                case FieldInfo field:
                    field.SetValue(instance, Get(field.FieldType));
                    break;
                case PropertyInfo property:
                    if (!property.CanWrite)
                        break;
                    property.SetValue(instance, Get(property.PropertyType));
                    break;
            }
        });

    private object GetInstance(DependencyInfo info) {
        if (instances.TryGetValue(info.Name, out var instance))
            return instance;

        instance = Instantiate(info);
        instances[info.Name] = instance;
        return instance;
    }

    private object Instantiate(DependencyInfo info) {
        if (instantiatingInstances.Contains(info.Name))
            throw new DependencyIsInstantiatingException(info);

        var constructors = info.Type.GetConstructors();
        if (constructors.Length != 1)
            throw new InvalidDependencyException($"\"{info.Type}\" is expected to have one public constructor.");

        var constructor = constructors[0];

        instantiatingInstances.Add(info.Name);
        try {
            var arguments = constructor.GetParameters()
                .Select(parameter => Get(parameter.ParameterType))
                .ToArray();
            return constructor.Invoke(arguments);
        } catch (Exception exception) {
            throw new DependencyInstantiationException(info, exception);
        } finally {
            instantiatingInstances.Remove(info.Name);
        }
    }

}
