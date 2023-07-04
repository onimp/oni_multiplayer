using System;
using MultiplayerMod.Multiplayer.Objects;

namespace MultiplayerMod.Game.Mechanics;

[Serializable]
public class AccessControlEventArgs : EventArgs {
    public MultiplayerReference MinionProxy { get; set; }
    public MultiplayerReference Target { get; set; }
    public AccessControl.Permission? Permission { get; set; }
}
