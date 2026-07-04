using System;
using MultiplayerMod.Network;

namespace MultiplayerMod.Multiplayer.Commands.Gameplay;

// Authoritative core-simulation replication. ONI's cell simulation (element / temperature / mass /
// disease) runs independently on each machine via the native Sim, so gas, heat and liquid drift apart
// and never reconverge until the daily hard-reload - which is why a duplicant can breathe on the host
// and suffocate on the client. The host periodically streams the truth for a chunk of cells (see
// SimStateSynchronizer) and the client stomps it into its own running Sim with ModifyCell/Replace.
//
// One chunk = a contiguous cell range [startCell, startCell + Length). The host round-robins chunks so
// the whole map is refreshed over several sends; individual cells keep simulating locally between
// refreshes, which is acceptable because drift is gradual.
// Rides the low-priority, unreliable Sim lane: these overwrites are idempotent and re-sent every sweep,
// so a dropped packet self-heals next sweep and never head-of-line-blocks gameplay. SimStateSynchronizer
// sizes chunks to stay within a single network message (unreliable must not fragment).
[Serializable]
[MultiplayerCommand(Lane = NetworkLane.Sim)]
public class SyncSimCells : MultiplayerCommand {

    private readonly int startCell;
    private readonly ushort[] elementIdx;
    private readonly float[] temperature;
    private readonly float[] mass;
    private readonly byte[] diseaseIdx;
    private readonly int[] diseaseCount;

    public SyncSimCells(
        int startCell,
        ushort[] elementIdx,
        float[] temperature,
        float[] mass,
        byte[] diseaseIdx,
        int[] diseaseCount
    ) {
        this.startCell = startCell;
        this.elementIdx = elementIdx;
        this.temperature = temperature;
        this.mass = mass;
        this.diseaseIdx = diseaseIdx;
        this.diseaseCount = diseaseCount;
    }

    public override void Execute(MultiplayerCommandContext context) {
        for (var i = 0; i < elementIdx.Length; i++) {
            var cell = startCell + i;
            if (!Grid.IsValidCell(cell))
                continue;

            // The sim sync owns only the FLUID simulation (gas / liquid / vacuum). Terrain structure is
            // owned authoritatively by DigSynchronizer (removal) and ConstructionSynchronizer (creation),
            // which ride the reliable Gameplay lane. If we let ModifyCell touch solid cells here it would
            // race those: a stale chunk (captured before a dig) arrives on the low-priority unreliable Sim
            // lane after SyncDugCell and re-solidifies the just-dug cell (block flicker, trapped dupes,
            // and a diverged client nav grid → teleporting). So skip a cell whenever the incoming element
            // is solid (would recreate terrain) or the client cell is currently solid (would clear it).
            var incoming = ElementLoader.elements[elementIdx[i]];
            if (incoming.IsSolid || Grid.Solid[cell])
                continue;

            // ReplaceType.Replace fully overwrites the cell contents (element, temperature, mass,
            // disease) with the host's values; no solid displacement so we do not shove neighbours.
            SimMessages.ModifyCell(
                cell,
                elementIdx[i],
                temperature[i],
                mass[i],
                diseaseIdx[i],
                diseaseCount[i],
                SimMessages.ReplaceType.Replace
            );
        }
    }

}
