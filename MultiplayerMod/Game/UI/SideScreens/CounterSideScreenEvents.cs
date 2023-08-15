using System;
using HarmonyLib;
using MultiplayerMod.Core.Patch;
using MultiplayerMod.Multiplayer.Objects;
using MultiplayerMod.Multiplayer.Objects.Reference;

namespace MultiplayerMod.Game.UI.SideScreens;

[HarmonyPatch(typeof(CounterSideScreen))]
public static class CounterSideScreenEvents {

    public static event Action<CounterSideScreenEventArgs>? UpdateLogicCounter;

    [HarmonyPostfix]
    [HarmonyPatch(nameof(CounterSideScreen.SetMaxCount))]
    // ReSharper disable once InconsistentNaming, UnusedMember.Local
    private static void SetMaxCount(CounterSideScreen __instance) => TriggerEvent(__instance);

    [HarmonyPostfix]
    [HarmonyPatch(nameof(CounterSideScreen.ToggleAdvanced))]
    // ReSharper disable once InconsistentNaming, UnusedMember.Local
    private static void ToggleAdvanced(CounterSideScreen __instance) => TriggerEvent(__instance);

    private static void TriggerEvent(CounterSideScreen instance) => PatchControl.RunIfEnabled(
        () => UpdateLogicCounter?.Invoke(
            new CounterSideScreenEventArgs(
                instance.targetLogicCounter.GetReference(),
                instance.targetLogicCounter.currentCount,
                instance.targetLogicCounter.maxCount,
                instance.targetLogicCounter.advancedMode
            )
        )
    );

    [Serializable]
    public record CounterSideScreenEventArgs(
        ComponentReference<LogicCounter> Target,
        int CurrentCount,
        int MaxCount,
        bool AdvancedMode
    );

}
