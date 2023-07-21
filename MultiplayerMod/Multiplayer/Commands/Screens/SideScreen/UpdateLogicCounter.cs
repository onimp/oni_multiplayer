using System;
using MultiplayerMod.Game.UI.SideScreens;
using MultiplayerMod.Multiplayer.Objects.Reference;

namespace MultiplayerMod.Multiplayer.Commands.Screens.SideScreen;

[Serializable]
public class UpdateLogicCounter : IMultiplayerCommand {
    private readonly ComponentReference target;
    private readonly CounterSideScreenEvents.CounterSideScreenEventArgs eventArgs;

    public UpdateLogicCounter(ComponentReference target, CounterSideScreenEvents.CounterSideScreenEventArgs eventArgs) {
        this.target = target;
        this.eventArgs = eventArgs;
    }

    public void Execute() {
        var logicCounter = (LogicCounter) target.GetComponent();
        logicCounter.maxCount = eventArgs.MaxCount;
        logicCounter.currentCount = eventArgs.CurrentCount;
        logicCounter.advancedMode = eventArgs.AdvancedMode;

        logicCounter.SetCounterState();
        logicCounter.UpdateLogicCircuit();
        logicCounter.UpdateVisualState(true);
        logicCounter.UpdateMeter();
    }
}
