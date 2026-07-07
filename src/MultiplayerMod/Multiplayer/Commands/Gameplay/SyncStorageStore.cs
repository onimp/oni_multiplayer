using System;
using System.Collections.Generic;
using MultiplayerMod.Multiplayer.Objects;
using MultiplayerMod.Multiplayer.Objects.Reference;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.Commands.Gameplay;

// Host-authoritative "item entered a storage" replication - the live half of the materials economy that
// used to be hard-sync only (swept/fetched/delivered debris stayed on the client's ground until the daily
// reload; see docs/sync.md). The host runs the real fetch/deliver labor and the item physically enters a
// Storage; StorageSynchronizer captures that moment and batches it here so the client reproduces the same
// end state: the loose item leaves the ground and lands in the same bin.
//
// Identity is resolved the same graceful way the rest of the mod uses (MultiplayerIdReference degrading to
// grid lookup): items present at the last hard-sync already carry a deterministic shared id; anything the
// host swept this session was minted a shared id at capture time and is located on the client by that id,
// or - the first time we see it - by scanning the loose pickupables at its origin cell for a matching prefab.
// Once found the client tags it with the host's id so any later hop (carry -> bin) resolves directly and the
// item is never double-counted. An entry we cannot locate is skipped (it self-heals at the next hard-sync),
// exactly as before - never injected, so we can't duplicate resources.
//
// Discrete one-shot result -> reliable Gameplay lane (default), like SyncSpawnPickupable / SyncBuildingComplete.
[Serializable]
public class SyncStorageStore : MultiplayerCommand {

    [Serializable]
    public struct Entry {
        // The host's shared id for the stored item. Present for hard-sync-era items and for anything the host
        // minted an id on at capture time (StorageSynchronizer mints one if absent), so this is effectively
        // always set; kept nullable for the rare case the host couldn't attach an instance.
        public MultiplayerId? ItemId;
        // Prefab tag of the item, used to disambiguate the loose-pickupable scan when resolving by cell.
        public string PrefabTag;
        // The item's cell at capture time (its ground position before the store), used for the first-sight
        // scan when the client hasn't yet tagged its copy with ItemId.
        public int Cell;
        // The host's own hide_popups flag for this store, replayed verbatim so the client's "Picked up" /
        // "+Delivered" FX matches the host exactly (fetch pickups and deliveries pop; internal transfers stay
        // silent). See StorageSynchronizer for capture.
        public bool HidePopups;
    }

    private readonly GameObjectReference storage;
    private readonly Entry[] entries;

    public SyncStorageStore(GameObjectReference storage, Entry[] entries) {
        this.storage = storage;
        this.entries = entries;
    }

    public override void Execute(MultiplayerCommandContext context) {
        // Resolve throws ObjectNotFoundException (swallowed by CommandExceptionHandler) when the destination
        // storage isn't on this client yet - the whole batch is then moot, which is the safe fallback.
        var storageObject = storage.Resolve();
        if (storageObject == null)
            return;
        var storageComponent = storageObject.GetComponent<Storage>();
        if (storageComponent == null)
            return;

        var objects = context.Multiplayer.Objects;
        foreach (var entry in entries) {
            var item = ResolveItem(objects, entry);
            if (item == null)
                continue;

            // Tag the client's copy under the host id so a subsequent hop (e.g. carry -> bin) resolves it
            // directly instead of rescanning by cell, and so it's counted as the same object on both sides.
            if (entry.ItemId != null) {
                var instance = item.GetComponent<MultiplayerInstance>();
                if (instance != null && instance.Id == null)
                    instance.Register(entry.ItemId);
            }

            // Reuse the game's own Store (with its TryAbsorb stack-merge) so client stacking matches the host.
            // Replay the host's hide_popups so the client shows the same pickup/delivery FX the host did.
            storageComponent.Store(item, hide_popups: entry.HidePopups);
        }
    }

    private static GameObject? ResolveItem(MultiplayerObjects objects, Entry entry) {
        if (entry.ItemId != null) {
            var byId = objects.Get<GameObject>(entry.ItemId);
            if (byId != null)
                return byId;
        }
        return FindLooseItem(entry.Cell, entry.PrefabTag);
    }

    // Walk the pickupable object-layer list at the cell (same structure the game maintains, see
    // ObjectLayerListItem) for a loose item matching the prefab tag and not already stored.
    private static GameObject? FindLooseItem(int cell, string prefabTag) {
        if (!Grid.IsValidCell(cell))
            return null;
        var head = Grid.Objects[cell, (int) ObjectLayer.Pickupables];
        if (head == null)
            return null;
        var pickupable = head.GetComponent<Pickupable>();
        var node = pickupable != null ? pickupable.objectLayerListItem : null;
        var tag = new Tag(prefabTag);
        while (node != null) {
            var candidate = node.pickupable;
            node = node.nextItem;
            if (candidate == null)
                continue;
            if (candidate.KPrefabID.HasTag(GameTags.Stored))
                continue;
            if (candidate.KPrefabID.PrefabTag == tag)
                return candidate.gameObject;
        }
        return null;
    }

}
