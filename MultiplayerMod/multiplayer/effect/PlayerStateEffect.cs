using System.Linq;
using MultiplayerMod.multiplayer.message;
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
            foreach (var mousePos in PlayerState.PlayerStates.Select(playerState => playerState.Value.MousePosition))
            {
                // TODO add transformation from world pos to screen pos.
                Graphics.DrawTexture(new Rect(mousePos.first, mousePos.second, _texture.width, _texture.height),
                    _texture);
            }
        }
    }
}