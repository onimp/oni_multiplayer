using HarmonyLib;
using UnityEngine;

namespace MultiplayerMod.patch
{
    [HarmonyPatch(typeof(WorldGenSpawner), "OnSpawn")]
    public class CursorPatch
    {
        private class CursorDrawer : MonoBehaviour
        {
            private Texture2D texture;

            private void OnGUI()
            {
                // Mouse move
                // Need to send cursor pos to other participants/server
                texture = Assets.GetTexture("cursor_arrow");

                var otherPos = new Vector2(100 / 1.1f, 100f);

                Graphics.DrawTexture(
                    new Rect(otherPos,
                        new Vector2(texture.width, texture.height)), texture);
            }
        }

        public static void Postfix()
        {
            var go = new GameObject();
            go.AddComponent<CursorDrawer>();
        }
    }
}