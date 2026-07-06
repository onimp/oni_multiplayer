using System;
using Klei.AI;
using MultiplayerMod.Multiplayer.Objects;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.Commands.Gameplay;

// Host-authoritative spawn of a critter/egg produced by a lifecycle event (lay egg, hatch -> baby,
// grow up -> adult). Breeding (FertilityMonitor.EggBreedingRoll) and incubation/age timers are
// non-deterministic, so each machine would otherwise create different critters at different times; the
// client suppresses its own lifecycle spawns (see the CritterLifecycle* synchronizers) and materializes the
// host's instead - under the SAME MultiplayerId the host minted, so position/health streaming and death sync
// resolve the two copies as one object. This is the critter analog of AcceptDelivery's shared-id handshake.
//
// The prefab is identified by its string tag ("Hatch" / "HatchBaby" / "HatchEgg" ...), resolved by
// Assets.GetPrefab on both machines from the same game data, so only the tag + cell + id + a tiny state
// bundle need to cross the wire. The bundle is intentionally minimal: position and health already stream,
// and other amounts (Calories, tameness, ...) reconcile at the daily hard-sync. Byproduct drops (egg shell,
// meat, raw egg) are NOT spawned here - they're loose economy items, deferred like the rest of the economy.
//
// A client that hasn't replicated the surrounding context yet still spawns fine (the spawn is
// self-contained); resolution failures only matter for the DESPAWN side. Rides the reliable Gameplay lane.
[Serializable]
public class SyncSpawnCritter : MultiplayerCommand {

    private readonly string prefabTag;
    private readonly int cell;
    private readonly MultiplayerId id;
    private readonly bool hasTemperature;
    private readonly float temperature;
    private readonly bool hasWildness;
    private readonly float wildness;
    private readonly bool hasAge;
    private readonly float age;

    private SyncSpawnCritter(
        string prefabTag, int cell, MultiplayerId id,
        bool hasTemperature, float temperature,
        bool hasWildness, float wildness,
        bool hasAge, float age
    ) {
        this.prefabTag = prefabTag;
        this.cell = cell;
        this.id = id;
        this.hasTemperature = hasTemperature;
        this.temperature = temperature;
        this.hasWildness = hasWildness;
        this.wildness = wildness;
        this.hasAge = hasAge;
        this.age = age;
    }

    // Host factory: register the freshly spawned host object under a new shared id and snapshot its state
    // bundle, so the client can reproduce the same object under the same id. Called from the lifecycle
    // synchronizers with the host's real spawned critter/egg.
    public static SyncSpawnCritter Create(GameObject spawned, int cell) {
        var id = spawned.GetComponent<MultiplayerInstance>().Register();
        var tag = spawned.GetComponent<KPrefabID>().PrefabTag.Name;

        var primaryElement = spawned.GetComponent<PrimaryElement>();
        var wildnessAmount = Db.Get().Amounts.Wildness.Lookup(spawned);
        var ageAmount = Db.Get().Amounts.Age.Lookup(spawned);

        return new SyncSpawnCritter(
            tag,
            cell,
            id,
            primaryElement != null, primaryElement != null ? primaryElement.Temperature : 0f,
            wildnessAmount != null, wildnessAmount != null ? wildnessAmount.value : 0f,
            ageAmount != null, ageAmount != null ? ageAmount.value : 0f
        );
    }

    public override void Execute(MultiplayerCommandContext context) {
        if (string.IsNullOrEmpty(prefabTag) || !Grid.IsValidCell(cell))
            return;
        var prefab = Assets.GetPrefab(new Tag(prefabTag));
        if (prefab == null)
            return;

        var position = Grid.CellToPosCCC(cell, Grid.SceneLayer.Creatures);
        var gameObject = GameUtil.KInstantiate(prefab, position, Grid.SceneLayer.Creatures, null, 0);
        if (gameObject == null)
            return;
        gameObject.SetActive(true);

        // Register the client's copy under the host's id so both machines resolve it as one object.
        var instance = gameObject.GetComponent<MultiplayerInstance>();
        if (instance != null)
            instance.Register(id);

        // Best-effort state bundle - guard every amount (an egg has no Age, etc.); missing pieces reconcile
        // at the hard-sync.
        if (hasTemperature) {
            var primaryElement = gameObject.GetComponent<PrimaryElement>();
            if (primaryElement != null)
                primaryElement.Temperature = temperature;
        }
        if (hasWildness) {
            var wildnessAmount = Db.Get().Amounts.Wildness.Lookup(gameObject);
            if (wildnessAmount != null)
                wildnessAmount.value = wildness;
        }
        if (hasAge) {
            var ageAmount = Db.Get().Amounts.Age.Lookup(gameObject);
            if (ageAmount != null)
                ageAmount.value = age;
        }
    }

}
