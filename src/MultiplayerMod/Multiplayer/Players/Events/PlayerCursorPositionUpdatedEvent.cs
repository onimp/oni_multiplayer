using MultiplayerMod.Core.Events;
using MultiplayerMod.Game.UI.Tools.Events;

namespace MultiplayerMod.Multiplayer.Players.Events;

public record PlayerCursorPositionUpdatedEvent(
    MultiplayerPlayer Player,
    InterfaceToolEvents.MouseMovedEventArgs MouseMovedEventArgs
) : IDispatchableEvent;
