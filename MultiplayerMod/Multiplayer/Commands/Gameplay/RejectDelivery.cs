using System;
using MultiplayerMod.Multiplayer.Objects.Reference;

namespace MultiplayerMod.Multiplayer.Commands.Gameplay;

[Serializable]
public class RejectDelivery : IMultiplayerCommand {

    private ComponentReference<Telepad> reference;

    public RejectDelivery(ComponentReference<Telepad> reference) {
        this.reference = reference;
    }

    public void Execute() => reference.GetComponent().RejectAll();

}
