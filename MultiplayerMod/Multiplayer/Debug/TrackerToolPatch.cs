using HarmonyLib;

namespace MultiplayerMod.Multiplayer.Debug;

[HarmonyPatch(typeof(TrackerTool))]
static class TrackerToolPatch {

    [HarmonyPostfix]
    [HarmonyPatch(nameof(TrackerTool.AddNewWorldTrackers))]
    private static void AddNewWorldTrackersPostfix(TrackerTool __instance, int worldID) {
        __instance.worldTrackers.Add(new MultiplayerErrorsTracker(worldID));
    }

}
