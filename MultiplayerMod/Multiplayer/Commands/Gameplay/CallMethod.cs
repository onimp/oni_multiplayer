using System;
using System.Linq;
using System.Reflection;
using MultiplayerMod.Game.Mechanics.Objects;
using MultiplayerMod.Multiplayer.Objects.Reference;

namespace MultiplayerMod.Multiplayer.Commands.Gameplay;

[Serializable]
public class CallMethod : IMultiplayerCommand {
    private readonly ObjectEventsArgs eventArgs;

    public CallMethod(ObjectEventsArgs eventArgs) {
        this.eventArgs = eventArgs;
    }

    public void Execute() {
        var method = eventArgs.MethodType
            .GetMethod(
                eventArgs.MethodName,
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance |
                BindingFlags.DeclaredOnly
            );
        var args = eventArgs.Args.Select(
            arg =>
                arg switch {
                    GameObjectReference gameObjectReference => gameObjectReference.GetGameObject(),
                    ComponentReference reference => reference.GetComponent(),
                    _ => arg
                }
        ).ToArray();
        var component = eventArgs.Target.GetComponent();
        if (component != null) {
            method?.Invoke(component, args);
        }
    }
}
