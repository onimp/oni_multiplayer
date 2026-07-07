using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using MultiplayerMod.Core.Collections;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Events;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.Multiplayer.CoreOperations.Events;
using MultiplayerMod.Multiplayer.Players;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.Ownership;

/// <summary>
/// Host-only map of a worked object (dig placer / constructable / …) to the player who ordered it. This is
/// NOT networked — only the host scores chores, so only the host needs it. The Phase 2 preference precondition
/// reads it via <see cref="GetOwner"/> using <c>chore.gameObject</c> (the chore's target).
///
/// Entries are keyed weakly on the target GameObject so destroyed/dug objects fall out without bookkeeping.
/// </summary>
[Core.Dependency.Dependency, UsedImplicitly]
public class ChoreOwnershipRegistry {

    private readonly Core.Logging.Logger log = LoggerFactory.GetLogger<ChoreOwnershipRegistry>();

    private ConditionalWeakTable<GameObject, BoxedValue<PlayerIdentity>> owners = new();

    public ChoreOwnershipRegistry(EventDispatcher events) {
        events.Subscribe<StopMultiplayerEvent>(_ => Clear());
        events.Subscribe<GameQuitEvent>(_ => Clear());
    }

    public PlayerIdentity? GetOwner(GameObject? target) {
        if (target == null)
            return null;
        return owners.TryGetValue(target, out var box) ? box.Value : null;
    }

    /// <summary>Tags the work target(s) sitting at each cell with the ordering player. No-op for cells with none.</summary>
    public void TagCells(IEnumerable<int> cells, PlayerIdentity owner) {
        foreach (var cell in cells) {
            var target = ResolveWorkTarget(cell);
            if (target != null)
                Tag(target, owner);
        }
    }

    private void Tag(GameObject target, PlayerIdentity owner) {
        owners.Remove(target);
        owners.Add(target, new BoxedValue<PlayerIdentity>(owner));
    }

    private void Clear() => owners = new ConditionalWeakTable<GameObject, BoxedValue<PlayerIdentity>>();

    // Dig places its marker on object layer 7 (see DigTool.PlaceDig); a build places a Constructable and a
    // deconstruct order marks the completed building's Deconstructable, both on the building's own layer, so
    // scan for whichever is present. Covers the dig + build + deconstruct scope.
    private static GameObject? ResolveWorkTarget(int cell) {
        if (!Grid.IsValidCell(cell))
            return null;

        var dig = Grid.Objects[cell, 7];
        if (dig != null && dig.GetComponent<Diggable>() != null)
            return dig;

        for (var layer = 0; layer < (int) ObjectLayer.NumLayers; layer++) {
            var occupant = Grid.Objects[cell, layer];
            if (occupant == null)
                continue;
            if (occupant.GetComponent<Constructable>() != null)
                return occupant;
            // A completed building always has a Deconstructable; only tag it once it's actually queued for
            // deconstruction (the DeconstructTool marks it synchronously before we get here).
            var deconstructable = occupant.GetComponent<Deconstructable>();
            if (deconstructable != null && deconstructable.IsMarkedForDeconstruction())
                return occupant;
        }

        return null;
    }

}
