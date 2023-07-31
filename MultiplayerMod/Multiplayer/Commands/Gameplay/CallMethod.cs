using System;
using System.Linq;
using System.Reflection;
using MultiplayerMod.Game.Mechanics.Objects;
using MultiplayerMod.Multiplayer.Objects.Reference;

namespace MultiplayerMod.Multiplayer.Commands.Gameplay;

[Serializable]
public class CallMethod : IMultiplayerCommand {
    private readonly ComponentReference? componentTarget;
    private readonly StateMachineReference? stateMachineTarget;
    private readonly Type methodType;
    private readonly string methodName;
    private readonly object[] args;

    public CallMethod(ComponentEventsArgs eventArgs) {
        componentTarget = eventArgs.Target;
        methodType = eventArgs.MethodType;
        methodName = eventArgs.MethodName;
        args = eventArgs.Args;
    }

    public CallMethod(StateMachineEventsArgs eventArgs) {
        stateMachineTarget = eventArgs.Target;
        methodType = eventArgs.MethodType;
        methodName = eventArgs.MethodName;
        args = eventArgs.Args;
    }

    public void Execute() {
        var method = methodType
            .GetMethod(
                methodName,
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance |
                BindingFlags.DeclaredOnly
            );
        var args = this.args.Select(
            arg =>
                arg switch {
                    GameObjectReference gameObjectReference => gameObjectReference.GetGameObject(),
                    ComponentReference reference => reference.GetComponent(),
                    _ => arg
                }
        ).ToArray();
        object? obj = componentTarget != null ? componentTarget.GetComponent() : stateMachineTarget!.GetInstance();
        if (obj != null) {
            method?.Invoke(obj, args);
        }
    }
}
