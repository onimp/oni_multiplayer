using System;
using MultiplayerMod.Game.Context;
using MultiplayerMod.Game.UI.Tools.Context;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.Commands.Gameplay;

// Host-authoritative building completion. Build orders are replicated (the client places the same
// under-construction ghost), but the client never finishes them: its Constructable storage is empty
// because the materials economy (fetch/deliver) is not synced, so OnCompleteWork bails with
// "this constructable is about to generate a nan" and the ghost stays forever ("3 of 5 ladders").
//
// Instead the host tells the client which cell just finished; the client spawns the completed building
// directly. BuildingDef.Build accepts a null resource_storage - it only uses storage for disease
// bookkeeping - so the finished building's element comes from selected_elements[0] and its temperature
// from the host, no fabricated materials required (this mirrors the mod's own instant-build path).
[Serializable]
public class SyncBuildingComplete : MultiplayerCommand {

    private readonly int cell;
    private readonly string prefabId;
    private readonly Orientation orientation;
    private readonly Tag[] materials;
    private readonly string? facadeId;
    private readonly float temperature;
    private readonly float timeBuilt;

    public SyncBuildingComplete(
        int cell,
        string prefabId,
        Orientation orientation,
        Tag[] materials,
        string? facadeId,
        float temperature,
        float timeBuilt
    ) {
        this.cell = cell;
        this.prefabId = prefabId;
        this.orientation = orientation;
        this.materials = materials;
        this.facadeId = facadeId;
        this.temperature = temperature;
        this.timeBuilt = timeBuilt;
    }

    public override void Execute(MultiplayerCommandContext context) {
        var definition = Assets.GetBuildingDef(prefabId);
        if (definition == null || !Grid.IsValidCell(cell))
            return;

        // Idempotent: if the finished building is already here (client happened to complete it, or a
        // duplicate command), do nothing but still clear any leftover under-construction ghost.
        var ghost = Grid.Objects[cell, (int) definition.ObjectLayer];
        var alreadyComplete = ghost != null && ghost.GetComponent<BuildingComplete>() != null;

        // Utility buildings (wires/pipes) render disconnected unless the finished building's tile
        // visualizer is seeded with the connection bitmask - exactly what Constructable.FinishConstruction
        // copies from the under-construction ghost. Capture it before the ghost is deleted.
        var ghostVisualizer = ghost != null ? ghost.GetComponent<KAnimGraphTileVisualizer>() : null;
        var hasConnections = ghostVisualizer != null;
        var connections = hasConnections ? ghostVisualizer!.Connections : default;

        RemoveConstructionGhost(ghost);
        if (alreadyComplete)
            return;

        GameObject? built = null;
        GameContext.Override(
            new DisableBuildingValidation(),
            () => built = definition.Build(cell, orientation, null, materials, temperature, facadeId, false, timeBuilt)
        );

        if (built != null && hasConnections) {
            var builtVisualizer = built.GetComponent<KAnimGraphTileVisualizer>();
            if (builtVisualizer != null)
                builtVisualizer.Connections = connections;
        }
    }

    // The under-construction ghost is a Constructable occupying the building object layer at this cell.
    // Deleting it stops the client's stuck build chore and removes the visual scaffold.
    private static void RemoveConstructionGhost(GameObject? ghost) {
        if (ghost != null && ghost.GetComponent<Constructable>() != null)
            ghost.DeleteObject();
    }

}
