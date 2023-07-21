using System;
using MultiplayerMod.Game.UI.SideScreens;
using MultiplayerMod.Multiplayer.Objects.Reference;

namespace MultiplayerMod.Multiplayer.Commands.Screens.SideScreen;

[Serializable]
public class UpdateCritterSensor : IMultiplayerCommand {
    private readonly ComponentReference target;
    private readonly CritterSensorSideScreenEvents.CritterSensorSideScreenEventArgs eventArgs;

    public UpdateCritterSensor(
        ComponentReference target,
        CritterSensorSideScreenEvents.CritterSensorSideScreenEventArgs eventArgs
    ) {
        this.target = target;
        this.eventArgs = eventArgs;
    }

    public void Execute() {
        var logicCritterCountSensor = (LogicCritterCountSensor) target.GetComponent();
        logicCritterCountSensor.countCritters = eventArgs.CountCritters;
        logicCritterCountSensor.countEggs = eventArgs.CountEggs;
    }
}
