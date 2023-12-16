using System;
using HarmonyLib;
using JetBrains.Annotations;

namespace MultiplayerMod.Test.Multiplayer.Commands.Chores.Patches;

[UsedImplicitly]
[HarmonyPatch(typeof(Assets))]
public class AssetsPatch {

    static AssetsPatch() {
        var batchGroupData = KAnimBatchManager.Instance().GetBatchGroupData(new HashedString("Fake"));
        batchGroupData.AddNewBuildFile(new KAnimHashedString());
        batchGroupData.builds[0].symbols = Array.Empty<KAnim.Build.Symbol>();
    }

    [UsedImplicitly]
    [HarmonyPrefix]
    [HarmonyPatch(nameof(Assets.GetAnim))]
    private static bool Assets_GetAnim(ref KAnimFile __result) {
        __result = new KAnimFile() {
            IsBuildLoaded = true,
            data = new KAnimFileData("") {
                buildIndex = 0, batchTag = new HashedString("Fake")
            }
        };
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
