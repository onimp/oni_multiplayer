using System;
using HarmonyLib;
using MultiplayerMod.Core.Patch;

namespace MultiplayerMod.Game.Research;

[HarmonyPatch(typeof(ResearchEntry))]
public static class ResearchEvents {

    public static event Action<string> OnResearchCanceled;
    public static event Action<string> OnResearchSelected;

    [HarmonyPostfix]
    [HarmonyPatch(nameof(ResearchEntry.OnResearchCanceled))]
    // ReSharper disable once UnusedMember.Local
    private static void OnResearchCanceledPatch(ResearchEntry __instance) =>
        PatchControl.RunIfEnabled(() => OnResearchCanceled?.Invoke(__instance.targetTech.Id));

    [HarmonyPostfix]
    [HarmonyPatch(nameof(ResearchEntry.OnResearchClicked))]
    // ReSharper disable once UnusedMember.Local
    private static void OnResearchClickedPatch(ResearchEntry __instance) =>
        PatchControl.RunIfEnabled(() => OnResearchSelected?.Invoke(__instance.targetTech.Id));
}
