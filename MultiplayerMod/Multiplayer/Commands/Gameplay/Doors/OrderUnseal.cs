using System;
using MultiplayerMod.Multiplayer.Objects;

namespace MultiplayerMod.Multiplayer.Commands.Gameplay.Doors;

[Serializable]
public class OrderUnseal : IMultiplayerCommand {

    private MultiplayerReference target;

    public OrderUnseal(MultiplayerReference target) {
        this.target = target;
    }

    public void Execute() => target.GetComponent<Door>().OrderUnseal();

}
