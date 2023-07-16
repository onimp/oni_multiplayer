using System;

namespace MultiplayerMod.Game.UI.Tools.Events;

[Serializable]
public record BuildEventArgs(
    int Cell,
    string PrefabId,
    bool InstantBuild,
    bool Upgrade,
    Orientation Orientation,
    Tag[] Materials,
    string FacadeId,
    PrioritySetting Priority
);
