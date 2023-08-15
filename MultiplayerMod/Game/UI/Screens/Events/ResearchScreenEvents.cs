using System;
using HarmonyLib;
using MultiplayerMod.Core.Patch;

namespace MultiplayerMod.Game.UI.Screens.Events;

[HarmonyPatch(typeof(ResearchEntry))]
public static class ResearchScreenEvents {

    public static event Action<string>? Cancel;
    public static event Action<string>? Select;

    [HarmonyPostfix]
    [HarmonyPatch(nameof(ResearchEntry.OnResearchCanceled))]
    // ReSharper disable once UnusedMember.Local
    private static void OnResearchCanceledPatch(ResearchEntry __instance) =>
        PatchControl.RunIfEnabled(() => Cancel?.Invoke(__instance.targetTech.Id));

    [HarmonyPostfix]
    [HarmonyPatch(nameof(ResearchEntry.OnResearchClicked))]
    // ReSharper disable once UnusedMember.Local
    private static void OnResearchClickedPatch(ResearchEntry __instance) =>
        PatchControl.RunIfEnabled(() => Select?.Invoke(__instance.targetTech.Id));
}
