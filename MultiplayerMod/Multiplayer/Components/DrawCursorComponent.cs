using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Multiplayer.State;
using MultiplayerMod.Network;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.Components;

// TODO: Replace with a game object with Image and Canvas components and draw it on the world canvas
public class DrawCursorComponent : MonoBehaviour {

    private readonly IMultiplayerClient client = Container.Get<IMultiplayerClient>();
    private Texture2D cursorTexture;
    private Camera mainCamera;

    private void OnEnable() {
        cursorTexture = Assets.GetTexture("cursor_arrow");
        mainCamera = Camera.main;
    }

    private void OnGUI() {
        foreach (var (player, state) in MultiplayerState.Shared.Players) {
            if (player.Equals(client.Player))
                continue;

            RenderCursor(state.CursorPosition);
        }
    }

    private void RenderCursor(Vector2 position) {
        var worldPos = new Vector3(position.x, position.y, 0);
        var screenPoint = mainCamera.WorldToScreenPoint(worldPos);
        Graphics.DrawTexture(
            new Rect(screenPoint.x, Screen.height - screenPoint.y, cursorTexture.width, cursorTexture.height),
            cursorTexture
        );
    }

}
