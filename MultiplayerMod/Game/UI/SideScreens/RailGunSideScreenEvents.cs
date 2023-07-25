using System;
using HarmonyLib;
using MultiplayerMod.Core.Patch;
using MultiplayerMod.Multiplayer.Objects;
using MultiplayerMod.Multiplayer.Objects.Reference;

namespace MultiplayerMod.Game.UI.SideScreens;

[HarmonyPatch(typeof(RailGunSideScreen))]
public static class RailGunSideScreenEvents {

    public static event Action<RailGunSideScreenEventArgs>? UpdateRailGunCapacity;

    [HarmonyPostfix]
    [HarmonyPatch(nameof(RailGunSideScreen.UpdateMaxCapacity))]
    // ReSharper disable once InconsistentNaming, UnusedMember.Local
    private static void UpdateMaxCapacity(RailGunSideScreen __instance, float newValue) => PatchControl.RunIfEnabled(
        () => UpdateRailGunCapacity?.Invoke(
            new RailGunSideScreenEventArgs(__instance.selectedGun.GetReference(), newValue)
        )
    );

    [Serializable]
    public record RailGunSideScreenEventArgs(ComponentReference<RailGun> Target, float LaunchMass);

}
