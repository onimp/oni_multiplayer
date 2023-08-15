using System;
using System.Collections.Generic;
using MultiplayerMod.Game.UI.Screens;

namespace MultiplayerMod.Multiplayer.Commands.Screens.Immigration;

[Serializable]
public class InitializeImmigration : MultiplayerCommand {

    private List<ITelepadDeliverable?>? deliverables;

    public InitializeImmigration(List<ITelepadDeliverable?>? deliverables) {
        this.deliverables = deliverables;
    }

    public override void Execute() {
        ImmigrantScreenPatch.Deliverables = deliverables;
    }
}
