using System;
using System.Collections.Generic;
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

        immigrantScreen.selectedDeliverables = new List<ITelepadDeliverable>();
        immigrantScreen.telepad = global::Components.Telepads[0];
        immigrantScreen.containers ??= new List<ITelepadDeliverableContainer>();

        immigrantScreen.AddDeliverable(deliverable);
        immigrantScreen.proceedButton.SignalClick(KKeyCode.Mouse0);

        ImmigrantScreenPatch.Deliverables = null;
    }
}
