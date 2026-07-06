using System;
using MultiplayerMod.Game.Context;
using MultiplayerMod.Game.UI.Tools.Context;
using MultiplayerMod.Multiplayer.Objects;
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
    // The host's shared id for the finished building, so the client's spawned copy resolves by the same id
    // (config/id-based syncs targeting a client-built building now work). Null when the host couldn't resolve
    // its own finished building; the client then falls back to grid-addressed resolution as before.
    private readonly MultiplayerId? multiplayerId;

    public SyncBuildingComplete(
        int cell,
        string prefabId,
        Orientation orientation,
        Tag[] materials,
        string? facadeId,
        float temperature,
        float timeBuilt,
        MultiplayerId? multiplayerId
    ) {
        this.cell = cell;
        this.prefabId = prefabId;
        this.orientation = orientation;
        this.materials = materials;
        this.facadeId = facadeId;
        this.temperature = temperature;
        this.timeBuilt = timeBuilt;
        this.multiplayerId = multiplayerId;
    }

    public override void Execute(MultiplayerCommandContext context) {
        var definition = Assets.GetBuildingDef(prefabId);
        if (definition == null || !Grid.IsValidCell(cell))
            return;

        // Idempotent: if the finished building is already here (client happened to complete it, or a
        // duplicate command), do nothing but still clear any leftover under-construction ghost.
        var ghost = Grid.Objects[cell, (int) definition.ObjectLayer];
        var alreadyComplete = ghost != null && ghost.GetComponent<BuildingComplete>() != null;

        // Utility buildings (wires/pipes) render disconnected AND carry no power/liquid/gas unless the
        // finished building's tile visualizer is seeded with the connection bitmask - exactly what
        // Constructable.FinishConstruction copies from the under-construction ghost. Capture it before the
        // ghost is deleted.
        var ghostVisualizer = ghost != null ? ghost.GetComponent<KAnimGraphTileVisualizer>() : null;
        var hasConnections = ghostVisualizer != null;
        var connections = hasConnections ? ghostVisualizer!.Connections : default;

        // Match vanilla Constructable.FinishConstruction: keep the ghost's teardown from running
        // ClearCell on this cell. ClearCell zeroes the cell's connections AND strips the reciprocal
        // bits off all four neighbours in the utility network manager - which would wipe out the
        // connections the completed building registers below (leaving isolated dead wires/pipes).
        if (ghostVisualizer != null)
            ghostVisualizer.skipCleanup = true;

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
                SeedConnections(built, builtVisualizer, connections);
        }

        // Register the client's finished building under the host's id so both machines resolve it as one
        // object (closes the "client-spawned completions are anonymous" gap for configurable buildings).
        if (built != null && multiplayerId != null) {
            var instance = built.GetComponent<MultiplayerInstance>();
            if (instance != null)
                instance.Register(multiplayerId);
        }
    }

    private static readonly Direction[] neighbourDirections = {
        Direction.Up, Direction.Down, Direction.Left, Direction.Right
    };

    // Wire up a freshly-spawned utility building (wire/pipe/tube) the way the game itself does when it
    // spawns one directly (see the WorldGen/Scenario PlaceUtilityConnection path). Two things matter:
    //   1. The KBatchedAnimController isn't ready during OnSpawn, so SetConnections + Refresh must run on
    //      the building's FIRST FRAME - doing it synchronously right after Def.Build leaves the kanim
    //      showing a disconnected stub (and, because Refresh re-seeds the visual grid, carrying no flow).
    //   2. Neighbouring wires/pipes that were already complete need their own Refresh so their junction
    //      art updates to include this new segment - the network rebuild doesn't replay their kanim.
    // Buildings without an IFirstFrameCallback component (rare for utilities) fall back to applying inline.
    private static void SeedConnections(GameObject built, KAnimGraphTileVisualizer visualizer, UtilityConnections connections) {
        void Apply() {
            // UpdateConnections (not the plain Connections setter) calls connectionManager.SetConnections,
            // registering the cell into the electrical/conduit flow network and propagating the reciprocal
            // bits to neighbours so the building actually conducts; Refresh then plays the matching kanim.
            visualizer.UpdateConnections(connections);
            visualizer.Refresh();
            foreach (var direction in neighbourDirections) {
                var neighbour = visualizer.GetNeighbour(direction);
                if (neighbour != null)
                    neighbour.Refresh();
            }
        }

        var firstFrame = built.GetComponent<IFirstFrameCallback>();
        if (firstFrame != null)
            firstFrame.SetFirstFrameCallback(Apply);
        else
            Apply();
    }

    // The under-construction ghost is a Constructable occupying the building object layer at this cell.
    // Deleting it stops the client's stuck build chore and removes the visual scaffold.
    private static void RemoveConstructionGhost(GameObject? ghost) {
        if (ghost != null && ghost.GetComponent<Constructable>() != null)
            ghost.DeleteObject();
    }

}
