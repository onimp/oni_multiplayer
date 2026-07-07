using System;
using MultiplayerMod.Multiplayer.Objects.Extensions;
using MultiplayerMod.Multiplayer.Objects.Reference;
using MultiplayerMod.Multiplayer.World;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.Commands.Gameplay;

// Host-authoritative "which recipe this duplicant-operated fabricator (Rock Crusher, Microbe Musher, ...) is
// now working" - a display/timing state the client can't derive on its own. The client never starts an order
// itself: ordering is gated on the fabricator's ingredient storage, and the host consumes those ingredients
// the instant it starts an order (that consumption streams to the client), so the client's copy always sees
// "no ingredients -> no order" and its fabricate WorkChore then runs with GetWorkTime()==-1 (finishes
// instantly, spams "CurrentMachineOrder is null", and completes via the 5s work fallback out of lockstep).
//
// So the host tells the client the working recipe index directly. recipe_list is built deterministically in
// OnPrefabInit (same order on both machines), so the index is portable. recipeIndex >= 0 = "now working that
// recipe"; recipeIndex < 0 = "no active order" (sent on completion/cancel). The physical product and storage
// stay host-authoritative and are streamed separately; this only mirrors the order pointer so the client's
// visualizer shows and its work stint runs the right duration (see ComplexFabricatorSynchronizer).
[Serializable]
public class SyncFabricatorOrder : MultiplayerCommand {

    private readonly GameObjectReference fabricator;
    private readonly int recipeIndex;

    public SyncFabricatorOrder(GameObject fabricator, int recipeIndex) {
        this.fabricator = fabricator.GetReference();
        this.recipeIndex = recipeIndex;
    }

    public override void Execute(MultiplayerCommandContext context) {
        GameObject gameObject;
        try {
            gameObject = fabricator.Resolve();
        } catch (ObjectNotFoundException) {
            return; // fabricator not present on this client yet - the order pointer is moot, self-heals at hard-sync
        }
        if (gameObject == null)
            return;
        var component = gameObject.GetComponent<ComplexFabricator>();
        if (component != null)
            ComplexFabricatorSynchronizer.ApplyOrder(component, recipeIndex);
    }

}
