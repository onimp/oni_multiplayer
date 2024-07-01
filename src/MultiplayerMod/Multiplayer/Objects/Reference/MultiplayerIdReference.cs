using System;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.ModRuntime;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.Objects.Reference;

[Serializable]
[DependenciesStaticTarget]
public class MultiplayerIdReference(MultiplayerId id) : GameObjectReference {

    [InjectDependency]
    private static MultiplayerObjects objects = null!;

    public MultiplayerId Id { get; } = id;

    protected override GameObject? ResolveGameObject() => objects.Get<GameObject>(Id);

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
