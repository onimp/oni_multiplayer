using System;
using MultiplayerMod.Game.UI.Screens;

namespace MultiplayerMod.Multiplayer.Commands.Screens.Immigration;

[Serializable]
public class RejectImmigration : IMultiplayerCommand {

    public void Execute() {
        if (ImmigrantScreen.instance == null) return;

        ImmigrantScreen.instance.OnRejectionConfirmed();

        ImmigrantScreenPatch.Deliverables = null;
    }
}
