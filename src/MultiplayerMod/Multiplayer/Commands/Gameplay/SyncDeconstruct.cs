using System;

namespace MultiplayerMod.Multiplayer.Commands.Gameplay;

// Host-authoritative deconstruction. The host tells the client which building it just finished
// deconstructing; the client removes the matching building at that cell so the two worlds agree
// regardless of whether the client's own deconstruct chore had completed.
[Serializable]
public class SyncDeconstruct : MultiplayerCommand {

    private readonly int cell;
    private readonly int objectLayer;

    public SyncDeconstruct(int cell, int objectLayer) {
        this.cell = cell;
        this.objectLayer = objectLayer;
    }

    public override void Execute(MultiplayerCommandContext context) {
        if (!Grid.IsValidCell(cell))
            return;

        var building = Grid.Objects[cell, objectLayer];
        if (building == null)
            return;

        var deconstructable = building.GetComponent<Deconstructable>();
        if (deconstructable == null || deconstructable.HasBeenDestroyed)
            return;

        // ForceDestroyAndGetMaterials runs the game's own removal (spawn debris + DeleteObject) using
        // the building's PrimaryElement. Any residual sim-cell state (e.g. tile occupiers) is corrected
        // by the authoritative cell-buffer stream (SimStateSynchronizer / SyncSimCells).
        deconstructable.ForceDestroyAndGetMaterials();
    }

}
