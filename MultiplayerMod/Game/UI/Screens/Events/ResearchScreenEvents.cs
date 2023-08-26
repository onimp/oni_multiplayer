using System;
using HarmonyLib;
using MultiplayerMod.ModRuntime.Context;

namespace MultiplayerMod.Game.UI.Screens.Events;

[HarmonyPatch(typeof(ResearchEntry))]
public static class ResearchScreenEvents {

    public static event Action<string>? Cancel;
    public static event Action<string>? Select;

    [HarmonyPostfix]
    [HarmonyPatch(nameof(ResearchEntry.OnResearchCanceled))]
    [RequireExecutionLevel(ExecutionLevel.Runtime)]
    // ReSharper disable once UnusedMember.Local
    private static void OnResearchCanceledPatch(ResearchEntry __instance) =>
        Cancel?.Invoke(__instance.targetTech.Id);

    [HarmonyPostfix]
    [HarmonyPatch(nameof(ResearchEntry.OnResearchClicked))]
    [RequireExecutionLevel(ExecutionLevel.Runtime)]
    // ReSharper disable once UnusedMember.Local
    private static void OnResearchClickedPatch(ResearchEntry __instance) =>
        Select?.Invoke(__instance.targetTech.Id);

}
