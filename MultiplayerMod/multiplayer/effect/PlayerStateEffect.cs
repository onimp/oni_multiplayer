using MultiplayerMod.multiplayer.message;
using Steamworks;
using UnityEngine;

namespace MultiplayerMod.multiplayer.effect
{
    // TODO consider using IRenderEveryTick
    public class PlayerStateEffect : MonoBehaviour
    {
        public static PlayersState PlayerState { private get; set; }

        private Texture2D _texture;

        private void OnEnable()
        {
            _texture = Assets.GetTexture("cursor_arrow");
        }

        private void OnGUI()
        {
            if (PlayerState == null) return;
            foreach (var playerState in PlayerState.PlayerStates)
            {
                // Skip current user.
                if (playerState.Key == SteamUser.GetSteamID()) continue;
                var mousePos = playerState.Value.MousePosition;
                var worldPos = new Vector3(mousePos.first, mousePos.second, -1);
                var screenPoint = Camera.main.WorldToScreenPoint(worldPos);
                Graphics.DrawTexture(
                    new Rect(screenPoint.x, Screen.height - screenPoint.y, _texture.width, _texture.height), _texture);
            }
        }
    }
}