using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MultiplayerMod.Core.Logging;

namespace MultiplayerMod.Game.Mechanics.Objects;

static class TargetExtractor {

    private static readonly Core.Logging.Logger log = LoggerFactory.GetLogger(typeof(TargetExtractor));

    public static IEnumerable<MethodBase> GetTargetMethods(Dictionary<Type, string[]> methodsForPatch, int argsCount) {
        var classTypes = methodsForPatch.Keys.Where(type => type.IsClass).ToList();
        var interfaceTypes = methodsForPatch.Keys.Where(type => type.IsInterface).ToList();
        var targetMethods = Assembly.GetAssembly(typeof(ISliderControl))
            .GetTypes()
            .Where(
                type => type.IsClass && (classTypes.Contains(type)
                                         || interfaceTypes.Any(interfaceType => interfaceType.IsAssignableFrom(type)))
            )
            .SelectMany(
                type => {
                    Type interfaceType = null;
                    if (!classTypes.Contains(type)) {
                        interfaceType =
                            interfaceTypes.Single(interfaceType => interfaceType.IsAssignableFrom(type));
                    }
                    var methodNames = interfaceType != null
                        ? methodsForPatch[interfaceType]
                        : methodsForPatch[type];
                    return methodNames.Select(methodName => GetMethodOrSetter(type, methodName, interfaceType));
                }
            )
            .Where(method => method.GetParameters().Length == argsCount)
            .ToList();
        return targetMethods;
    }

    private static MethodBase GetMethodOrSetter(Type type, string methodName, Type interfaceType) {
        var methodInfo = GetMethod(type, methodName, interfaceType);
        if (methodInfo != null) return methodInfo;

        var property = type.GetProperty(
            methodName,
            BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance
        );
        if (property != null) return property.GetSetMethod();

        log.Error($"Method {type}.{methodName} not found");
        return null;
    }

    private static MethodBase GetMethod(Type type, string methodName, Type interfaceType) {
        var methodInfo = type.GetMethod(
            methodName,
            BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance
        );
        if (methodInfo != null) return methodInfo;

        if (interfaceType == null) return null;

        // Some overrides names prefixed by interface e.g. Clinic#ISliderControl.SetSliderValue
        methodInfo = type.GetMethod(
            interfaceType.Name + "." + methodName,
            BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance
        );
        return methodInfo;
    }
}
