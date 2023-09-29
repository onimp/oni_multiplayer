using System.Collections.Generic;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Events;
using MultiplayerMod.Core.Unity;
using MultiplayerMod.Multiplayer.Players;
using MultiplayerMod.Multiplayer.Players.Events;

namespace MultiplayerMod.Multiplayer.Components;

public class CursorManager : MultiplayerMonoBehaviour {

    [InjectDependency] private readonly EventDispatcher eventDispatcher = null!;

    private readonly Dictionary<MultiplayerPlayer, CursorComponent> cursors = new();
    private EventSubscriptions subscriptions = null!;

    private void OnEnable() {
        subscriptions = new EventSubscriptions()
            .Add(eventDispatcher.Subscribe<PlayerCursorPositionUpdatedEvent>(OnCursorUpdated))
            .Add(eventDispatcher.Subscribe<PlayerLeftEvent>(OnPlayerLeft));
    }

    private void OnPlayerLeft(PlayerLeftEvent @event) {
        Destroy(cursors[@event.Player]);
        cursors.Remove(@event.Player);
    }

    private void OnDisable() => subscriptions.Cancel();

    private void OnCursorUpdated(PlayerCursorPositionUpdatedEvent updatedEvent) {
        if (!cursors.TryGetValue(updatedEvent.Player, out var cursor)) {
            cursor = gameObject.AddComponent<CursorComponent>();
            cursor.Position = updatedEvent.Position;
            cursor.CursorText = updatedEvent.Player.Profile.PlayerName;
            cursors[updatedEvent.Player] = cursor;
            return;
        }
        cursor.Trace(updatedEvent.Position);
    }

}
