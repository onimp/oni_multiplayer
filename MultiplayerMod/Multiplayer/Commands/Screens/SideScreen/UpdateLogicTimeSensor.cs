using System;
using MultiplayerMod.Game.UI.SideScreens;
using MultiplayerMod.Multiplayer.Objects.Reference;

namespace MultiplayerMod.Multiplayer.Commands.Screens.SideScreen;

[Serializable]
public class UpdateLogicTimeSensor : IMultiplayerCommand {
    private readonly ComponentReference target;
    private readonly TimerSideScreenEvents.TimerSideScreenEventArgs eventArgs;

    public UpdateLogicTimeSensor(ComponentReference target, TimerSideScreenEvents.TimerSideScreenEventArgs eventArgs) {
        this.target = target;
        this.eventArgs = eventArgs;
    }

    public void Execute() {
        var sensor = (LogicTimerSensor) target.GetComponent();
        sensor.displayCyclesMode = eventArgs.DisplayCyclesMode;
        sensor.onDuration = eventArgs.OnDuration;
        sensor.offDuration = eventArgs.OffDuration;
        sensor.timeElapsedInCurrentState = eventArgs.TimeElapsedInCurrentState;
    }
}
