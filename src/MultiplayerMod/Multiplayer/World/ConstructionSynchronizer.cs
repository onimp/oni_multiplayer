using HarmonyLib;
using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.ModRuntime.Context;
using MultiplayerMod.Multiplayer.Commands.Gameplay;
using MultiplayerMod.Multiplayer.Objects;
using MultiplayerMod.Network;

namespace MultiplayerMod.Multiplayer.World;

// Host-authoritative construction and deconstruction result-replication. Each side runs the labor sim
// independently, and the client's materials economy (fetch/deliver) is not synced, so the client's
// builds get stuck (empty storage) and completions/removals drift. Instead the host broadcasts the
// moment a building actually finishes building or deconstructing, and the client applies the same
// result directly (see SyncBuildingComplete / SyncDeconstruct).
[Dependency, UsedImplicitly]
public class ConstructionSynchronizer {

    private static IMultiplayerServer server = null!;
    private static MultiplayerGame multiplayer = null!;
    private static ExecutionLevelManager manager = null!;

    public ConstructionSynchronizer(IMultiplayerServer server, MultiplayerGame multiplayer, ExecutionLevelManager manager) {
        ConstructionSynchronizer.server = server;
        ConstructionSynchronizer.multiplayer = multiplayer;
        ConstructionSynchronizer.manager = manager;
    }

    private static bool IsHostActive() =>
        manager != null && multiplayer != null && server != null &&
        manager.LevelIsActive(ExecutionLevel.Multiplayer) && multiplayer.Mode == MultiplayerMode.Host;

    // FinishConstruction is the private method that actually spawns the completed building and deletes
    // the ghost; a postfix fires exactly once per real completion, after initialTemperature and the
    // selected elements are settled.
    [HarmonyPatch(typeof(Constructable), "FinishConstruction")]
    private static class ConstructablePatch {

        [HarmonyPostfix, UsedImplicitly]
        private static void Postfix(Constructable __instance) {
            if (!IsHostActive())
                return;

            var def = __instance.building.Def;
            var cell = Grid.PosToCell(__instance.transform.GetLocalPosition());
            var rotatable = __instance.GetComponent<Rotatable>();
            var orientation = rotatable != null ? rotatable.GetOrientation() : Orientation.Neutral;
            var facade = __instance.GetComponent<BuildingFacade>();
            var facadeId = facade != null ? facade.CurrentFacade : null;

            // Mint a shared id on the just-finished building (a new grid-addressable object at this cell,
            // separate from the constructable ghost) so the client can register its copy under the same id.
            var completed = Grid.Objects[cell, (int) def.ObjectLayer];
            MultiplayerId? multiplayerId = null;
            if (completed != null) {
                var instance = completed.GetComponent<MultiplayerInstance>();
                if (instance != null)
                    multiplayerId = instance.Register();
            }

            server.Send(new SyncBuildingComplete(
                cell,
                def.PrefabID,
                orientation,
                __instance.selectedElementsTags,
                facadeId,
                __instance.initialTemperature,
                GameClock.Instance.GetTime(),
                multiplayerId
            ));
        }

    }

    [HarmonyPatch(typeof(Deconstructable), "OnCompleteWork")]
    private static class DeconstructablePatch {

        [HarmonyPostfix, UsedImplicitly]
        private static void Postfix(Deconstructable __instance) {
            if (!IsHostActive())
                return;

            var building = __instance.GetComponent<Building>();
            if (building == null)
                return;

            var cell = Grid.PosToCell(__instance.transform.GetPosition());
            server.Send(new SyncDeconstruct(cell, (int) building.Def.ObjectLayer));
        }

    }

}
