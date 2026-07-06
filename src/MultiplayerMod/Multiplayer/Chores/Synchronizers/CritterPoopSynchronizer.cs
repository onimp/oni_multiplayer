using HarmonyLib;
using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.ModRuntime.Context;
using MultiplayerMod.Multiplayer.Commands.Gameplay;
using MultiplayerMod.Network;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.Chores.Synchronizers;

// Host-authoritative critter element emission. Each machine runs its own critter AI, so left alone every
// client Hatch would eat and poop its own divergent coal on its own timer. Gas / liquid / solid-tile poop
// lands in the Sim cells (already streamed by SimStateSynchronizer), so the only emission the cell stream
// misses is the loose ORE a poop drops via Substance.SpawnResource. We let the client's Stomach.Poop run its
// normal bookkeeping (calories drain, the poop behaviour clears) but intercept just that one spawn: the host
// captures + broadcasts it (SyncCritterPoop) and the client suppresses its local copy, so both sides end up
// with the host's exact drop instead of two independent piles.
//
// The `pooping` flag scopes the Substance.SpawnResource patch to poop drops ONLY - it is a shared primitive
// used by mining and much else, so outside a Poop the prefix passes straight through. Reset via a finalizer
// so a throw inside Poop can't leave the flag stuck (Poop is synchronous and non-reentrant, so a plain
// static bool is enough). See ReplicationGate for the null-guard / bool-prefix semantics.
[Dependency, UsedImplicitly]
public class CritterPoopSynchronizer {

    private static IMultiplayerServer server = null!;
    private static MultiplayerGame multiplayer = null!;
    private static ExecutionLevelManager manager = null!;

    // True only while THIS machine is inside a critter's Stomach.Poop.
    private static bool pooping;

    public CritterPoopSynchronizer(IMultiplayerServer server, MultiplayerGame multiplayer, ExecutionLevelManager manager) {
        CritterPoopSynchronizer.server = server;
        CritterPoopSynchronizer.multiplayer = multiplayer;
        CritterPoopSynchronizer.manager = manager;
    }

    [HarmonyPrefix, UsedImplicitly]
    [HarmonyPatch(typeof(CreatureCalorieMonitor.Stomach), nameof(CreatureCalorieMonitor.Stomach.Poop))]
    private static void PoopPrefix() => pooping = true;

    // Finalizer (not postfix) so the flag resets even if Poop throws; void + no __exception param leaves any
    // pending exception untouched.
    [HarmonyFinalizer, UsedImplicitly]
    [HarmonyPatch(typeof(CreatureCalorieMonitor.Stomach), nameof(CreatureCalorieMonitor.Stomach.Poop))]
    private static void PoopFinalizer() => pooping = false;

    [HarmonyPrefix, UsedImplicitly]
    [HarmonyPatch(typeof(Substance), nameof(Substance.SpawnResource))]
    private static bool SpawnResourcePrefix(
        Substance __instance, Vector3 position, float mass, float temperature, byte disease_idx, int disease_count
    ) {
        if (!pooping)
            return true; // not a poop drop - untouched
        if (ReplicationGate.IsActiveClient(multiplayer, manager))
            return false; // client: host replicates the drop via SyncCritterPoop instead of spawning its own
        if (server != null && ReplicationGate.IsActiveHost(multiplayer, manager)) {
            var cell = Grid.PosToCell(position);
            if (Grid.IsValidCell(cell))
                server.Send(new SyncCritterPoop(
                    cell, (int) __instance.elementID, mass, temperature, disease_idx, disease_count
                ));
        }
        return true; // host keeps its own chunk
    }

}
