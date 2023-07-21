using HarmonyLib;

namespace MultiplayerMod.Game.Mechanics.Minions;

[HarmonyPatch(typeof(MinionStartingStats))]
public static class MinionDeliveryState {

    public static bool Spawning { get; private set; }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(MinionStartingStats.Deliver))]
    private static void BeforeDelivery() => Spawning = true;

    [HarmonyPostfix]
    [HarmonyPatch(nameof(MinionStartingStats.Deliver))]
    private static void AfterDelivery() => Spawning = false;

}
