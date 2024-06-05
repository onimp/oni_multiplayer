using System;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.Objects.Reference;

[Serializable]
public class ComponentReference<T>(GameObjectReference reference) : TypedReference<T> where T : Component {

    private GameObjectReference GameObjectReference { get; } = reference;
    private Type ComponentType { get; } = typeof(T);

    public override T Resolve() => (T) GameObjectReference.Resolve().GetComponent(ComponentType);

    protected bool Equals(ComponentReference other) {
        return GameObjectReference.Equals(other.GameObjectReference) && ComponentType == other.ComponentType;
    }

    public override bool Equals(object? obj) {
        if (ReferenceEquals(null, obj))
            return false;
        if (ReferenceEquals(this, obj))
            return true;

        return obj.GetType() == GetType() && Equals((ComponentReference) obj);
    }

    public override int GetHashCode() => GameObjectReference.GetHashCode() * 397 ^ ComponentType.GetHashCode();

}

[Serializable]
public class ComponentReference(GameObjectReference reference, Type type) : ComponentReference<Component>(reference) {

    private readonly Type type = type;

    public override Component Resolve() => base.Resolve().GetComponent(type);

}
