using System;
using MultiplayerMod.Network;

namespace MultiplayerMod.Multiplayer.Commands;

[AttributeUsage(AttributeTargets.Class)]
public class MultiplayerCommandAttribute : Attribute {
    public MultiplayerCommandType Type { get; set; } = MultiplayerCommandType.Game;
    public bool ExecuteOnServer { get; set; }

    /// <summary>
    /// The Steam priority lane this command's traffic rides on. Defaults to
    /// <see cref="NetworkLane.Gameplay"/> so unmarked commands stay on the fast, reliable lane.
    /// </summary>
    public NetworkLane Lane { get; set; } = NetworkLane.Gameplay;
}
