using System;
using static MultiplayerMod.Game.UI.SideScreens.TimeRangeSideScreenEvents;

namespace MultiplayerMod.Multiplayer.Commands.Screens.SideScreen;

[Serializable]
public class UpdateLogicTimeOfDaySensor : MultiplayerCommand {

    private readonly TimeRangeSideScreenEventArgs args;

    public UpdateLogicTimeOfDaySensor(TimeRangeSideScreenEventArgs args) {
        this.args = args;
    }

    public override void Execute(MultiplayerCommandContext context) {
        var sensor = args.Target.Resolve();
        sensor.startTime = args.StartTime;
        sensor.duration = args.Duration;
    }

}
