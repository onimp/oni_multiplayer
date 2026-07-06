using System.Collections.Generic;
using HarmonyLib;
using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Unity;
using MultiplayerMod.ModRuntime.Context;
using MultiplayerMod.Multiplayer.Commands.Gameplay;
using MultiplayerMod.Multiplayer.Objects;
using MultiplayerMod.Multiplayer.Objects.Extensions;
using MultiplayerMod.Multiplayer.Objects.Reference;
using MultiplayerMod.Network;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.World;

// Host-authoritative replication of items entering a Storage - the live half of the materials economy that
// used to be hard-sync only. Sweep / fetch / deliver all funnel through Storage.Store (the one method that
// physically moves an item object into a storage), so a single host-side prefix captures every case: it
// records the destination storage, the item's shared id (minting one if the item is a session-spawned
// pickupable with none yet), the item's origin cell and prefab tag, then hands the client the same end
// result via SyncStorageStore (item leaves the ground, lands in the same bin). See SyncStorageStore for the
// client apply and identity-resolution details.
//
// Congestion control: captures are buffered, not sent per-store. StorageSyncFlusher drains the buffer on a
// fixed cadence and groups records per destination storage into one command each, so a burst (auto-sweepers,
// conveyor rails, mass transfers hammering a bin) collapses to ~one message per storage per flush instead of
// hundreds - traffic scales with storages touched, not raw move frequency. Mirrors the buffered/gated design
// of the other host synchronizers (ConstructionSynchronizer for the patch/gate, SimStateSynchronizer for the
// cadence).
[Dependency, UsedImplicitly]
public class StorageSynchronizer {

    private static IMultiplayerServer server = null!;
    private static MultiplayerGame multiplayer = null!;
    private static ExecutionLevelManager manager = null!;

    private static readonly object bufferLock = new();
    private static readonly List<Record> buffer = new();

    public StorageSynchronizer(IMultiplayerServer server, MultiplayerGame multiplayer, ExecutionLevelManager manager) {
        StorageSynchronizer.server = server;
        StorageSynchronizer.multiplayer = multiplayer;
        StorageSynchronizer.manager = manager;
    }

    private struct Record {
        public GameObjectReference Storage;
        public MultiplayerId? ItemId;
        public string PrefabTag;
        public int Cell;
    }

    // Storage.Store is the universal "an item object entered this storage" seam (fetch/deliver/sweep pickup +
    // delivery all reach it). Capture in a prefix, before the store reparents the item and overwrites its
    // position, so the recorded cell is the item's real origin. is_deserializing filters out save-load stores.
    [HarmonyPatch(
        typeof(Storage),
        nameof(Storage.Store),
        new[] { typeof(GameObject), typeof(bool), typeof(bool), typeof(bool), typeof(bool) }
    )]
    private static class StoragePatch {

        [HarmonyPrefix, UsedImplicitly]
        private static void Prefix(Storage __instance, GameObject go, bool is_deserializing) {
            if (is_deserializing || server == null || !ReplicationGate.IsActiveHost(multiplayer, manager))
                return;
            if (server.Clients.Count == 0 || __instance == null || go == null)
                return;

            var pickupable = go.GetComponent<Pickupable>();
            if (pickupable == null)
                return;

            // Ensure the item carries a shared id so both hops (carry -> bin) and both machines resolve it as
            // one object. Hard-sync-era items already have one; session-spawned debris gets a minted id here.
            MultiplayerId? itemId = null;
            var instance = go.GetComponent<MultiplayerInstance>();
            if (instance != null)
                itemId = instance.Id ?? instance.Register();

            var record = new Record {
                Storage = __instance.gameObject.GetReference(),
                ItemId = itemId,
                PrefabTag = pickupable.KPrefabID.PrefabTag.ToString(),
                Cell = Grid.PosToCell(go.transform.GetPosition())
            };
            lock (bufferLock)
                buffer.Add(record);
        }

    }

    // Drain the buffer, grouping per destination storage into one command each. Called on a cadence by
    // StorageSyncFlusher (host only).
    public static void Flush() {
        if (server == null || !ReplicationGate.IsActiveHost(multiplayer, manager)) {
            lock (bufferLock)
                buffer.Clear();
            return;
        }

        List<Record> batch;
        lock (bufferLock) {
            if (buffer.Count == 0)
                return;
            batch = new List<Record>(buffer);
            buffer.Clear();
        }

        // Group per destination storage, but preserve first-touch order (and capture order within a storage)
        // when sending. The reliable Gameplay lane delivers in order, so an item that is picked up into a
        // carrier and then delivered into a bin inside the same flush window is applied carrier-then-bin on
        // the client - the correct sequence. Dictionary enumeration order is not contractually guaranteed, so
        // the parallel `order` list drives sending instead.
        var order = new List<GameObjectReference>();
        var groups = new Dictionary<GameObjectReference, List<SyncStorageStore.Entry>>();
        foreach (var record in batch) {
            if (!groups.TryGetValue(record.Storage, out var entries)) {
                entries = new List<SyncStorageStore.Entry>();
                groups[record.Storage] = entries;
                order.Add(record.Storage);
            }
            entries.Add(new SyncStorageStore.Entry {
                ItemId = record.ItemId,
                PrefabTag = record.PrefabTag,
                Cell = record.Cell
            });
        }

        foreach (var storageReference in order)
            server.Send(new SyncStorageStore(storageReference, groups[storageReference].ToArray()));
    }

}

// Host-only ticker that flushes StorageSynchronizer's coalescing buffer on a fixed cadence. Separate from
// StorageSynchronizer because the capture is a static Harmony patch while the flush needs a per-frame Unity
// tick (the other cadence synchronizers are components for the same reason). Registered in
// MultiplayerGameObjectsSpawner alongside SimStateSynchronizer et al.
public class StorageSyncFlusher : MultiplayerKMonoBehaviour, IRenderEveryTick {

    // Coalescing window: cap per-storage command rate to ~1/sendPeriod while keeping the visual latency low.
    private const float sendPeriod = 0.25f;

    [InjectDependency]
    private readonly MultiplayerGame multiplayer = null!;

    [InjectDependency]
    private readonly ExecutionLevelManager manager = null!;

    private float lastTime;

    public void RenderEveryTick(float dt) {
        if (!manager.LevelIsActive(ExecutionLevel.Multiplayer) || multiplayer.Mode != MultiplayerMode.Host)
            return;
        if (GameClock.Instance == null || GameClock.Instance.GetTime() - lastTime < sendPeriod)
            return;

        lastTime = GameClock.Instance.GetTime();
        StorageSynchronizer.Flush();
    }

}
