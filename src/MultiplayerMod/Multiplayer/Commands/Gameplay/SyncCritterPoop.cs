using System;

namespace MultiplayerMod.Multiplayer.Commands.Gameplay;

// Host-authoritative replication of the loose ore a critter drops when it poops (a Hatch's coal, etc).
//
// CreatureCalorieMonitor.Stomach.Poop emits by element phase: gas / liquid / solid-TILE poop goes straight
// into the world Sim cells, which SimStateSynchronizer already streams host->client every full-map sweep -
// so those need no explicit replication. Only the loose-SOLID branch (element.substance.SpawnResource, e.g.
// coal chunks) drops a pickupable the cell stream does NOT carry; if both machines ran it, each would spawn
// its own divergent pile. CritterPoopSynchronizer suppresses that spawn on the client and the host sends
// this instead, reproducing the same element / mass / temperature / disease drop at the same cell.
//
// The chunk carries no shared MultiplayerId - loose ore is fungible economy (still reconciled in bulk at the
// daily hard-sync), so we only need the same substance to appear in the same place, not a tracked identity.
// Element is addressed by its SimHashes value, stable across machines from the same game data. Rides the
// reliable Gameplay lane.
[Serializable]
public class SyncCritterPoop : MultiplayerCommand {

    private readonly int cell;
    private readonly int elementHash;
    private readonly float mass;
    private readonly float temperature;
    private readonly byte diseaseIdx;
    private readonly int diseaseCount;

    public SyncCritterPoop(int cell, int elementHash, float mass, float temperature, byte diseaseIdx, int diseaseCount) {
        this.cell = cell;
        this.elementHash = elementHash;
        this.mass = mass;
        this.temperature = temperature;
        this.diseaseIdx = diseaseIdx;
        this.diseaseCount = diseaseCount;
    }

    public override void Execute(MultiplayerCommandContext context) {
        if (mass <= 0f || !Grid.IsValidCell(cell))
            return;
        var element = ElementLoader.FindElementByHash((SimHashes) elementHash);
        if (element?.substance == null)
            return;

        // Not inside a Stomach.Poop, so CritterPoopSynchronizer's SpawnResource guard passes this through
        // normally (it merges into any co-located pile exactly as the host's own drop did).
        var position = Grid.CellToPosCCC(cell, Grid.SceneLayer.Ore);
        element.substance.SpawnResource(position, mass, temperature, diseaseIdx, diseaseCount);
    }

}
