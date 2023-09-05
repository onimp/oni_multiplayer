using System;

namespace MultiplayerMod.Multiplayer;

[AttributeUsage(AttributeTargets.Class)]
public class MultiplayerCommandAttribute : Attribute {
    public MultiplayerCommandType Type { get; set; } = MultiplayerCommandType.Gameplay;
}
