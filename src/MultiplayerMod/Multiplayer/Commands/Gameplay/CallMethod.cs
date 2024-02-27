using System;
using System.Reflection;
using MultiplayerMod.Game.Mechanics.Objects;
using MultiplayerMod.Multiplayer.Objects.Extensions;
using MultiplayerMod.Multiplayer.Objects.Reference;

namespace MultiplayerMod.Multiplayer.Commands.Gameplay;

[Serializable]
public class CallMethod : MultiplayerCommand {
    private readonly Reference target;
    private readonly Type methodType;
    private readonly string methodName;
    private readonly object?[] args;

    public CallMethod(ComponentEventsArgs eventArgs) {
        target = eventArgs.Component.GetReference();
        methodType = eventArgs.Method.GetType();
        methodName = eventArgs.Method.Name;
        args = ArgumentUtils.WrapObjects(eventArgs.Args);
    }

    public CallMethod(StateMachineEventsArgs eventArgs) {
        target = eventArgs.StateMachineInstance.GetReference();
        methodType = eventArgs.Method.GetType();
        methodName = eventArgs.Method.Name;
        args = ArgumentUtils.WrapObjects(eventArgs.Args);
    }

    public override void Execute(MultiplayerCommandContext context) {
        var method = methodType
            .GetMethod(
                methodName,
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance |
                BindingFlags.DeclaredOnly
            );
        var obj = target.ResolveRaw();
        if (obj != null) {
            method?.Invoke(obj, ArgumentUtils.UnWrapObjects(this.args));
        }
    }

    public override string ToString() => $"{base.ToString()} (Type = {methodType.FullName}, Method = {methodName})";

}
