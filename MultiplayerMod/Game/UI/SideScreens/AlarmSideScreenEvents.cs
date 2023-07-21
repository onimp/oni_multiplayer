using System;
using HarmonyLib;
using MultiplayerMod.Core.Patch;
using MultiplayerMod.Multiplayer.Objects;
using MultiplayerMod.Multiplayer.Objects.Reference;

namespace MultiplayerMod.Game.UI.SideScreens;

[HarmonyPatch(typeof(AlarmSideScreen))]
public static class AlarmSideScreenEvents {

    public static event Action<ComponentReference, AlarmSideScreenEventArgs>? UpdateAlarm;

    [HarmonyPostfix]
    [HarmonyPatch(nameof(AlarmSideScreen.OnEndEditName))]
    // ReSharper disable once InconsistentNaming, UnusedMember.Local
    private static void OnEndEditName(AlarmSideScreen __instance) => PatchControl.RunIfEnabled(
        () => UpdateAlarm?.Invoke(
            __instance.targetAlarm.GetGridReference(),
            new AlarmSideScreenEventArgs(
                __instance.targetAlarm.notificationName,
                __instance.targetAlarm.notificationTooltip,
                __instance.targetAlarm.pauseOnNotify,
                __instance.targetAlarm.zoomOnNotify,
                __instance.targetAlarm.notificationType
            )
        )
    );

    [HarmonyPostfix]
    [HarmonyPatch(nameof(AlarmSideScreen.OnEndEditTooltip))]
    // ReSharper disable once InconsistentNaming, UnusedMember.Local
    private static void OnEndEditTooltip(AlarmSideScreen __instance) => PatchControl.RunIfEnabled(
        () => UpdateAlarm?.Invoke(
            __instance.targetAlarm.GetGridReference(),
            new AlarmSideScreenEventArgs(
                __instance.targetAlarm.notificationName,
                __instance.targetAlarm.notificationTooltip,
                __instance.targetAlarm.pauseOnNotify,
                __instance.targetAlarm.zoomOnNotify,
                __instance.targetAlarm.notificationType
            )
        )
    );

    [HarmonyPostfix]
    [HarmonyPatch(nameof(AlarmSideScreen.TogglePause))]
    // ReSharper disable once InconsistentNaming, UnusedMember.Local
    private static void TogglePause(AlarmSideScreen __instance) => PatchControl.RunIfEnabled(
        () => UpdateAlarm?.Invoke(
            __instance.targetAlarm.GetGridReference(),
            new AlarmSideScreenEventArgs(
                __instance.targetAlarm.notificationName,
                __instance.targetAlarm.notificationTooltip,
                __instance.targetAlarm.pauseOnNotify,
                __instance.targetAlarm.zoomOnNotify,
                __instance.targetAlarm.notificationType
            )
        )
    );

    [HarmonyPostfix]
    [HarmonyPatch(nameof(AlarmSideScreen.ToggleZoom))]
    // ReSharper disable once InconsistentNaming, UnusedMember.Local
    private static void ToggleZoom(AlarmSideScreen __instance) => PatchControl.RunIfEnabled(
        () => UpdateAlarm?.Invoke(
            __instance.targetAlarm.GetGridReference(),
            new AlarmSideScreenEventArgs(
                __instance.targetAlarm.notificationName,
                __instance.targetAlarm.notificationTooltip,
                __instance.targetAlarm.pauseOnNotify,
                __instance.targetAlarm.zoomOnNotify,
                __instance.targetAlarm.notificationType
            )
        )
    );

    [HarmonyPostfix]
    [HarmonyPatch(nameof(AlarmSideScreen.SelectType))]
    // ReSharper disable once InconsistentNaming, UnusedMember.Local
    private static void SelectType(AlarmSideScreen __instance) => PatchControl.RunIfEnabled(
        () => UpdateAlarm?.Invoke(
            __instance.targetAlarm.GetGridReference(),
            new AlarmSideScreenEventArgs(
                __instance.targetAlarm.notificationName,
                __instance.targetAlarm.notificationTooltip,
                __instance.targetAlarm.pauseOnNotify,
                __instance.targetAlarm.zoomOnNotify,
                __instance.targetAlarm.notificationType
            )
        )
    );

    [Serializable]
    public record AlarmSideScreenEventArgs(
        string NotificationName,
        string NotificationTooltip,
        bool PauseOnNotify,
        bool ZoomOnNotify,
        NotificationType NotificationType
    );
}
