using System.Collections.Generic;
using System.Linq;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Events;
using MultiplayerMod.Core.Extensions;
using MultiplayerMod.Core.Unity;
using MultiplayerMod.Multiplayer.Commands.State;
using MultiplayerMod.Multiplayer.State;
using MultiplayerMod.Network;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.Components;

// TODO: Replace with a game object with Image and Canvas components and draw it on the world canvas
public class DrawCursorComponent : MultiplayerMonoBehaviour {

    [Dependency]
    private readonly IMultiplayerClient client = null!;

    [Dependency]
    private readonly MultiplayerGame multiplayer = null!;

    [Dependency]
    private readonly EventDispatcher eventDispatcher = null!;

    private readonly Dictionary<IMultiplayerClientId, TemporalCursor> cursors = new();
    private Texture2D cursorTexture = null!;
    private Camera mainCamera = null!;
    private bool initialized;
    private EventSubscription cursorUpdateSubscription = null!;

    private void OnEnable() {
        cursorUpdateSubscription = eventDispatcher.Subscribe<UpdateCursorPositionEvent>(OnCursorUpdated);
        cursorTexture = Assets.GetTexture("cursor_arrow");
        mainCamera = Camera.main!;
        initialized = true;
    }

    private void OnDisable() {
        cursorUpdateSubscription.Cancel();
    }

    private void OnCursorUpdated(UpdateCursorPositionEvent @event) {
        if (@event.Player.Equals(client.Id))
            return;

        if (!cursors.TryGetValue(@event.Player, out var cursor)) {
            cursor = new TemporalCursor(@event.Position);
            cursors[@event.Player] = cursor;
            return;
        }
        cursor.Trace(@event.Position);
    }

    private void OnGUI() {
        // TODO: Replace alive check with player events when ready
        CheckAlive();
        cursors.ForEach(it => RenderCursor(it.Value.Position));
    }

    private void CheckAlive() {
        new List<IMultiplayerClientId>(cursors.Keys)
            .Where(player => !multiplayer.State.Players.ContainsKey(player))
            .ForEach(player => cursors.Remove(player));
    }

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
