using System;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.Objects.Reference;

[Serializable]
public abstract class GameObjectReference : TypedReference<GameObject> {

    protected abstract GameObject? ResolveGameObject();

    public override GameObject Resolve() => ResolveGameObject() ?? throw new ObjectNotFoundException(this);

}
