using System;
using MultiplayerMod.Multiplayer.Objects;
using MultiplayerMod.Multiplayer.Objects.Reference;

namespace MultiplayerMod.Game.Mechanics.Printing;

[Serializable]
public record AcceptDeliveryEventArgs(
    GameObjectReference Target,
    ITelepadDeliverable Deliverable,
    MultiplayerId MinionId,
    MultiplayerId ProxyId
);
