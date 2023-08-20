﻿using System;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.Objects.Reference;

[Serializable]
public class ComponentReference {

    private GameObjectReference GameObjectReference { get; set; }
    private Type ComponentType { get; set; }

    public ComponentReference(GameObjectReference gameObjectReference, Type type) {
        GameObjectReference = gameObjectReference;
        ComponentType = type;
    }

    public Component? GetComponent() => GameObjectReference.GetComponent(ComponentType);

}

[Serializable]
public class ComponentReference<T> where T : KMonoBehaviour {

    private GameObjectReference GameObjectReference { get; set; }

    public ComponentReference(GameObjectReference gameObjectReference) {
        GameObjectReference = gameObjectReference;
    }

    public T GetComponent() => GameObjectReference.GetComponent<T>();

    protected bool Equals(ComponentReference<T> other)
    {
        return GameObjectReference.Equals(other.GameObjectReference);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((ComponentReference<T>)obj);
    }

    public override int GetHashCode()
    {
        return GameObjectReference.GetHashCode();
    }

}
