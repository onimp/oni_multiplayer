using System;
using System.Linq;
using System.Reflection;
using MultiplayerMod.Game.Mechanics.Objects;
using MultiplayerMod.Multiplayer.Objects.Extensions;
using MultiplayerMod.Multiplayer.Objects.Reference;

namespace MultiplayerMod.Multiplayer.Commands.Gameplay;

[Serializable]
public class CallMethod : MultiplayerCommand {

    private const BindingFlags MethodBindingFlags =
        BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;

    private readonly Reference target;
    private readonly Type declaringType;
    private readonly string methodName;
    private readonly Type[] parameterTypes;
    private readonly object?[] args;

    public CallMethod(ComponentEventsArgs eventArgs) {
        target = eventArgs.Component.GetReference();
        declaringType = eventArgs.Method.DeclaringType!;
        methodName = eventArgs.Method.Name;
        parameterTypes = eventArgs.Method.GetParameters().Select(it => it.ParameterType).ToArray();
        args = ArgumentUtils.WrapObjects(eventArgs.Args);
    }

    public CallMethod(StateMachineEventsArgs eventArgs) {
        target = eventArgs.StateMachineInstance.GetReference();
        declaringType = eventArgs.Method.DeclaringType!;
        methodName = eventArgs.Method.Name;
        parameterTypes = eventArgs.Method.GetParameters().Select(it => it.ParameterType).ToArray();
        args = ArgumentUtils.WrapObjects(eventArgs.Args);
    }

    public override void Execute(MultiplayerCommandContext context) {
        // Resolve by exact signature: the plain GetMethod(name, flags) overload throws
        // AmbiguousMatchException when the target method is overloaded (several methods sharing a name
        // on the same type), which is not an ObjectNotFoundException and so crashes the whole command
        // batch. Matching on the captured parameter types picks the exact overload.
        var method = declaringType.GetMethod(methodName, MethodBindingFlags, null, parameterTypes, null);
        var obj = target.Resolve();
        if (obj != null)
            method?.Invoke(obj, ArgumentUtils.UnWrapObjects(args));
    }

    public override string ToString() => $"{base.ToString()} (Type = {declaringType.FullName}, Method = {methodName})";

}
