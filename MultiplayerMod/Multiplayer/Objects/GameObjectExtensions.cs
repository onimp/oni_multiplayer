using System.Runtime.CompilerServices;
using MultiplayerMod.Multiplayer.Objects.Reference;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.Objects;

public static class GameObjectExtensions {

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static GameObjectReference GetMultiplayerReference(this GameObject gameObject) =>
        new MultiplayerIdReference(gameObject.GetComponent<MultiplayerInstance>().Id);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static GameObjectReference GetMultiplayerReference(this KMonoBehaviour component) =>
        GetMultiplayerReference(component.gameObject);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static GameObjectReference GetGridReference(this GameObject gameObject) =>
        new GridReference(gameObject);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static GameObjectReference GetGridReference(this KMonoBehaviour component) =>
        GetGridReference(component.gameObject);

}
