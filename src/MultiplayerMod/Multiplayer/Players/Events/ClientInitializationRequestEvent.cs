using MultiplayerMod.Core.Events;
using MultiplayerMod.Network;

namespace MultiplayerMod.Multiplayer.Players.Events;

public record ClientInitializationRequestEvent(
    IMultiplayerClientId ClientId,
    PlayerProfile Profile
) : IDispatchableEvent;
