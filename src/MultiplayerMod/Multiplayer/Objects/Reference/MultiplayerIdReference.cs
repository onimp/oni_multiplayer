using System;
using MultiplayerMod.ModRuntime.StaticCompatibility;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.Objects.Reference;

[Serializable]
public class MultiplayerIdReference(MultiplayerId id) : GameObjectReference {

    public MultiplayerId Id { get; } = id;

    protected override GameObject? ResolveGameObject() => Dependencies.Get<MultiplayerGame>().Objects[Id] as GameObject;

    public override string ToString() => Id.ToString();

    protected bool Equals(MultiplayerIdReference other) => Id.Equals(other.Id);

    public override bool Equals(object? obj) {
        if (ReferenceEquals(null, obj))
            return false;
        if (ReferenceEquals(this, obj))
            return true;

        return obj.GetType() == GetType() && Equals((MultiplayerIdReference) obj);
    }

    public override int GetHashCode() => Id.GetHashCode();

}
