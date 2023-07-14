using System;
using System.Reflection;
using MultiplayerMod.Game.Mechanics;

namespace MultiplayerMod.Multiplayer.Commands.Gameplay;

[Serializable]
public class CallMethod : IMultiplayerCommand {
    private readonly ObjectEvents.ObjectEventsArgs args;

    public CallMethod(ObjectEvents.ObjectEventsArgs args) {
        this.args = args;
    }

    public void Execute() {
        var method = args.MethodType
            .GetMethod(
                args.MethodName,
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance |
                BindingFlags.DeclaredOnly
            );
        var component = args.Target.GetComponent(args.MethodType);
        if (component != null) {
            method?.Invoke(component, args.Args);
        }
    }
}
