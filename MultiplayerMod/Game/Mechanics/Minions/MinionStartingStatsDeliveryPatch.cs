using HarmonyLib;
using MultiplayerMod.Core.Patch;
using MultiplayerMod.Core.Patch.Context;

namespace MultiplayerMod.Game.Mechanics.Minions;

[HarmonyPatch(typeof(MinionStartingStats))]
// ReSharper disable once UnusedType.Global
public static class MinionStartingStatsDeliveryPatch {

    // ReSharper disable once UnusedMember.Local
    [HarmonyPrefix]
    [HarmonyPatch(nameof(MinionStartingStats.Deliver))]
    private static void BeforeDelivery() => PatchContext.Enter(PatchControl.DisablePatches);

    // ReSharper disable once UnusedMember.Local
    [HarmonyPostfix]
    [HarmonyPatch(nameof(MinionStartingStats.Deliver))]
    private static void AfterDelivery() => PatchContext.Leave();

}
