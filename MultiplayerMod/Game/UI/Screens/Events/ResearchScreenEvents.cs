using System;
using HarmonyLib;
using MultiplayerMod.Core.Patch;

namespace MultiplayerMod.Game.UI.Screens.Events;

[HarmonyPatch(typeof(ResearchEntry))]
public static class ResearchScreenEvents {

    public static event Action<string> Canceled;
    public static event Action<string> Selected;

    [HarmonyPostfix]
    [HarmonyPatch(nameof(ResearchEntry.OnResearchCanceled))]
    // ReSharper disable once UnusedMember.Local
    private static void OnResearchCanceledPatch(ResearchEntry __instance) =>
        PatchControl.RunIfEnabled(() => Canceled?.Invoke(__instance.targetTech.Id));

    [HarmonyPostfix]
    [HarmonyPatch(nameof(ResearchEntry.OnResearchClicked))]
    // ReSharper disable once UnusedMember.Local
    private static void OnResearchClickedPatch(ResearchEntry __instance) =>
        PatchControl.RunIfEnabled(() => Selected?.Invoke(__instance.targetTech.Id));
}
