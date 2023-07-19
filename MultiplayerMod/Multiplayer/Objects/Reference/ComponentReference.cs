using System;
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

    public Component GetComponent() => GameObjectReference.GetComponent(ComponentType);
}
