using System;
using MultiplayerMod.Multiplayer.State;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.Objects;

[Serializable]
public class MultiplayerReference {

    private MultiplayerId id;

    public MultiplayerReference(MultiplayerId id) {
        this.id = id;
    }

    public GameObject GetGameObject() => MultiplayerGame.Objects[id];

    public T GetComponent<T>() => MultiplayerGame.Objects[id].GetComponent<T>();

}
