using System;
using MultiplayerMod.Game.UI.Screens;
using MultiplayerMod.Multiplayer.Objects.Reference;

namespace MultiplayerMod.Multiplayer.Commands.Gameplay;

[Serializable]
public class RejectDelivery : MultiplayerCommand {

    private ComponentReference<Telepad> reference;

    public RejectDelivery(ComponentReference<Telepad> reference) {
        this.reference = reference;
    }

    public override void Execute(MultiplayerCommandContext context) {
        reference.Resolve().RejectAll();
        ImmigrantScreenPatch.Deliverables = null;
    }

}
