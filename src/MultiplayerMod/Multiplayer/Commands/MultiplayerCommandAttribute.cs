using System;

namespace MultiplayerMod.Multiplayer.Commands;

[AttributeUsage(AttributeTargets.Class)]
public class MultiplayerCommandAttribute : Attribute {
    public MultiplayerCommandType Type { get; set; } = MultiplayerCommandType.Game;
    public bool ExecuteOnServer { get; set; }
}
