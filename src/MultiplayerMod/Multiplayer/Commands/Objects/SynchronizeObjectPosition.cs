using System;
using MultiplayerMod.Multiplayer.Objects.Extensions;
using MultiplayerMod.Multiplayer.Objects.Reference;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.Commands.Objects;

[Serializable]
public class SynchronizeObjectPosition : MultiplayerCommand {

    private readonly GameObjectReference reference;
    private readonly Vector3 position;
    private readonly bool? facingLeft;

    public SynchronizeObjectPosition(GameObject gameObject) {
        reference = gameObject.GetReference();
        position = gameObject.transform.GetPosition();
        var facing = gameObject.GetComponent<Facing>();
        if (facing != null)
            facingLeft = facing.facingLeft;
    }

    public override void Execute(MultiplayerCommandContext context) {
        var gameObject = reference.Resolve();
        gameObject.transform.SetPosition(position);
        if (facingLeft != null)
            gameObject.GetComponent<Facing>().SetFacing(facingLeft.Value);
    }

}
