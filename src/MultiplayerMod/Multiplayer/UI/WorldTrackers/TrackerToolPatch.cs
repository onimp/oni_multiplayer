using HarmonyLib;

namespace MultiplayerMod.Multiplayer.UI.WorldTrackers;

[HarmonyPatch(typeof(TrackerTool))]
// ReSharper disable once UnusedType.Global
internal static class TrackerToolPatch {

    [HarmonyPostfix]
    [HarmonyPatch(nameof(TrackerTool.AddNewWorldTrackers))]
    // ReSharper disable once UnusedMember.Local
    private static void AddNewWorldTrackersPostfix(TrackerTool __instance, int worldID) {
        __instance.worldTrackers.Add(new MultiplayerErrorsTracker(worldID));
    }

}
