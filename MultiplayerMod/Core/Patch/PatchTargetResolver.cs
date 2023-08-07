using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MultiplayerMod.Core.Logging;

namespace MultiplayerMod.Core.Patch;

public class PatchTargetResolver {

    private static readonly Logging.Logger log = LoggerFactory.GetLogger<PatchTargetResolver>();

    private readonly Dictionary<Type, List<string>> targets;
    private readonly IEnumerable<Type> baseTypes;
    private readonly Assembly assembly = Assembly.GetAssembly(typeof(global::Game));

    public PatchTargetResolver(Dictionary<Type, List<string>> targets, IEnumerable<Type> baseTypes) {
        this.targets = targets;
        this.baseTypes = baseTypes;
    }

    public IEnumerable<MethodBase> Resolve() {
        var classTypes = targets.Keys.Where(type => type.IsClass).ToList();
        var interfaceTypes = targets.Keys.Where(type => type.IsInterface).ToList();
        return assembly.GetTypes()
            .Where(
                type => type.IsClass && (classTypes.Contains(type)
                                         || interfaceTypes.Any(interfaceType => interfaceType.IsAssignableFrom(type)))
            )
            .Where(
                type => {
                    if (!baseTypes.Any())
                        return true;

                    var assignable = baseTypes.Any(it => it.IsAssignableFrom(type));
                    if (!assignable)
                        log.Warning(
                            $"{type} can not be assigned to any of " +
                            $"{string.Join(", ", baseTypes.Select(it => it.Name))}."
                        );
                    return assignable;
                }
            )
            .SelectMany(
                type => {
                    if (classTypes.Contains(type))
                        return targets[type].Select(methodName => GetMethodOrSetter(type, methodName, null));

                    var implementedInterfaces = GetImplementedInterfaces(interfaceTypes, type);
                    return implementedInterfaces.SelectMany(
                        implementedInterface => targets[implementedInterface].Select(
                            methodName => GetMethodOrSetter(type, methodName, implementedInterface)
                        )
                    );
                }
            ).ToList();
    }

    private MethodBase GetMethodOrSetter(Type type, string methodName, Type? interfaceType) {
        var methodInfo = GetMethod(type, methodName, interfaceType);
        if (methodInfo != null)
            return methodInfo;

        var property = GetSetter(type, methodName, interfaceType);
        if (property != null)
            return property;

        var message = $"Method {type}.{methodName} ({interfaceType}) not found";
        log.Error(message);
        throw new Exception(message);
    }

    private MethodBase? GetMethod(Type type, string methodName, Type? interfaceType) {
        var methodInfo = type.GetMethod(
            methodName,
            BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance
        );
        if (methodInfo != null)
            return methodInfo;

        if (interfaceType == null)
            return null;

        // Some overrides names prefixed by interface e.g. Clinic#ISliderControl.SetSliderValue
        methodInfo = type.GetMethod(
            interfaceType.Name + "." + methodName,
            BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance
        );
        return methodInfo;
    }

    private MethodBase? GetSetter(Type type, string propertyName, Type? interfaceType) {
        var property = type.GetProperty(
            propertyName,
            BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance
        );
        if (property != null)
            return property.GetSetMethod(true);

        if (interfaceType == null)
            return null;

        // Some overrides names prefixed by interface e.g. Clinic#ISliderControl.SetSliderValue
        property = type.GetProperty(
            interfaceType.Name + "." + propertyName,
            BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance
        );
        return property?.GetSetMethod(true);
    }

    private List<Type> GetImplementedInterfaces(IEnumerable<Type> interfaceTypes, Type type) => interfaceTypes
        .Where(interfaceType => interfaceType.IsAssignableFrom(type))
        .ToList();

    public class Builder {

        private readonly Dictionary<Type, List<string>> targets = new();
        private readonly List<Type> baseTypes = new();

        private List<string> GetTargets(Type type) {
            if (targets.TryGetValue(type, out var methods))
                return methods;

            methods = new List<string>();
            targets[type] = methods;
            return methods;
        }

        public Builder AddMethods(Type type, params string[] methods) {
            GetTargets(type).AddRange(methods);
            return this;
        }

        public Builder AddBaseType(Type type) {
            baseTypes.Add(type);
            return this;
        }

        public PatchTargetResolver Build() => new(targets, baseTypes);

    }

}
