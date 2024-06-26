﻿using System;
using static MultiplayerMod.Game.UI.SideScreens.TimerSideScreenEvents;

namespace MultiplayerMod.Multiplayer.Commands.Screens.SideScreen;

[Serializable]
public class UpdateLogicTimeSensor : MultiplayerCommand {

    private readonly TimerSideScreenEventArgs args;

    public UpdateLogicTimeSensor(TimerSideScreenEventArgs args) {
        this.args = args;
    }

    public override void Execute(MultiplayerCommandContext context) {
        var sensor = args.Target.Resolve();
        sensor.displayCyclesMode = args.DisplayCyclesMode;
        sensor.onDuration = args.OnDuration;
        sensor.offDuration = args.OffDuration;
        sensor.timeElapsedInCurrentState = args.TimeElapsedInCurrentState;
    }

}
