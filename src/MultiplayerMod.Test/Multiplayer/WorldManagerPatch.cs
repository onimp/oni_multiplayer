using HarmonyLib;
using JetBrains.Annotations;
using MultiplayerMod.Multiplayer.World;

namespace MultiplayerMod.Test.Multiplayer;

[UsedImplicitly]
[HarmonyPatch(typeof(WorldManager))]
public class WorldManagerPatch {

    [UsedImplicitly]
    [HarmonyPrefix]
    [HarmonyPatch("GetWorldSave")]
    // ReSharper disable once RedundantAssignment
    private static bool GetWorldSave(ref byte[] __result) {
        __result = new byte[] { 0xFF };
        return false;
    }

    [UsedImplicitly]
    [HarmonyPrefix]
    [HarmonyPatch("LoadWorldSave")]
    // ReSharper disable once RedundantAssignment
    private static bool LoadWorldSave() {
        return false;
    }

}
