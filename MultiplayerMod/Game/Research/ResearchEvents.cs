using System;
using HarmonyLib;
using MultiplayerMod.Core.Patch;

namespace MultiplayerMod.Game.Research;

[HarmonyPatch(typeof(ResearchEntry))]
public static class ResearchEvents {

    public static event Action<string> ResearchCanceled;
    public static event Action<string> ResearchSelected;

    [HarmonyPostfix]
    [HarmonyPatch(nameof(ResearchEntry.OnResearchCanceled))]
    // ReSharper disable once UnusedMember.Local
    private static void OnResearchCanceledPatch(ResearchEntry __instance) =>
        PatchControl.RunIfEnabled(() => ResearchCanceled?.Invoke(__instance.targetTech.Id));

    [HarmonyPostfix]
    [HarmonyPatch(nameof(ResearchEntry.OnResearchClicked))]
    // ReSharper disable once UnusedMember.Local
    private static void OnResearchClickedPatch(ResearchEntry __instance) =>
        PatchControl.RunIfEnabled(() => ResearchSelected?.Invoke(__instance.targetTech.Id));
}
