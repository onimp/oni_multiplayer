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

        // A Diggable has no shared id and registers ITSELF at Grid.Objects[cell, DigPlacer] in OnSpawn -
        // NOT at its GameObjectExtension.GridLayer. A default GridReference therefore reads the wrong layer
        // on the client and resolves to the wrong object (or null), so the reconstructed dig WorkChore
        // targets the wrong thing (the "weird animation" + placeholder errand during the dig phase of a tile
        // build) and SyncWorkComplete can't find the diggable (dropped -> the client dupe animates the dig 5s
        // longer every stint via the completion fallback). Pin the DigPlacer layer so the client finds its own
        // Diggable at the same cell. Only reached on the id-less path, so it doesn't cost the common case.
        if (gameObject.GetComponent<Diggable>() != null)
            return new GridReference(Grid.PosToCell(gameObject), (int) ObjectLayer.DigPlacer);

        return new GridReference(gameObject);
    }

}
