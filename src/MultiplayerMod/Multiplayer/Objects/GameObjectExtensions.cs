using System.Runtime.CompilerServices;
using MultiplayerMod.Multiplayer.Objects.Reference;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.Objects;

public static class GameObjectExtensions {

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static GameObjectReference GetMultiplayerReference(this GameObject gameObject) =>
        new MultiplayerIdReference(gameObject.GetComponent<MultiplayerInstance>().Id!);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static GameObjectReference GetGridReference(this GameObject gameObject) =>
        new GridReference(gameObject);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static GameObjectReference GetReference(this GameObject gameObject) {
        var multiplayerId = gameObject.GetComponent<MultiplayerInstance>().Id;
        if (multiplayerId != null)
            return new MultiplayerIdReference(multiplayerId);

        return new GridReference(gameObject);
    }

}
