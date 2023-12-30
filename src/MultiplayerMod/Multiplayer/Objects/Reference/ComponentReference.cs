using System;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.Objects.Reference;

[Serializable]
public class ComponentReference : Reference<Component> {

    private GameObjectReference GameObjectReference { get; set; }
    private Type ComponentType { get; set; }

    public ComponentReference(GameObjectReference gameObjectReference, Type type) {
        GameObjectReference = gameObjectReference;
        ComponentType = type;
    }

    public override Component? Resolve() => GameObjectReference.GetComponent(ComponentType);

    protected bool Equals(ComponentReference other) {
        return GameObjectReference.Equals(other.GameObjectReference) && ComponentType == other.ComponentType;
    }

    public override bool Equals(object? obj) {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;

        return Equals((ComponentReference) obj);
    }

    public override int GetHashCode() {
        unchecked {
            return (GameObjectReference.GetHashCode() * 397) ^ ComponentType.GetHashCode();
        }
    }

}

[Serializable]
public class ComponentReference<T> : ComponentReference where T : KMonoBehaviour {

    public ComponentReference(GameObjectReference gameObjectReference) : base(gameObjectReference, typeof(T)) { }

    public T GetComponent() => (T) Resolve()!;
}
