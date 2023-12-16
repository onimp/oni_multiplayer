using System;
using System.Collections.Generic;
using HarmonyLib;
using JetBrains.Annotations;

namespace MultiplayerMod.Test.Multiplayer.Commands.Chores.Patches;

[UsedImplicitly]
[HarmonyPatch(typeof(Assets))]
public class AssetsPatch {

    public static readonly Dictionary<string, KAnimFile> Cache = new();

    static AssetsPatch() {
        var batchGroupData = KAnimBatchManager.Instance().GetBatchGroupData(new HashedString("Fake"));
        batchGroupData.AddNewBuildFile(new KAnimHashedString());
        batchGroupData.builds[0].symbols = Array.Empty<KAnim.Build.Symbol>();
    }

    [UsedImplicitly]
    [HarmonyPrefix]
    [HarmonyPatch(nameof(Assets.GetAnim))]
    private static bool Assets_GetAnim(ref KAnimFile __result, HashedString name) {
        var key1 = name.ToString();
        var key2 = new HashedString(name.ToString()).ToString();
        if (!Cache.ContainsKey(key1) && !Cache.ContainsKey(key2)) {
            Cache[key1] = Cache[key2] = new KAnimFile() {
                name = key1,
                IsBuildLoaded = true,
                data = new KAnimFileData("") {
                    buildIndex = 0, batchTag = "Fake"
                }
            };
        }
        __result = Cache[key1];
        return false;
    }

    [UsedImplicitly]
    [HarmonyPrefix]
    [HarmonyPatch(nameof(Assets.GetTintedSprite))]
    private static bool Assets_GetTintedSprite(ref TintedSprite __result) {
        __result = new TintedSprite();
        return false;
    }

    [UsedImplicitly]
    [HarmonyPrefix]
    [HarmonyPatch(nameof(Assets.GetBlockTileDecorInfo))]
    private static bool Assets_BlockTileDecorInfo(ref BlockTileDecorInfo __result) {
        __result = new BlockTileDecorInfo();
        return false;
    }
}
