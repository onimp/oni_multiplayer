using System;
using static MultiplayerMod.Game.UI.SideScreens.CritterSensorSideScreenEvents;

namespace MultiplayerMod.Multiplayer.Commands.Screens.SideScreen;

[Serializable]
public class UpdateCritterSensor : MultiplayerCommand {

    private readonly CritterSensorSideScreenEventArgs args;

    public UpdateCritterSensor(CritterSensorSideScreenEventArgs args) {
        this.args = args;
    }

    public override void Execute(MultiplayerCommandContext context) {
        var logicCritterCountSensor = args.Target.Resolve();
        logicCritterCountSensor.countCritters = args.CountCritters;
        logicCritterCountSensor.countEggs = args.CountEggs;
    }

}
