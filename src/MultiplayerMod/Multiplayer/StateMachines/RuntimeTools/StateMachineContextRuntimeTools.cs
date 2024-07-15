using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using MultiplayerMod.Core.Extensions;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.Game.NameOf;

namespace MultiplayerMod.Multiplayer.StateMachines.RuntimeTools;

public class StateMachineContextRuntimeTools {

    private static readonly Core.Logging.Logger log = LoggerFactory.GetLogger<StateMachineContextRuntimeTools>();

    private static readonly ConditionalWeakTable<StateMachine.Parameter.Context, StateMachineContextRuntimeTools>
        cache = new();

    private readonly Type supportedParameterType = typeof(StateMachine<,,,>.Parameter<>);

    private readonly StateMachine.Parameter.Context context;
    private readonly MethodInfo setMethod;

    private StateMachineContextRuntimeTools(StateMachine.Parameter.Context context) {
        this.context = context;
        setMethod = context.GetType().GetMethod(
            nameof(StateMachineMemberReference.Parameter.Context.Set),
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
        )!;
    }

    public bool Set(StateMachine.Instance instance, object? value) {
        var parameterType = context.parameter.GetType();
        var baseType = parameterType.BaseType;
        if (!parameterType.IsGenericType || baseType?.GetGenericTypeDefinition() != supportedParameterType) {
            log.Error($"Unable to set context value: unsupported parameter type {parameterType.GetSignature()}");
            return false;
        }
        var valueType = baseType.GenericTypeArguments.Last();
        if (value != null && valueType != value.GetType()) {
            log.Error(
                $"Unable to set context value: invalid value type {value.GetType().GetSignature()} ({valueType.GetSignature()} expected)"
            );
            return false;
        }
        setMethod.Invoke(context, [value, instance, false]);
        return true;
    }

    public static StateMachineContextRuntimeTools Get(StateMachine.Parameter.Context context) {
        if (cache.TryGetValue(context, out var tools))
            return tools;

        tools = new StateMachineContextRuntimeTools(context);
        cache.Add(context, tools);
        return tools;
    }

}
