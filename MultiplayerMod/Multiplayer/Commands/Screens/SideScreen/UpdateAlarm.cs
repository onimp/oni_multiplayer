using System;
using MultiplayerMod.Game.UI.SideScreens;
using MultiplayerMod.Multiplayer.Objects.Reference;

namespace MultiplayerMod.Multiplayer.Commands.Screens.SideScreen;

[Serializable]
public class UpdateAlarm : IMultiplayerCommand {
    private readonly ComponentReference target;
    private readonly AlarmSideScreenEvents.AlarmSideScreenEventArgs alarmSideScreenEventArgs;

    public UpdateAlarm(
        ComponentReference target,
        AlarmSideScreenEvents.AlarmSideScreenEventArgs alarmSideScreenEventArgs
    ) {
        this.target = target;
        this.alarmSideScreenEventArgs = alarmSideScreenEventArgs;
    }

    public void Execute() {
        var alarm = (LogicAlarm) target.GetComponent();
        alarm.notificationName = alarmSideScreenEventArgs.NotificationName;
        alarm.notificationTooltip = alarmSideScreenEventArgs.NotificationTooltip;
        alarm.pauseOnNotify = alarmSideScreenEventArgs.PauseOnNotify;
        alarm.zoomOnNotify = alarmSideScreenEventArgs.ZoomOnNotify;
        alarm.notificationType = alarmSideScreenEventArgs.NotificationType;

        alarm.UpdateNotification(true);
    }
}
