using System;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.Objects.Reference;

[Serializable]
public class ComponentReference : TypedReference<Component> {

    private GameObjectReference GameObjectReference { get; }
    private Type ComponentType { get; }

    public ComponentReference(GameObjectReference gameObjectReference, Type type) {
        GameObjectReference = gameObjectReference;
        ComponentType = type;
    }

    public override Component? Resolve() => GameObjectReference.GetComponent(ComponentType);

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
public class ComponentReference<T> : ComponentReference where T : KMonoBehaviour {

    public ComponentReference(GameObjectReference gameObjectReference) : base(gameObjectReference, typeof(T)) { }

    public T GetComponent() => (T) Resolve()!;

}
