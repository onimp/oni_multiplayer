using System;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Multiplayer.State;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.Objects.Reference;

[Serializable]
public class MultiplayerIdReference : GameObjectReference {

    public MultiplayerId Id { get; }

    public MultiplayerIdReference(MultiplayerId id) {
        Id = id;
    }

    protected override GameObject? Resolve() => Dependencies.Get<MultiplayerGame>().Objects[Id];

    public override string ToString() => Id.ToString();

}
