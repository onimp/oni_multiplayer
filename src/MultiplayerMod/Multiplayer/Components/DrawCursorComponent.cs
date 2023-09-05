using System.Collections.Generic;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Events;
using MultiplayerMod.Core.Extensions;
using MultiplayerMod.Core.Unity;
using MultiplayerMod.Multiplayer.Players;
using MultiplayerMod.Multiplayer.Players.Events;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.Components;

// TODO: Replace with a game object with Image and Canvas components and draw it on the world canvas
public class DrawCursorComponent : MultiplayerMonoBehaviour {

    [Dependency]
    private readonly EventDispatcher eventDispatcher = null!;

    private readonly Dictionary<MultiplayerPlayer, TemporalCursor> cursors = new();
    private Texture2D cursorTexture = null!;
    private Camera mainCamera = null!;
    private bool initialized;
    private EventSubscriptions subscriptions = null!;

    private void OnEnable() {
        subscriptions = new EventSubscriptions()
            .Add(eventDispatcher.Subscribe<PlayerCursorPositionUpdatedEvent>(OnCursorUpdated))
            .Add(eventDispatcher.Subscribe<PlayerLeftEvent>(OnPlayerLeft));
        cursorTexture = Assets.GetTexture("cursor_arrow");
        mainCamera = Camera.main!;
        initialized = true;
    }

    private void OnPlayerLeft(PlayerLeftEvent @event) => cursors.Remove(@event.Player);

    private void OnDisable() => subscriptions.Cancel();

    private void OnCursorUpdated(PlayerCursorPositionUpdatedEvent updatedEvent) {
        if (!cursors.TryGetValue(updatedEvent.Player, out var cursor)) {
            cursor = new TemporalCursor(updatedEvent.Position);
            cursors[updatedEvent.Player] = cursor;
            return;
        }
        cursor.Trace(updatedEvent.Position);
    }

    private void OnGUI() => cursors.ForEach(it => RenderCursor(it.Value.Position));

    private void RenderCursor(Vector2 position) {
        if (!initialized)
            return;

        var worldPos = new Vector3(position.x, position.y, 0);
        var screenPoint = mainCamera.WorldToScreenPoint(worldPos);

        var outOfView = screenPoint.x < -cursorTexture.width || screenPoint.x > Screen.width ||
                        screenPoint.y < 0 || screenPoint.y > Screen.height + cursorTexture.height;

        GUI.DrawTexture(
            new Rect(
                Mathf.Clamp(
                    screenPoint.x,
                    outOfView ? 0 : -cursorTexture.width,
                    outOfView ? Screen.width - cursorTexture.width : Screen.width
                ),
                Mathf.Clamp(
                    Screen.height - screenPoint.y,
                    outOfView ? 0 : -cursorTexture.height,
                    outOfView ? Screen.height - cursorTexture.height : Screen.height
                ),
                cursorTexture.width,
                cursorTexture.height
            ),
            cursorTexture,
            ScaleMode.ScaleToFit,
            alphaBlend: true,
            imageAspect: 0,
            color: new Color(1.0f, 1.0f, 1.0f, outOfView ? 0.4f : 1.0f),
            borderWidth: 0,
            borderRadius: 0
        );
    }

    private record TimedCursor(Vector2 Position, long Time);

    private class TemporalCursor {

        public Vector2 Position => GetCurrentPosition();

        private TimedCursor previous;
        private TimedCursor current;

        public TemporalCursor(Vector2 position) {
            var ticks = System.DateTime.Now.Ticks;
            previous = new TimedCursor(position, ticks);
            current = new TimedCursor(position, ticks);
        }

        public void Trace(Vector2 position) {
            previous = current;
            current = new TimedCursor(position, System.DateTime.Now.Ticks);
        }

        private Vector2 GetCurrentPosition() {
            float updateDelta = current.Time - previous.Time;
            var timeDiff = (System.DateTime.Now.Ticks - current.Time) / updateDelta;
            return Vector2.Lerp(previous.Position, current.Position, timeDiff);
        }

    }

}
