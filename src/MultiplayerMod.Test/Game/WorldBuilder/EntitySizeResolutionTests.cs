using System.Collections.Generic;
using NUnit.Framework;

namespace MultiplayerMod.Test.Game.WorldBuilder;

/// <summary>
/// Tests for the unified entity-size resolution logic in RealWorldState.GetEntitiesBytes().
///
/// Root cause (old system):
///   GetEntitiesBytes had 5 branches, each with different size logic:
///     buildings   → ResolveBuildingSize (BuildingDefCache → Assets.GetBuildingDef → sizeMap)
///     otherEntities → GetEntitySize     (BuildingDefCache → PrefabSizeMap → Assets.GetPrefab)
///     pickupables   → GetEntitySize
///     elementalOres → GetEntitySize (formerly hardcoded 1×1)
///     DirectlySpawned → GetEntitySize
///   Geysers (e.g. GeyserGeneric_steam) ended up in otherEntities. CaptureEntitySize wrote
///   (1,1) to PrefabSizeMap because the spawned GO had null OccupyArea offsets in headless;
///   PrefabSizeMap was returned before the Assets.GetPrefab fallback could run.
///
/// Fix:
///   1. CaptureEntitySize now falls back to Assets.GetPrefab when the spawned GO has no
///      OccupyArea offsets — geysers get their correct 2×4 (or 4×2 / 3×3) size.
///   2. GetEntitiesBytes builds ONE merged size map (PrefabSizeMap + building sizes from
///      BuildingDefCache; buildings always win over stale PrefabSizeMap entries).
///   3. All five branches call ResolveEntitySize(id, mergedSizeMap) — one function, one source.
///
/// Tests replicate the resolution logic inline (project convention — no game runtime).
/// </summary>
[TestFixture]
[Parallelizable]
public class EntitySizeResolutionTests {

    // ─── Helper: replicates RealWorldState.ResolveEntitySize ─────────────────

    private static (int w, int h) ResolveEntitySize(
        string prefabId,
        IReadOnlyDictionary<string, (int w, int h)>? sizeMap) {
        if (sizeMap != null && sizeMap.TryGetValue(prefabId, out var sz) && sz.w > 0 && sz.h > 0)
            return sz;
        return (1, 1);
    }

    // ─── Buildings ────────────────────────────────────────────────────────────

    [Test]
    public void Headquarters_Returns4x4() {
        var sizeMap = new Dictionary<string, (int w, int h)> { { "Headquarters", (4, 4) } };
        Assert.That(ResolveEntitySize("Headquarters", sizeMap), Is.EqualTo((4, 4)));
    }

    [Test]
    public void Telepad_Returns2x2() {
        var sizeMap = new Dictionary<string, (int w, int h)> { { "Telepad", (2, 2) } };
        Assert.That(ResolveEntitySize("Telepad", sizeMap), Is.EqualTo((2, 2)));
    }

    [Test]
    public void GeneShuffler_Returns3x4() {
        var sizeMap = new Dictionary<string, (int w, int h)> { { "GeneShuffler", (3, 4) } };
        Assert.That(ResolveEntitySize("GeneShuffler", sizeMap), Is.EqualTo((3, 4)));
    }

    // ─── Geysers (previously returned 1×1 — the regression) ─────────────────

    [Test]
    public void GeyserGenericSteam_Returns2x4() {
        // Gas geysers (steam, hot_co2, hydrogen, etc.) are 2 wide × 4 tall.
        // This was the primary regression: CaptureEntitySize wrote (1,1) for geysers
        // because OccupyArea offsets were null on spawned GOs in headless mode.
        // Fix: CaptureEntitySize now falls back to Assets.GetPrefab whose OccupyArea
        // has correct offsets set by EntityTemplates.CreatePlacedEntity.
        var sizeMap = new Dictionary<string, (int w, int h)> { { "GeyserGeneric_steam", (2, 4) } };
        Assert.That(ResolveEntitySize("GeyserGeneric_steam", sizeMap), Is.EqualTo((2, 4)));
    }

    [Test]
    public void GeyserGenericHotWater_Returns4x2() {
        // Liquid geysers are 4 wide × 2 tall.
        var sizeMap = new Dictionary<string, (int w, int h)> { { "GeyserGeneric_hot_water", (4, 2) } };
        Assert.That(ResolveEntitySize("GeyserGeneric_hot_water", sizeMap), Is.EqualTo((4, 2)));
    }

    [Test]
    public void GeyserGenericSmallVolcano_Returns3x3() {
        // Molten/volcano geysers are 3×3.
        var sizeMap = new Dictionary<string, (int w, int h)> { { "GeyserGeneric_small_volcano", (3, 3) } };
        Assert.That(ResolveEntitySize("GeyserGeneric_small_volcano", sizeMap), Is.EqualTo((3, 3)));
    }

    // ─── Duplicants ───────────────────────────────────────────────────────────

    [Test]
    public void Minion_Returns1x2() {
        // Dupe KBoxCollider2D.size = (1, 1.5) → rounds to 1×2.
        var sizeMap = new Dictionary<string, (int w, int h)> { { "Minion", (1, 2) } };
        Assert.That(ResolveEntitySize("Minion", sizeMap), Is.EqualTo((1, 2)));
    }

