using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.Platform.Steam.Network.Messaging.Surrogates;
using UnityEngine;

namespace MultiplayerMod.Core.Patch;

public class PatchTargetResolver {

    private static readonly Logging.Logger log = LoggerFactory.GetLogger<PatchTargetResolver>();

    private readonly Dictionary<Type, List<string>> targets;
    private readonly IEnumerable<Type> baseTypes;
    private readonly Assembly assembly = Assembly.GetAssembly(typeof(global::Game));
    private bool checkArgumentsSerializable;

    private PatchTargetResolver(
        Dictionary<Type, List<string>> targets,
        IEnumerable<Type> baseTypes,
        bool checkArgumentsSerializable
    ) {
        this.targets = targets;
        this.baseTypes = baseTypes;
        this.checkArgumentsSerializable = checkArgumentsSerializable;
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
        if (methodInfo != null) {
            if (checkArgumentsSerializable)
                ValidateArguments(methodInfo);
            return methodInfo;
        }

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

    private void ValidateArguments(MethodBase? methodBase) {
        if (methodBase == null) return;

        var parameters = methodBase.GetParameters();
        foreach (var parameterInfo in parameters) {
            var paramType = parameterInfo.ParameterType;
            ValidateTypeIsSerializable(methodBase, paramType);
        }
    }

    private void ValidateTypeIsSerializable(MethodBase methodBase, Type checkType) {
        if (checkType.IsInterface) {
            var implementations = assembly.GetTypes()
                .Where(
                    type => type.IsClass && checkType.IsAssignableFrom(type)
                ).ToList();
            foreach (var implementation in implementations) {
                ValidateTypeIsSerializable(methodBase, implementation);
            }
            return;
        }
        if (checkType.IsEnum) {
            return;
        }
        var isTypeSerializable = checkType.IsDefined(typeof(SerializableAttribute), false);
        var isSurrogateExists = SerializationSurrogates.Selector.GetSurrogate(
            checkType,
            new StreamingContext(StreamingContextStates.All),
            out ISurrogateSelector _
        ) != null;
        var gameObjectOrKMono =
            checkType.IsSubclassOf(typeof(GameObject)) || checkType.IsSubclassOf(typeof(KMonoBehaviour));
        if (isTypeSerializable || isSurrogateExists || gameObjectOrKMono) return;

        var message = $"{checkType} is not serializable (method {methodBase}.";
        log.Error(message);
        throw new Exception(message);
    }

    public class Builder {

        private readonly Dictionary<Type, List<string>> targets = new();
        private readonly List<Type> baseTypes = new();
        private bool checkArgumentsSerializable;

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

        public Builder CheckArgumentsSerializable(bool check) {
            checkArgumentsSerializable = check;
            return this;
        }

        public PatchTargetResolver Build() => new(targets, baseTypes, checkArgumentsSerializable);

    }

}
