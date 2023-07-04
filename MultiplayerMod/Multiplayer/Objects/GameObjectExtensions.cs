using System.Runtime.CompilerServices;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.Objects;

public static class GameObjectExtensions {

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static MultiplayerReference GetMultiplayerReference(this GameObject gameObject) =>
        new(gameObject.GetComponent<MultiplayerInstance>().Id);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static MultiplayerReference GetMultiplayerReference(this KMonoBehaviour component) =>
        GetMultiplayerReference(component.gameObject);

}
