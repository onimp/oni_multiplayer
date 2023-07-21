using System;
using MultiplayerMod.Game.UI.SideScreens;
using MultiplayerMod.Multiplayer.Objects.Reference;

namespace MultiplayerMod.Multiplayer.Commands.Screens.SideScreen;

[Serializable]
public class UpdateLogicTimeOfDaySensor : IMultiplayerCommand {
    private readonly ComponentReference target;
    private readonly TimeRangeSideScreenEvents.TimeRangeSideScreenEventArgs eventArgs;

    public UpdateLogicTimeOfDaySensor(
        ComponentReference target,
        TimeRangeSideScreenEvents.TimeRangeSideScreenEventArgs eventArgs
    ) {
        this.target = target;
        this.eventArgs = eventArgs;
    }

    public void Execute() {
        var sensor = (LogicTimeOfDaySensor) target.GetComponent();
        sensor.startTime = eventArgs.StartTime;
        sensor.duration = eventArgs.Duration;
    }
}
