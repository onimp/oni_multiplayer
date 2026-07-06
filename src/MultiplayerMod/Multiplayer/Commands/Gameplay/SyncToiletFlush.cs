using System;
using MultiplayerMod.Multiplayer.Objects.Reference;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.Commands.Gameplay;

// Host-authoritative replication of a toilet flush - the "fill level + polluted-dirt output" that used to be
// hard-sync only (see docs/sync.md). Toilet.FlushMultiple is host-authoritative *production*: it bumps
// FlushesUsed (which drives the meter and the full/clean state machine), consumes Dirt from the toilet's
// storage and spawns Polluted Dirt + germs back into it. ToiletFlushSynchronizer suppresses that method on
// the client so it can't double-produce; the host sends this instead and the client reproduces the same end
// state on the same toilet.
//
// The toilet's inputs are already replicated - the pee WorkChore completes in lockstep (WorkChoreSynchronizer)
// and the Dirt delivered into its storage rides the storage-intake sync - so the client can reuse the game's
// own ConsumeAndGetDisease / SpawnResource against its own matching storage rather than shipping every mass /
// germ value across. We send only the flush COUNT; the client clamps it against its own FlushesUsed exactly
// as the host did (both start from the same synced value), so the two stay 1:1 across the full/clean cycle.
//
// Deliberately omitted vs. the real FlushMultiple: the germs added to the *duplicant* (worker.AddDisease).
// Duplicant germ load rides the duplicant-vitals hard-sync bucket, so replicating it here would double-count.
// Any residual economy drift in the produced pile self-heals at the next hard-sync, as everywhere else.
//
// Discrete one-shot result -> reliable Gameplay lane, like SyncCritterPoop / SyncStorageStore.
[Serializable]
public class SyncToiletFlush : MultiplayerCommand {

    private readonly ComponentReference<Toilet> toilet;
    private readonly int flushCount;

    public SyncToiletFlush(ComponentReference<Toilet> toilet, int flushCount) {
        this.toilet = toilet;
        this.flushCount = flushCount;
    }

    public override void Execute(MultiplayerCommandContext context) {
        // Resolve throws ObjectNotFoundException (swallowed by CommandExceptionHandler) when the toilet isn't
        // on this client yet - the flush is then moot, the safe fallback (state reconciles at hard-sync).
        var toiletComponent = toilet.Resolve();

        // Clamp against THIS client's fill level exactly as FlushMultiple does. FlushesUsed is synced, so the
        // applied count matches the host's; a fully-used toilet yields 0 and we no-op.
        var applied = Mathf.Min(flushCount, toiletComponent.maxFlushes - toiletComponent.FlushesUsed);
        if (applied <= 0)
            return;

        var storage = toiletComponent.storage;
        if (storage == null)
            return;

        // Mirror Toilet.FlushMultiple's production against our own storage, minus the worker-germ line.
        toiletComponent.FlushesUsed += applied; // setter also pushes the SM `flushes` param -> full/clean states
        if (toiletComponent.meter != null)
            toiletComponent.meter.SetPositionPercent((float) toiletComponent.FlushesUsed / toiletComponent.maxFlushes);

        var dirtTag = ElementLoader.FindElementByHash(SimHashes.Dirt).tag;
        storage.ConsumeAndGetDisease(
            dirtTag, toiletComponent.dirtUsedPerFlush * applied,
            out var amountConsumed, out var diseaseInfo, out _
        );

        var germIndex = Db.Get().Diseases.GetIndex(toiletComponent.diseaseId);
        var germCount = toiletComponent.diseasePerFlush * applied;
        var mass = toiletComponent.solidWastePerUse.mass + amountConsumed;
        var wasteElement = ElementLoader.FindElementByHash(toiletComponent.solidWastePerUse.elementID);
        if (wasteElement?.substance == null)
            return;

        var waste = wasteElement.substance.SpawnResource(
            toiletComponent.transform.GetPosition(), mass, toiletComponent.solidWasteTemperature,
            germIndex, germCount, prevent_merge: true
        );
        waste.GetComponent<PrimaryElement>().AddDisease(diseaseInfo.idx, diseaseInfo.count, "Toilet.Flush.Sync");
        storage.Store(waste);
    }

}
