using System;
using MultiplayerMod.ModRuntime.StaticCompatibility;
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

    protected bool Equals(MultiplayerIdReference other)
    {
        return Id.Equals(other.Id);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((MultiplayerIdReference)obj);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

}
