using System;
using System.Collections.Generic;
using HarmonyLib;
using JetBrains.Annotations;

namespace MultiplayerMod.Test.Multiplayer.Commands.Chores.Patches;

[UsedImplicitly]
[HarmonyPatch(typeof(Assets))]
public class AssetsPatch {

    private static readonly Dictionary<string, KAnimFile> cache = new();

    private static bool initialized;

    private static void Init() {
        var batchGroupData = KAnimBatchManager.Instance().GetBatchGroupData(new HashedString("Fake"));
        var hash = new KAnimHashedString();
        batchGroupData.textureStartIndex.Remove(hash);
        batchGroupData.AddNewBuildFile(hash);
        batchGroupData.builds[0].symbols = Array.Empty<KAnim.Build.Symbol>();
        initialized = true;
    }

    public static void Clear() {
        initialized = false;
        cache.Clear();
    }

    [UsedImplicitly]
    [HarmonyPrefix]
    [HarmonyPatch(nameof(Assets.GetAnim))]
    private static bool Assets_GetAnim(ref KAnimFile __result, HashedString name) {
        var key1 = name.ToString();
        var key2 = new HashedString(name.ToString()).ToString();
        if (!initialized) {
            Init();
        }
        if (!cache.ContainsKey(key1) && !cache.ContainsKey(key2)) {
            cache[key1] = cache[key2] = new KAnimFile() {
                name = key1,
                IsBuildLoaded = true,
                data = new KAnimFileData("") {
                    buildIndex = 0, batchTag = "Fake"
                }
            };
        }
        __result = cache[key1];
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
