using System;
using MultiplayerMod.Multiplayer.Objects.Reference;

namespace MultiplayerMod.Multiplayer.Commands.Gameplay;

[Serializable]
public class RejectDelivery : IMultiplayerCommand {

    private GameObjectReference reference;

    public RejectDelivery(GameObjectReference reference) {
        this.reference = reference;
    }

    public void Execute() => reference.GetComponent<Telepad>().RejectAll();

}
