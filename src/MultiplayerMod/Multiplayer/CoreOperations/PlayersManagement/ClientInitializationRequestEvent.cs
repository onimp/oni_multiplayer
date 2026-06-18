using MultiplayerMod.Core.Events;
using MultiplayerMod.Multiplayer.Compatibility;
using MultiplayerMod.Multiplayer.Players;
using MultiplayerMod.Network;

namespace MultiplayerMod.Multiplayer.CoreOperations.PlayersManagement;

public record ClientInitializationRequestEvent(
    IMultiplayerClientId ClientId,
    PlayerProfile Profile,
    CompatibilityFingerprint Compatibility
) : IDispatchableEvent;
