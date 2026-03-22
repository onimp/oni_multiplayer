using System;
using UnityEngine;

namespace DedicatedServer.Game;

/// <summary>
/// Per-entity component bootstrap for loose pickupable / plant GOs in the headless environment.
///
/// Problem: EntityTemplates.CreateBaseOreTemplates() and ExtendEntityToBasicPlant() add
/// Clearable and Prioritizable to every ore / plant prefab. In headless, KComponentSpawn
/// never fires OnPrefabInit on the baseOreTemplate because it is created inactive outside
/// of SpawnEntities. When PlaceOtherEntities() spawns a real instance the template components
/// ARE cloned, but if the prefab was registered via a code path that skips baseOreTemplate
/// (e.g. CreateLooseEntity + direct AddOrGet) the canonical components are absent.
///
/// Confirmed-missing (from dedicated-server error log):
///   • Clearable   — 890x NPE: Clearable.GetSMI() → null reference
///   • Prioritizable — 858x NPE: Prioritizable.OnSpawn → GetComponent<KSelectable>() used
///
/// Setup(go) is called for every spawned GO that:
///   • has a Pickupable component                — is a loose item or plant
///   • has NO MinionBrain or CreatureBrain       — dupes/critters are handled separately
///   • is NOT a building (no Building component) — buildings are handled by BuildingConfigManager
///
/// Components added (from CreateBaseOreTemplates / ExtendEntityToBasicPlant canonical lists):
///   Clearable       — isClearable=false; critters/dupes set their own value; ores default false
///   Prioritizable   — required by fetch chores and harvesting UI
/// </summary>
public static class PickupablePrefab {

    public static void Setup(GameObject go) {
        if (go == null) return;

        // AddOrGet is idempotent — no-op if already present (correct value preserved).
        // isClearable=false matches EntityTemplates.CreateBaseOreTemplates() default.
        go.AddOrGet<Clearable>().isClearable = false;
        go.AddOrGet<Prioritizable>();

        Console.WriteLine(
            $"[Pickupables] {go.name}: Clearable+Prioritizable ensured " +
            $"cell={Grid.PosToCell(go)}"
        );
    }
}
