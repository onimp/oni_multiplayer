using System;
using MultiplayerMod.Multiplayer.Objects.Reference;

namespace MultiplayerMod.Game.Mechanics;

[Serializable]
public record AccessControlEventArgs(
    GameObjectReference? MinionProxy,
    GameObjectReference Target,
    AccessControl.Permission? Permission
);
