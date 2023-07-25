using System;
using HarmonyLib;
using MultiplayerMod.Core.Patch;
using MultiplayerMod.Multiplayer.Objects;
using MultiplayerMod.Multiplayer.Objects.Reference;

namespace MultiplayerMod.Game.UI.SideScreens;

[HarmonyPatch(typeof(AlarmSideScreen))]
public static class AlarmSideScreenEvents {

    public static event Action<AlarmSideScreenEventArgs>? UpdateAlarm;

    [HarmonyPostfix]
    [HarmonyPatch(nameof(AlarmSideScreen.OnEndEditName))]
    // ReSharper disable once InconsistentNaming, UnusedMember.Local
    private static void OnEndEditName(AlarmSideScreen __instance) => TriggerEvent(__instance);

    [HarmonyPostfix]
    [HarmonyPatch(nameof(AlarmSideScreen.OnEndEditTooltip))]
    // ReSharper disable once InconsistentNaming, UnusedMember.Local
    private static void OnEndEditTooltip(AlarmSideScreen __instance) => TriggerEvent(__instance);

    [HarmonyPostfix]
    [HarmonyPatch(nameof(AlarmSideScreen.TogglePause))]
    // ReSharper disable once InconsistentNaming, UnusedMember.Local
    private static void TogglePause(AlarmSideScreen __instance) => TriggerEvent(__instance);

    [HarmonyPostfix]
    [HarmonyPatch(nameof(AlarmSideScreen.ToggleZoom))]
    // ReSharper disable once InconsistentNaming, UnusedMember.Local
    private static void ToggleZoom(AlarmSideScreen __instance) => TriggerEvent(__instance);

    [HarmonyPostfix]
    [HarmonyPatch(nameof(AlarmSideScreen.SelectType))]
    // ReSharper disable once InconsistentNaming, UnusedMember.Local
    private static void SelectType(AlarmSideScreen __instance) => TriggerEvent(__instance);

    private static void TriggerEvent(AlarmSideScreen instance) => PatchControl.RunIfEnabled(
        () => UpdateAlarm?.Invoke(
            new AlarmSideScreenEventArgs(
                instance.targetAlarm.GetReference(),
                instance.targetAlarm.notificationName,
                instance.targetAlarm.notificationTooltip,
                instance.targetAlarm.pauseOnNotify,
                instance.targetAlarm.zoomOnNotify,
                instance.targetAlarm.notificationType
            )
        )
    );

    [Serializable]
    public record AlarmSideScreenEventArgs(
        ComponentReference<LogicAlarm> Target,
        string NotificationName,
        string NotificationTooltip,
        bool PauseOnNotify,
        bool ZoomOnNotify,
        NotificationType NotificationType
    );

}
