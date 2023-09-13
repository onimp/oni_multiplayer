using MultiplayerMod.Core.Events;
using MultiplayerMod.Multiplayer.Players;

namespace MultiplayerMod.Multiplayer.CoreOperations.PlayersManagement;

public record CurrentPlayerInitializedEvent(MultiplayerPlayer Player) : IDispatchableEvent;
