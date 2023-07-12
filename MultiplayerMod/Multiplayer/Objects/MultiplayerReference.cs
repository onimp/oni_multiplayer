using System;
using MultiplayerMod.Multiplayer.State;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.Objects;

[Serializable]
public class MultiplayerReference {

    public MultiplayerId Id { get; }

    public MultiplayerReference(MultiplayerId id) {
        Id = id;
    }

    public GameObject GetGameObject() {
        var gameObject = MultiplayerGame.Objects[Id] ?? throw new MultiplayerObjectNotFoundException(Id);
        return gameObject;
    }

    public T GetComponent<T>() where T : class => GetGameObject()?.GetComponent<T>();

}
