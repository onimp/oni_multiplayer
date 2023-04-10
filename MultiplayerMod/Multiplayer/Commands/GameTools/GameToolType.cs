using System;

namespace MultiplayerMod.Multiplayer.Commands.GameTools;

[Serializable]
public enum GameToolType {
    Dig,
    Cancel,
    Deconstruct,
    Prioritize,
    Disinfect,
    Sweep, // Clear
    Attack,
    Mop,
    Wrangle, // Capture
    Harvest,
    EmptyPipe,
    Disconnect,

    Build,
    UtilityBuild,
    WireBuild,
    CopySettings,
    Debug,
    Place
}
