using System;
using static MultiplayerMod.Game.UI.SideScreens.CritterSensorSideScreenEvents;

namespace MultiplayerMod.Multiplayer.Commands.Screens.SideScreen;

[Serializable]
public class UpdateCritterSensor : IMultiplayerCommand {

    private readonly CritterSensorSideScreenEventArgs args;

    public UpdateCritterSensor(CritterSensorSideScreenEventArgs args) {
        this.args = args;
    }

    public void Execute() {
        var logicCritterCountSensor = args.Target.GetComponent();
        logicCritterCountSensor.countCritters = args.CountCritters;
        logicCritterCountSensor.countEggs = args.CountEggs;
    }

}
