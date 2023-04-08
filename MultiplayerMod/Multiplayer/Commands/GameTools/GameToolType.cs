using System;

namespace MultiplayerMod.Multiplayer.Commands.GameTools;

[Serializable]
public enum GameToolType {
    Dig,
    Cancel,
    Deconstruct,
    Prioritize,
    Disinfect,
    Sweep,
    Attack,
    Mop,
    Wrangle,
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
