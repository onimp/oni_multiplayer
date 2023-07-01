using HarmonyLib;
using MultiplayerMod.Game.Extension;
using UnityEngine;

namespace MultiplayerMod.Game.Location;

// ReSharper disable once UnusedType.Global
[HarmonyPatch(typeof(Grid.ObjectLayerIndexer))]
public static class GridObjectLayerIndexerPatch {

    // ReSharper disable once UnusedMember.Local
    [HarmonyPostfix]
    [HarmonyPatch("set_Item")]
    private static void SetItemPostfix(int cell, int layer, GameObject value) {
        if (value == null)
            return;

        var extension = value.GetComponent<GameObjectExtension>();
        if (extension == null)
            return;

        extension.GridLayer = layer;
    }

}
