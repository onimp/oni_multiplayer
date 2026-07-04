using System;

namespace MultiplayerMod.Multiplayer.Commands.Gameplay;

// Host-authoritative terrain removal. Both machines run the dig simulation independently, so a starved
// client often never accumulates enough WorldDamage to actually remove a cell (block dug on host but
// still solid on client). Instead of relying on the client's own work, the host broadcasts every cell
// it removes and the client removes the same cell directly. Idempotent: DestroyCell no-ops on a cell
// that is already gone, so it is safe even if the client happened to finish the dig on its own.
[Serializable]
public class SyncDugCell : MultiplayerCommand {

    private readonly int cell;

    public SyncDugCell(int cell) {
        this.cell = cell;
    }

    public override void Execute(MultiplayerCommandContext context) {
        if (WorldDamage.Instance != null && Grid.IsValidCell(cell) && Grid.Solid[cell])
            WorldDamage.Instance.DestroyCell(cell);
    }

}
