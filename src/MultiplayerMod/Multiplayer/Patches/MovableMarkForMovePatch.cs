using HarmonyLib;
using JetBrains.Annotations;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.ModRuntime.Context;
using MultiplayerMod.ModRuntime.StaticCompatibility;

namespace MultiplayerMod.Multiplayer.Patches;

// ONI's Movable.MarkForMove (and the OnReachableChanged / ClearStorageProxy it drives) dereference
// StorageProxy without a null guard. In single-player Movable.MoveToLocation always calls CreateStorageProxy
// first, which yields a live proxy, so MarkForMove never sees a null one. On a multiplayer *client* moves are
// host-authoritative and the client never runs the full move-chore lifecycle, so the move-layer / placer state
// can diverge and CreateStorageProxy can leave StorageProxy resolving to null. MarkForMove then NREs
// (OnReachableChanged -> ClearMove -> ClearStorageProxy, "Object reference not set" in ClearStorageProxy),
// which fires when the local player uses the Move-To tool (MoveToLocationTool.SetMoveToLocation).
//
// Skip the local mark-for-move when there is no valid storage proxy: the client's mark-for-move is only a
// local prediction, the real move is applied on the host and replicated back. The stale storageProxy Ref left
// on the field is self-healing - CreateStorageProxy rebuilds it next time because Get() returns null.
[UsedImplicitly]
[HarmonyPatch(typeof(Movable), "MarkForMove")]
internal static class MovableMarkForMovePatch {

    private static readonly Core.Logging.Logger log = LoggerFactory.GetLogger(typeof(MovableMarkForMovePatch));

    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool MarkForMovePrefix(Movable __instance) {
        // Only intervene inside a multiplayer session; leave vanilla single-player behaviour untouched.
        if (!Dependencies.Get<ExecutionLevelManager>().LevelIsActive(ExecutionLevel.Multiplayer))
            return true;

        if (__instance.StorageProxy != null)
            return true;

        log.Warning("Skipping Movable.MarkForMove: storage proxy could not be resolved (move is host-authoritative)");
        return false;
    }

}
