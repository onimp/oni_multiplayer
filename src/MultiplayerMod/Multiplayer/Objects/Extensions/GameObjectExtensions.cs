using System.Runtime.CompilerServices;
using MultiplayerMod.Multiplayer.Objects.Reference;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.Objects.Extensions;

public static class GameObjectExtensions {

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static GameObjectReference GetReference(this GameObject gameObject) {
        // A never-registered or destroyed object has no MultiplayerInstance; guard the null component
        // instead of NRE-ing on .Id (which used to abort world sync and hang joiners) and fall back to
        // the grid reference the id-less path already uses.
        var instance = gameObject.GetComponent<MultiplayerInstance>();
        if (instance != null && instance.Id != null)
            return new MultiplayerIdReference(instance.Id);

        return new GridReference(gameObject);
    }

}
