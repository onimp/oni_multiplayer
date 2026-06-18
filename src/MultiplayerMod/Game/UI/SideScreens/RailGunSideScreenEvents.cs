using System;
using HarmonyLib;
using MultiplayerMod.Game.World;
using MultiplayerMod.ModRuntime.Context;
using MultiplayerMod.Multiplayer.Objects.Extensions;
using MultiplayerMod.Multiplayer.Objects.Reference;

namespace MultiplayerMod.Game.UI.SideScreens;

[HarmonyPatch(typeof(RailGunSideScreen))]
public static class RailGunSideScreenEvents {

    public static event Action<RailGunSideScreenEventArgs>? UpdateRailGunCapacity;

    [HarmonyPostfix]
    [HarmonyPatch(nameof(RailGunSideScreen.UpdateMaxCapacity))]
    [RequireExecutionLevel(ExecutionLevel.Game)]
    // ReSharper disable once InconsistentNaming, UnusedMember.Local
    private static void UpdateMaxCapacity(RailGunSideScreen __instance, float newValue) {
        if (__instance.selectedGun == null)
            return;

        UpdateRailGunCapacity?.Invoke(
            new RailGunSideScreenEventArgs(
                __instance.selectedGun.GetReference(),
                newValue,
                WorldIdentity.GetObjectWorldId(__instance.selectedGun.gameObject)
            )
        );
    }

    [Serializable]
    public record RailGunSideScreenEventArgs(ComponentReference<RailGun> Target, float LaunchMass, int? WorldId = null);

}
