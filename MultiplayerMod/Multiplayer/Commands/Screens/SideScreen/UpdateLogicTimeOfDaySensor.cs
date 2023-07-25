using System;
using static MultiplayerMod.Game.UI.SideScreens.TimeRangeSideScreenEvents;

namespace MultiplayerMod.Multiplayer.Commands.Screens.SideScreen;

[Serializable]
public class UpdateLogicTimeOfDaySensor : IMultiplayerCommand {

    private readonly TimeRangeSideScreenEventArgs args;

    public UpdateLogicTimeOfDaySensor(TimeRangeSideScreenEventArgs args) {
        this.args = args;
    }

    public void Execute() {
        var sensor = args.Target.GetComponent();
        sensor.startTime = args.StartTime;
        sensor.duration = args.Duration;
    }

}
