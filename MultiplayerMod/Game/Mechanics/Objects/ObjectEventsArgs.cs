using System;
using MultiplayerMod.Multiplayer.Objects.Reference;

namespace MultiplayerMod.Game.Mechanics.Objects;

[Serializable]
public class ObjectEventsArgs {
    public GameObjectReference Target { get; }

    public Type MethodType { get; }
    public string MethodName { get; }
    public object[] Args { get; }

    public ObjectEventsArgs(GameObjectReference target, Type methodType, string methodName, object[] args) {
        Target = target;
        MethodType = methodType;
        MethodName = methodName;
        Args = args;
    }
}
