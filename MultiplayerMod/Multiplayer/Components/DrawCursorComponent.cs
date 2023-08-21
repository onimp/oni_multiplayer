using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Unity;
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

    private Texture2D cursorTexture = null!;
    private Camera mainCamera = null!;
    private bool initialized;

    private void OnEnable() {
        cursorTexture = Assets.GetTexture("cursor_arrow");
        mainCamera = Camera.main!;
        initialized = true;
    }

    private void OnGUI() {
        foreach (var (player, state) in multiplayer.State.Players) {
            if (player.Equals(client.Player))
                continue;

            var updateDelta = state.NewCursorTime - state.PrevCursorTime;
            var timeDiff = (Time.time - state.NewCursorTime) / updateDelta;
            var position = Vector2.Lerp(state.PrevCursorLocation, state.NewCursorLocation, timeDiff);
            RenderCursor(position);
        }
    }

    private void RenderCursor(Vector2 position) {
        if (!initialized) return;

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

}
