using System;
using HarmonyLib;
using MultiplayerMod.Core.Patch;
using MultiplayerMod.Multiplayer.Objects;
using MultiplayerMod.Multiplayer.Objects.Reference;

namespace MultiplayerMod.Game.UI.SideScreens;

[HarmonyPatch(typeof(CounterSideScreen))]
public static class CounterSideScreenEvents {

    public static event Action<ComponentReference, CounterSideScreenEventArgs>? UpdateLogicCounter;

    [HarmonyPostfix]
    [HarmonyPatch(nameof(CounterSideScreen.SetMaxCount))]
    // ReSharper disable once InconsistentNaming, UnusedMember.Local
    private static void SetMaxCount(CounterSideScreen __instance) =>
        PatchControl.RunIfEnabled(
            () => UpdateLogicCounter?.Invoke(
                __instance.targetLogicCounter.GetGridReference(),
                new CounterSideScreenEventArgs(
                    __instance.targetLogicCounter.currentCount,
                    __instance.targetLogicCounter.maxCount,
                    __instance.targetLogicCounter.advancedMode
                )
            )
        );

    [HarmonyPostfix]
    [HarmonyPatch(nameof(CounterSideScreen.ToggleAdvanced))]
    // ReSharper disable once InconsistentNaming, UnusedMember.Local
    private static void ToggleAdvanced(CounterSideScreen __instance) =>
        PatchControl.RunIfEnabled(
            () => UpdateLogicCounter?.Invoke(
                __instance.targetLogicCounter.GetGridReference(),
                new CounterSideScreenEventArgs(
                    __instance.targetLogicCounter.currentCount,
                    __instance.targetLogicCounter.maxCount,
                    __instance.targetLogicCounter.advancedMode
                )
            )
        );

    [Serializable]
    public record CounterSideScreenEventArgs(int CurrentCount, int MaxCount, bool AdvancedMode);
}
