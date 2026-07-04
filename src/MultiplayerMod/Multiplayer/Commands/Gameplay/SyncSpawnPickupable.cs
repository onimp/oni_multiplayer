using System;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.Commands.Gameplay;

// Host-authoritative spawn of a loose pickupable produced by work whose result the client no longer
// runs (task #2 Phase A suppressed the client's own production). The host does the real work, then
// broadcasts the concrete product so the client materializes the *same* item without touching the
// unsynced materials economy (no fetch/deliver, no shared storage, no shared MultiplayerId - the
// spawned item is anonymous, exactly like client-spawned buildings).
//
// Two producers use it today (see MopSynchronizer / HarvestSynchronizer):
//   - Liquid bottle: mopping bottles consumed liquid into a SubstanceChunk (LiquidSourceManager).
//   - Crop: harvesting KInstantiates a food pickupable and sets its PrimaryElement.
// Both reduce to "spawn a pickupable at a cell with this element/amount/temperature/disease", so one
// command covers both via a Kind discriminator.
//
// Discrete one-shot result -> reliable Gameplay lane (default), like SyncDugCell - NOT the idempotent
// unreliable Sim stream. The element index is the ElementLoader index, which is identical on both
// machines (loaded from the same game data, same as SyncSimCells relies on).
[Serializable]
public class SyncSpawnPickupable : MultiplayerCommand {

    public enum Kind { LiquidChunk, Prefab }

    private readonly Kind kind;
    private readonly int cell;
    private readonly float amount;        // liquid mass (kg) or prefab units
    private readonly float temperature;
    private readonly byte diseaseIdx;
    private readonly int diseaseCount;
    private readonly int elementIdx;      // LiquidChunk only (ElementLoader index)
    private readonly string? prefabTag;   // Prefab only

    private SyncSpawnPickupable(
        Kind kind, int cell, float amount, float temperature, byte diseaseIdx, int diseaseCount,
        int elementIdx, string? prefabTag
    ) {
        this.kind = kind;
        this.cell = cell;
        this.amount = amount;
        this.temperature = temperature;
        this.diseaseIdx = diseaseIdx;
        this.diseaseCount = diseaseCount;
        this.elementIdx = elementIdx;
        this.prefabTag = prefabTag;
    }

    // Mopped liquid -> a bottled SubstanceChunk (mirrors Moppable.OnCellMopped's CreateChunk).
    public static SyncSpawnPickupable LiquidChunk(
        int cell, int elementIdx, float mass, float temperature, byte diseaseIdx, int diseaseCount
    ) => new(Kind.LiquidChunk, cell, mass, temperature, diseaseIdx, diseaseCount, elementIdx, null);

    // Harvested crop / any prefab pickupable (mirrors Crop.SpawnSomeFruit's KInstantiate + PrimaryElement).
    public static SyncSpawnPickupable Prefab(
        string prefabTag, int cell, float units, float temperature, byte diseaseIdx, int diseaseCount
    ) => new(Kind.Prefab, cell, units, temperature, diseaseIdx, diseaseCount, 0, prefabTag);

    public override void Execute(MultiplayerCommandContext context) {
        if (!Grid.IsValidCell(cell))
            return;
        var position = Grid.CellToPosCCC(cell, Grid.SceneLayer.Ore);

        switch (kind) {
            case Kind.LiquidChunk:
                if (LiquidSourceManager.Instance == null ||
                    elementIdx < 0 || elementIdx >= ElementLoader.elements.Count)
                    return;
                var element = ElementLoader.elements[elementIdx];
                if (element == null)
                    return;
                LiquidSourceManager.Instance.CreateChunk(
                    element, amount, temperature, diseaseIdx, diseaseCount, position
                );
                break;

            case Kind.Prefab:
                if (string.IsNullOrEmpty(prefabTag))
                    return;
                var prefab = Assets.GetPrefab(new Tag(prefabTag));
                if (prefab == null)
                    return;
                var go = GameUtil.KInstantiate(prefab, position, Grid.SceneLayer.Ore, null, 0);
                if (go == null)
                    return;
                go.SetActive(true);
                var primaryElement = go.GetComponent<PrimaryElement>();
                if (primaryElement != null) {
                    primaryElement.Units = amount;
                    primaryElement.Temperature = temperature;
                    if (diseaseCount > 0)
                        primaryElement.AddDisease(diseaseIdx, diseaseCount, "SyncSpawnPickupable");
                }
                break;
        }
    }

}
