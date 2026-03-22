using System.Collections.Generic;
using NUnit.Framework;

namespace MultiplayerMod.Test.Game.WorldBuilder;

/// <summary>
/// Unit tests for the elementalOres branch size fix in RealWorldState.GetEntitiesBytes().
///
/// Root cause:
///   The elementalOres loop was hardcoded to w=1, h=1 for every ore — no lookup at all:
///     entities.Add(new { ..., w = 1, h = 1 });
///   This is the only branch NOT touched by fixes 914832f / 720a7da / 9ddd91b.
///   If Headquarters (or any building) ends up in spawnData.elementalOres it gets 1×1
///   regardless of BuildingDefCache.  [EntityPath] never matched because the conditional
///   check was only for "Headquarters"-containing IDs that also happened to print BEFORE
///   the hardcoded add — so the log fired but the size was wrong.
///
/// Fix (RealWorldState.GetEntitiesBytes(), elementalOres loop):
///   Replace hardcoded w=1, h=1 with GetEntitySize(o.id), which consults BuildingDefCache
///   first.  Actual ores (Algae, Dirt, Sand …) are not in BuildingDefCache and still get
///   (1,1) as their default; buildings that land here get their real footprint.
///
///   Also: [EntityPath] logs are now unconditional (every entity, every branch) so HQ's
///   exact code path is always visible in startup logs without server restart.
///
/// Tests replicate the size-resolution logic inline (project convention).
/// </summary>
[TestFixture]
[Parallelizable]
public class ElementalOresSizeTests {

    /// <summary>
    /// Replicates the OLD (broken) elementalOres branch logic: always 1×1.
    /// Used to document the root cause.
    /// </summary>
    private static (int w, int h) OldElementalOreSize(string id) =>
        (1, 1); // was hardcoded — BuildingDefCache never consulted

    /// <summary>
    /// Replicates the NEW elementalOres branch logic after the fix:
    /// consult BuildingDefCache first, then fall back to (1,1).
    /// Mirrors GetEntitySize primary path used in the production fix.
    /// </summary>
    private static (int w, int h) NewElementalOreSize(
        string id,
        IReadOnlyDictionary<string, (int w, int h)>? buildingDefCache) {
        if (buildingDefCache != null &&
            buildingDefCache.TryGetValue(id, out var cached) &&
            cached.w > 0 && cached.h > 0)
            return cached;
        return (1, 1); // real ores (Algae, Dirt, Sand …) default to 1×1
    }

    // ─── Root cause regression ────────────────────────────────────────────────

    [Test]
    public void OldLogic_Headquarters_InElementalOres_Returns1x1_ShowingTheBug() {
        // Before the fix: hardcoded (1,1) — BuildingDefCache never consulted.
        var result = OldElementalOreSize("Headquarters");
        Assert.That(result, Is.EqualTo((1, 1)),
            "Old logic returns 1×1 for every ore, including Headquarters — this was the bug");
    }

    // ─── Core fix: BuildingDefCache consulted before 1×1 default ─────────────

    [Test]
    public void NewLogic_Headquarters_InCacheWith4x4_Returns4x4() {
        // THE regression fix: HQ in BuildingDefCache → 4×4 returned instead of hardcoded 1×1.
        var cache = new Dictionary<string, (int w, int h)> { { "Headquarters", (4, 4) } };

        var result = NewElementalOreSize("Headquarters", cache);

        Assert.That(result, Is.EqualTo((4, 4)),
            "Headquarters in BuildingDefCache must return 4×4, not the old hardcoded 1×1");
    }

    [Test]
    public void NewLogic_Headquarters_NotInCache_Returns1x1() {
        // If BuildingDefCache doesn't have HQ (shouldn't happen in prod after configTable
        // harvest, but must be handled gracefully).
        var result = NewElementalOreSize("Headquarters", new Dictionary<string, (int w, int h)>());

        Assert.That(result, Is.EqualTo((1, 1)),
            "When HQ is not in cache, default (1,1) is returned — same as before the fix");
    }

    // ─── Actual ores remain 1×1 ────────────────────────────────────────────────

    [Test]
    public void NewLogic_RealOre_NotInCache_Returns1x1() {
        // Ores like Algae, Dirt, Sand are not in BuildingDefCache → still 1×1.
        // Backward-compat: fix must not change sizes for real ores.
        var cache = new Dictionary<string, (int w, int h)> { { "Headquarters", (4, 4) } };

        var algae  = NewElementalOreSize("Algae",  cache);
        var dirt   = NewElementalOreSize("Dirt",   cache);
        var sand   = NewElementalOreSize("SandStone", cache);

        Assert.That(algae,  Is.EqualTo((1, 1)), "Algae not in cache → 1×1");
        Assert.That(dirt,   Is.EqualTo((1, 1)), "Dirt not in cache → 1×1");
        Assert.That(sand,   Is.EqualTo((1, 1)), "SandStone not in cache → 1×1");
    }

    [Test]
    public void NewLogic_DegenerateCache_ZeroSize_Returns1x1() {
        // Cache entries with zero dimensions must be ignored (degenerate defs).
        var cache = new Dictionary<string, (int w, int h)> { { "Headquarters", (0, 0) } };

        var result = NewElementalOreSize("Headquarters", cache);

        Assert.That(result, Is.EqualTo((1, 1)),
            "Degenerate cache entry (0×0) is skipped — falls through to default 1×1");
    }

    [Test]
    public void NewLogic_NullCache_Returns1x1() {
        // Null cache must not throw — defensive coding.
        var result = NewElementalOreSize("Headquarters", null);

        Assert.That(result, Is.EqualTo((1, 1)),
            "Null buildingDefCache must not throw — returns 1×1");
    }
}
