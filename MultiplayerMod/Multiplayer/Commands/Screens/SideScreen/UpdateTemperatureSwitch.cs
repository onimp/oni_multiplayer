using System;
using MultiplayerMod.Game.UI.SideScreens;
using MultiplayerMod.Multiplayer.Objects.Reference;

namespace MultiplayerMod.Multiplayer.Commands.Screens.SideScreen;

[Serializable]
public class UpdateTemperatureSwitch : IMultiplayerCommand {
    private readonly ComponentReference target;
    private readonly TemperatureSwitchSideScreenEvents.TemperatureSwitchSideScreenEventArgs eventArgs;

    public UpdateTemperatureSwitch(
        ComponentReference target,
        TemperatureSwitchSideScreenEvents.TemperatureSwitchSideScreenEventArgs eventArgs
    ) {
        this.target = target;
        this.eventArgs = eventArgs;
    }

    public void Execute() {
        var temperatureControlledSwitch = (TemperatureControlledSwitch) target.GetComponent();
        temperatureControlledSwitch.thresholdTemperature = eventArgs.ThresholdTemperature;
        temperatureControlledSwitch.activateOnWarmerThan = eventArgs.ActivateOnWarmerThan;
    }
}