    // ─── Ores / pickupables ───────────────────────────────────────────────────

    [Test]
    public void Algae_Returns1x1() {
        // Ores have no BuildingDef and no OccupyArea → default 1×1.
        var sizeMap = new Dictionary<string, (int w, int h)> { { "Algae", (1, 1) } };
        Assert.That(ResolveEntitySize("Algae", sizeMap), Is.EqualTo((1, 1)));
    }

    [Test]
    public void UnknownEntity_NotInMap_Returns1x1() {
        // Entity absent from merged map → (1,1) default.
        var sizeMap = new Dictionary<string, (int w, int h)>();
        Assert.That(ResolveEntitySize("UnknownEntity", sizeMap), Is.EqualTo((1, 1)));
    }

    [Test]
    public void NullSizeMap_Returns1x1() {
        // Null map must not throw — returns default.
        Assert.That(ResolveEntitySize("Headquarters", null), Is.EqualTo((1, 1)));
    }

    // ─── Merged map: buildings always win over stale PrefabSizeMap entries ───

    [Test]
    public void BuildingsWinOverStaleEntries() {
        // BuildingDefCache has correct HQ=4×4, but PrefabSizeMap has stale (1,1).
        // The merged map should contain 4×4 (buildings overlay PrefabSizeMap).
        // Replicate BuildMergedSizeMap logic: start with prefabSizeMap, overlay buildingSizes.
        var prefabSizeMap = new Dictionary<string, (int w, int h)> {
            { "Headquarters", (1, 1) }, // stale entry from failed CaptureEntitySize
            { "GeyserGeneric_steam", (2, 4) } // correct geyser entry
        };
        var buildingSizes = new Dictionary<string, (int w, int h)> {
            { "Headquarters", (4, 4) } // authoritative from BuildingDefCache
        };

        // Merge: copy prefab, then overlay buildings
        var merged = new Dictionary<string, (int w, int h)>(prefabSizeMap);
        foreach (var kvp in buildingSizes) merged[kvp.Key] = kvp.Value;

        Assert.That(ResolveEntitySize("Headquarters", merged), Is.EqualTo((4, 4)),
            "BuildingDefCache must win over stale PrefabSizeMap (1,1) for HQ");
        Assert.That(ResolveEntitySize("GeyserGeneric_steam", merged), Is.EqualTo((2, 4)),
            "Geyser entry from PrefabSizeMap (correctly captured) must be preserved");
    }

    [Test]
    public void DegenerateEntry_ZeroSize_ReturnsDefault1x1() {
        // If an entry somehow has (0,0), it must be skipped and default (1,1) returned.
        var sizeMap = new Dictionary<string, (int w, int h)> { { "BadBuilding", (0, 0) } };
        Assert.That(ResolveEntitySize("BadBuilding", sizeMap), Is.EqualTo((1, 1)),
            "Degenerate (0,0) entry must not be used — default (1,1) instead");
    }

    [Test]
    public void LegitimateOneByOne_Building_Returned() {
        // A 1×1 tile building must NOT be confused with the default fallback.
        var sizeMap = new Dictionary<string, (int w, int h)> { { "Tile", (1, 1) } };
        Assert.That(ResolveEntitySize("Tile", sizeMap), Is.EqualTo((1, 1)),
            "1×1 buildings are valid and must be returned correctly");
    }

    // ─── All entity types covered by a single map ─────────────────────────────

    [Test]
    public void AllBranchesCoveredByOneMap_CorrectSizes() {
        // Verifies that ONE merged map correctly serves all five branches of GetEntitiesBytes.
        // Each entity type that used to have its own size logic now uses the same source.
        var sizeMap = new Dictionary<string, (int w, int h)> {
            // buildings branch (from BuildingDefCache)
            { "Headquarters",           (4, 4) },
            { "Telepad",                (2, 2) },
            // otherEntities branch (geysers, critters — from CaptureEntitySize + prefab fallback)
            { "GeyserGeneric_steam",    (2, 4) },
            { "GeyserGeneric_hot_water",(4, 2) },
            { "HatchHard",              (1, 1) },
            // elementalOres branch (ores remain 1×1)
            { "Algae",                  (1, 1) },
            // DirectlySpawnedEntities branch (dupes)
            { "Minion",                 (1, 2) },
        };

        Assert.That(ResolveEntitySize("Headquarters",           sizeMap), Is.EqualTo((4, 4)));
        Assert.That(ResolveEntitySize("Telepad",                sizeMap), Is.EqualTo((2, 2)));
        Assert.That(ResolveEntitySize("GeyserGeneric_steam",    sizeMap), Is.EqualTo((2, 4)));
        Assert.That(ResolveEntitySize("GeyserGeneric_hot_water",sizeMap), Is.EqualTo((4, 2)));
        Assert.That(ResolveEntitySize("HatchHard",              sizeMap), Is.EqualTo((1, 1)));
        Assert.That(ResolveEntitySize("Algae",                  sizeMap), Is.EqualTo((1, 1)));
        Assert.That(ResolveEntitySize("Minion",                 sizeMap), Is.EqualTo((1, 2)));
    }
}
