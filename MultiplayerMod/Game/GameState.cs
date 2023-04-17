using HarmonyLib;
using UnityEngine;

namespace MultiplayerMod.Game;

[HarmonyPatch]
public static class GameState {

    public static GameObject LastSelectedObject { get; private set; }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(global::Game), nameof(global::Game.OnPrefabInit))]
    private static void OnGamePrefabInit(global::Game __instance) {
        __instance.Subscribe(
            (int) GameHashes.SelectObject,
            o => {
                if (o != null)
                    LastSelectedObject = (GameObject) o;
            }
        );
    }

}
