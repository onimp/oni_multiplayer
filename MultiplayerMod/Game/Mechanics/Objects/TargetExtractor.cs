using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MultiplayerMod.Core.Logging;

namespace MultiplayerMod.Game.Mechanics.Objects;

static class TargetExtractor {

    private static readonly Core.Logging.Logger log = LoggerFactory.GetLogger(typeof(TargetExtractor));

    public static IEnumerable<MethodBase> GetTargetMethods(Dictionary<Type, string[]> methodsForPatch) {
        var classTypes = methodsForPatch.Keys.Where(type => type.IsClass).ToList();
        var interfaceTypes = methodsForPatch.Keys.Where(type => type.IsInterface).ToList();
        var targetMethods = Assembly.GetAssembly(typeof(ISliderControl))
            .GetTypes()
            .Where(
                type => type.IsClass && (classTypes.Contains(type)
                                         || interfaceTypes.Any(interfaceType => interfaceType.IsAssignableFrom(type)))
            )
            .Where(
                type => {
                    var isAssignableFrom = typeof(KMonoBehaviour).IsAssignableFrom(type) ||
                                           typeof(StateMachine.Instance).IsAssignableFrom(type);
                    if (!isAssignableFrom) {
                        log.Error($"{type} can not be assigned to KMonoBehaviour or StateMachine.Instance.");
                    }
                    return isAssignableFrom;
                }
            )
            .SelectMany(
                type => {
                    if (classTypes.Contains(type))
                        return methodsForPatch[type].Select(methodName => GetMethodOrSetter(type, methodName, null));

                    var implementedInterfaces = GetImplementedInterfaces(interfaceTypes, type);
                    return implementedInterfaces.SelectMany(
                        implementedInterface => methodsForPatch[implementedInterface].Select(
                            methodName => GetMethodOrSetter(type, methodName, implementedInterface)
                        )
                    );
                }
            ).ToList();
        return targetMethods;
    }

    private static MethodBase GetMethodOrSetter(Type type, string methodName, Type? interfaceType) {
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

    private static MethodBase? GetMethod(Type type, string methodName, Type? interfaceType) {
        var methodInfo = type.GetMethod(
            methodName,
            BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance
        );
        if (methodInfo != null)
            return methodInfo;

        if (interfaceType == null) return null;

        // Some overrides names prefixed by interface e.g. Clinic#ISliderControl.SetSliderValue
        methodInfo = type.GetMethod(
            interfaceType.Name + "." + methodName,
            BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance
        );
        return methodInfo;
    }

    private static MethodBase? GetSetter(Type type, string propertyName, Type? interfaceType) {
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

    private static List<Type> GetImplementedInterfaces(List<Type> interfaceTypes, Type type) {
        return interfaceTypes.Where(interfaceType => interfaceType.IsAssignableFrom(type)).ToList();
    }

}
