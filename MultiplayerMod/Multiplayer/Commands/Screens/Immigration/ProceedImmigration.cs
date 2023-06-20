using System;
using MultiplayerMod.Game.UI.Screens;

namespace MultiplayerMod.Multiplayer.Commands.Screens.Immigration;

[Serializable]
public class ProceedImmigration : IMultiplayerCommand {
    private readonly ITelepadDeliverable deliverable;

    public ProceedImmigration(ITelepadDeliverable deliverable) {
        this.deliverable = deliverable;
    }

    public void Execute() {
        var immigrantScreen = ImmigrantScreen.instance;
        if (immigrantScreen == null) return;

        immigrantScreen.RemoveLast();
        immigrantScreen.AddDeliverable(deliverable);
        immigrantScreen.proceedButton.SignalClick(KKeyCode.Mouse0);

        ImmigrantScreenPatch.Deliverables = null;
    }
}
