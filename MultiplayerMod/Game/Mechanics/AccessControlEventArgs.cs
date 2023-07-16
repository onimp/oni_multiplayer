using System;
using MultiplayerMod.Multiplayer.Objects.Reference;

namespace MultiplayerMod.Game.Mechanics;

[Serializable]
public class AccessControlEventArgs : EventArgs {
    public GameObjectReference? MinionProxy { get; }
    public GameObjectReference Target { get; }
    public AccessControl.Permission? Permission { get; }

    public AccessControlEventArgs(
        GameObjectReference? minionProxy,
        GameObjectReference target,
        AccessControl.Permission? permission
    ) {
        MinionProxy = minionProxy;
        Target = target;
        Permission = permission;
    }
}
