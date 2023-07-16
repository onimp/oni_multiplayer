using System;
using System.Collections.Generic;
using MultiplayerMod.Game.UI.Screens;

namespace MultiplayerMod.Multiplayer.Commands.Screens.Immigration;

[Serializable]
public class InitializeImmigration : IMultiplayerCommand {

    private List<ITelepadDeliverable?>? deliverables;

    public InitializeImmigration(List<ITelepadDeliverable?>? deliverables) {
        this.deliverables = deliverables;
    }

    public void Execute() {
        ImmigrantScreenPatch.Deliverables = deliverables;
    }
}
