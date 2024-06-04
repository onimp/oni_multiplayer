using System;
using static MultiplayerMod.Game.UI.SideScreens.AlarmSideScreenEvents;

namespace MultiplayerMod.Multiplayer.Commands.Screens.SideScreen;

[Serializable]
public class UpdateAlarm : MultiplayerCommand {

    private readonly AlarmSideScreenEventArgs args;

    public UpdateAlarm(AlarmSideScreenEventArgs args) {
        this.args = args;
    }

    public override void Execute(MultiplayerCommandContext context) {
        var alarm = args.Target.Resolve();
        alarm.notificationName = args.NotificationName;
        alarm.notificationTooltip = args.NotificationTooltip;
        alarm.pauseOnNotify = args.PauseOnNotify;
        alarm.zoomOnNotify = args.ZoomOnNotify;
        alarm.notificationType = args.NotificationType;

        alarm.UpdateNotification(true);
    }

}
