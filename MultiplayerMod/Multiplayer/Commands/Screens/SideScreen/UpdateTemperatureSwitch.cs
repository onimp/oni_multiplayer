using System;
using static MultiplayerMod.Game.UI.SideScreens.TemperatureSwitchSideScreenEvents;

namespace MultiplayerMod.Multiplayer.Commands.Screens.SideScreen;

[Serializable]
public class UpdateTemperatureSwitch : MultiplayerCommand {

    private readonly TemperatureSwitchSideScreenEventArgs args;

    public UpdateTemperatureSwitch(TemperatureSwitchSideScreenEventArgs args) {
        this.args = args;
    }

    public override void Execute() {
        var temperatureControlledSwitch = args.Target.GetComponent();
        temperatureControlledSwitch.thresholdTemperature = args.ThresholdTemperature;
        temperatureControlledSwitch.activateOnWarmerThan = args.ActivateOnWarmerThan;
    }

}
