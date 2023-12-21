using System;
using System.Reflection;
using MultiplayerMod.Game.Mechanics.Objects;
using MultiplayerMod.Multiplayer.Objects;
using MultiplayerMod.Multiplayer.Objects.Reference;

namespace MultiplayerMod.Multiplayer.Commands.Gameplay;

[Serializable]
public class CallMethod : MultiplayerCommand {
    private readonly ComponentReference? componentTarget;
    private readonly StateMachineReference? stateMachineTarget;
    private readonly Type methodType;
    private readonly string methodName;
    private readonly object?[] args;

    public CallMethod(ComponentEventsArgs eventArgs) {
        componentTarget = eventArgs.Component.GetReference();
        methodType = eventArgs.Method.GetType();
        methodName = eventArgs.Method.Name;
        args = ArgumentUtils.WrapObjects(eventArgs.Args);
    }

    public CallMethod(StateMachineEventsArgs eventArgs) {
        stateMachineTarget = eventArgs.StateMachineInstance.GetReference();
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
        object? obj = componentTarget != null ? componentTarget.GetComponent() : stateMachineTarget!.GetInstance();
        if (obj != null) {
            method?.Invoke(obj, ArgumentUtils.UnWrapObjects(this.args));
        }
    }

    public override string ToString() => $"{base.ToString()} (Type = {methodType.FullName}, Method = {methodName})";

}
