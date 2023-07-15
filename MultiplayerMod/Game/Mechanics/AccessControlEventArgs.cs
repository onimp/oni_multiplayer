using System;
using MultiplayerMod.Multiplayer.Objects.Reference;

namespace MultiplayerMod.Game.Mechanics;

[Serializable]
public class AccessControlEventArgs : EventArgs {
    public GameObjectReference MinionProxy { get; set; }
    public GameObjectReference Target { get; set; }
    public AccessControl.Permission? Permission { get; set; }
}
