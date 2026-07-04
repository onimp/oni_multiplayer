using System;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.Objects.Reference;

[Serializable]
public class ComponentReference<T>(GameObjectReference reference) : TypedReference<T> where T : Component {

    private GameObjectReference GameObjectReference { get; } = reference;
    private Type ComponentType { get; } = typeof(T);

    // Resolve gracefully: a chore argument can reference a game object (or component) that hasn't been
    // replicated to this client yet, was destroyed, or deserialized without its GameObjectReference.
    // Throw ObjectNotFoundException (logged + skipped by CommandExceptionHandler) rather than NRE-ing and
    // aborting the whole command batch. Uses Unity's null semantics so destroyed objects count as missing.
    public override T Resolve() {
        if (GameObjectReference == null)
            throw new ObjectNotFoundException(this);
        var gameObject = GameObjectReference.Resolve();
        if (gameObject == null)
            throw new ObjectNotFoundException(this);
        var component = gameObject.GetComponent(ComponentType);
        if (component == null)
            throw new ObjectNotFoundException(this);
        return (T) component;
    }

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

    public override Component Resolve() {
        var component = base.Resolve().GetComponent(type);
        if (component == null)
            throw new ObjectNotFoundException(this);
        return component;
    }

}
