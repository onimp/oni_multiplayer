using System.Collections.Generic;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Events;
using MultiplayerMod.Core.Extensions;
using MultiplayerMod.Core.Unity;
using MultiplayerMod.Multiplayer.Players.Events;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.Components;

public class MultiplayerPlayerNotifier : MultiplayerMonoBehaviour {

    [InjectDependency]
    private readonly EventDispatcher dispatcher = null!;

    private const float notificationTimeout = 10f;
    private readonly LinkedList<Notification> notifications = new();
    private EventSubscription subscription = null!;
    private bool removalPending;

    protected override void Awake() {
        base.Awake();
        subscription = dispatcher.Subscribe<PlayerLeftEvent>(OnPlayerLeft);
        NotificationManager.Instance.notificationRemoved += OnNotificationRemoved;
    }

    private void OnDestroy() {
        if (NotificationManager.Instance != null)
            NotificationManager.Instance.notificationRemoved -= OnNotificationRemoved;
        subscription.Cancel();
    }

    private void OnPlayerLeft(PlayerLeftEvent @event) {
        var playerName = @event.Player.Profile.PlayerName;
        var message = $"{playerName} left";
        var description = $"{playerName} {(@event.Gracefully ? "left" : "disconnected")}";
        AddNotification(message, description, NotificationType.BadMinor);
    }

    private void AddNotification(string message, string tooltip, NotificationType type) {
        var notification = new Notification(
            message,
            type,
            tooltip: (_, _) => tooltip,
            expires: false,
            clear_on_click: true,
            show_dismiss_button: true
        ) {
            GameTime = Time.unscaledTime,
            Time = KTime.Instance.UnscaledGameTime,
            Delay = -Time.unscaledTime
        };
        NotificationManager.Instance.AddNotification(notification);
        notifications.AddLast(notification);
    }

    private void OnNotificationRemoved(Notification notification) {
        if (!removalPending)
            notifications.Remove(notification);
    }

    private void Update() {
        notifications.ForEach(
            (notification, node) => {
                if (Time.unscaledTime - notification.GameTime < notificationTimeout)
                    return;

                removalPending = true;
                NotificationManager.Instance.RemoveNotification(notification);
                notifications.Remove(node);
                removalPending = false;
            }
        );
    }

}
