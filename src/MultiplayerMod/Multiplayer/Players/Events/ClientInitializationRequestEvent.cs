using System;
using MultiplayerMod.Network;

namespace MultiplayerMod.Multiplayer.Players.Events;

[Serializable]
public record ClientInitializationRequestEvent(IMultiplayerClientId ClientId, PlayerProfile Profile);
