using System;
using MultiplayerMod.Multiplayer.Objects;

namespace MultiplayerMod.Game.Mechanics;

[Serializable]
public class DoorStateChangedEventArgs : EventArgs {
    public MultiplayerReference Target { get; set; }
    public Door.ControlState State { get; set; }
}
