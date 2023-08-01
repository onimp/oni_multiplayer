using System;
using static MultiplayerMod.Game.UI.SideScreens.CounterSideScreenEvents;

namespace MultiplayerMod.Multiplayer.Commands.Screens.SideScreen;

[Serializable]
public class UpdateLogicCounter : MultiplayerCommand {

    private readonly CounterSideScreenEventArgs args;

    public UpdateLogicCounter(CounterSideScreenEventArgs args) {
        this.args = args;
    }

    public override void Execute() {
        var logicCounter = args.Target.GetComponent();
        logicCounter.maxCount = args.MaxCount;
        logicCounter.currentCount = args.CurrentCount;
        logicCounter.advancedMode = args.AdvancedMode;

        logicCounter.SetCounterState();
        logicCounter.UpdateLogicCircuit();
        logicCounter.UpdateVisualState(true);
        logicCounter.UpdateMeter();
    }

}
