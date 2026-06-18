using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.Compatibility;

public static class DlcMultiplayerSafety {

    private const float NotificationCooldownSeconds = 5f;
    private static float lastNotificationTime = float.NegativeInfinity;

    private static readonly DlcUnsafeTypePattern[] UnsafeObjectSyncTypes = {
        new("PassengerRocketModule", "rocket crew assignment is not synced safely yet"),
        new("RocketControlStation", "rocket control station state is not synced safely yet"),
        new("Rocket", "rocket launch and flight state are not synced safely yet"),
        new("CraftModule", "rocket craft module actions are not synced safely yet"),
        new("LaunchCondition", "rocket launch validation is not synced safely yet"),
        new("Starmap", "starmap interactions are not synced safely yet"),
        new("ClusterMap", "cluster map interactions are not synced safely yet"),
        new("ClusterDestination", "cluster destination selection is not synced safely yet"),
        new("Clustercraft", "spacecraft state is not synced safely yet"),
        new("SpaceDestination", "space destination objects are not synced safely yet"),
        new("SpacePOI", "space POI interactions are not synced safely yet"),
        new("IEmptyableCargo", "rocket cargo controls are not synced safely yet")
    };

    public static bool ShouldBlockObjectSync(MethodBase method) => GetBlockReason(method) != null;

    public static string? GetBlockReason(MethodBase method) {
        var declaringTypeName = method.DeclaringType?.Name ?? "";
        return UnsafeObjectSyncTypes
            .FirstOrDefault(pattern => declaringTypeName.IndexOf(pattern.TypeName, StringComparison.OrdinalIgnoreCase) >= 0)
            ?.Reason;
    }

    public static string GetBlockMessage(MethodBase method) {
        var typeName = method.DeclaringType?.Name ?? "<unknown>";
        var reason = GetBlockReason(method) ?? "this DLC action is not synced safely yet";
        return $"Blocked DLC multiplayer action {typeName}.{method.Name}: {reason}.";
    }

    public static void NotifyUnsupported(string message) {
        try {
            if (NotificationManager.Instance == null)
                return;
            if (Time.unscaledTime - lastNotificationTime < NotificationCooldownSeconds)
                return;

            lastNotificationTime = Time.unscaledTime;
            var notification = new Notification(
                "DLC multiplayer preview",
                NotificationType.BadMinor,
                tooltip: (_, _) => message,
                expires: false,
                clear_on_click: true,
                show_dismiss_button: true
            ) {
                GameTime = Time.unscaledTime,
                Time = KTime.Instance?.UnscaledGameTime ?? 0,
                Delay = -Time.unscaledTime
            };
            NotificationManager.Instance.AddNotification(notification);
        } catch { }
    }

    private record DlcUnsafeTypePattern(string TypeName, string Reason);

}
