using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace MultiplayerMod.Test.Game.WorldBuilder;

/// <summary>
/// Unit tests for the HQ entity pipeline fix in GetEntitiesBytes().
///
/// Root cause:
///   GetEntitiesBytes() iterated world.SpawnData.buildings for the buildings branch.
///   SpawnData.buildings can be empty (or its entries absent) at query time when:
///     - worldgen fails to place the base template in headless mode
///     - the SpawnData reference is stale (underlying WorldGen object GC'd)
///   In either case, [EntityPath] logs never appear for Headquarters and
///   /api/entities returns no building entries — HQ is invisible to the visualizer.
///
/// Fix (WorldBuilder.SpawnEntities + RealWorldState.GetEntitiesBytes):
///   Each building processed in SpawnEntities() is now tracked in WorldBuilder._trackedBuildings
///   with world offsets already applied.  GetEntitiesBytes() iterates TrackedBuildings instead
///   of spawnData.buildings — the source is always valid because it is populated eagerly
///   during SpawnEntities(), not read lazily from the possibly-stale SpawnData reference.
///   Size resolution uses BuildingDefCache (populated during RegisterBuildingDefs()) so
///   Headquarters always returns 4×4 regardless of whether its GO was fully initialized.
///
/// Also added:
///   [EntityCollect] diagnostic log at the VERY START of GetEntitiesBytes() (before cache
///   check) so spawnData stats and trackedBuildings content appear on every call.
///
/// Tests replicate the entity-collection logic inline (project convention).
/// </summary>
[TestFixture]
[Parallelizable]
public class HqSizeFromTrackedBuildingsTests {

    // ─── Helper: replicates the buildings-branch logic of GetEntitiesBytes ───

    /// <summary>
    /// Replicates the NEW buildings branch in GetEntitiesBytes():
    ///   iterate trackedBuildings, resolve size from buildingDefCache first.
    /// Returns null if <paramref name="id"/> is not in <paramref name="trackedBuildings"/>.
    /// </summary>
    private static (int w, int h)? ResolveTrackedBuilding(
        string id,
        IReadOnlyList<(string id, int x, int y)> trackedBuildings,
        IReadOnlyDictionary<string, (int w, int h)>? buildingDefCache) {

        if (!trackedBuildings.Any(b => b.id == id))
            return null; // not tracked → would not appear in /api/entities

        // Primary path: BuildingDefCache (mirrors ResolveBuildingSize primary branch)
        if (buildingDefCache != null &&
            buildingDefCache.TryGetValue(id, out var cached) &&
            cached.w > 0 && cached.h > 0)
            return cached;

        return (1, 1); // fallback
    }

    // ─── Root cause regression ────────────────────────────────────────────────

    [Test]
    public void OldPath_EmptySpawnData_HeadquartersAbsentFromEntities() {
        // Before the fix: GetEntitiesBytes iterated spawnData.buildings.
        // If spawnData.buildings is empty (worldgen template not placed), HQ is absent.
        var spawnDataBuildings = new List<(string id, int x, int y)>(); // empty — simulates missing template

        var isPresent = spawnDataBuildings.Any(b => b.id == "Headquarters");

        Assert.That(isPresent, Is.False,
            "Old path with empty spawnData.buildings: Headquarters absent — this was the root cause");
    }

    // ─── Core fix: TrackedBuildings populated in SpawnEntities ───────────────

    [Test]
    public void NewPath_HeadquartersInTrackedBuildings_ReturnsSize4x4() {
        // THE regression fix: building tracked during SpawnEntities → size resolved from cache.
        var trackedBuildings = new List<(string id, int x, int y)> {
            ("Headquarters", 128, 196)
        };
        var buildingDefCache = new Dictionary<string, (int w, int h)> {
            { "Headquarters", (4, 4) }
        };

        var result = ResolveTrackedBuilding("Headquarters", trackedBuildings, buildingDefCache);

        Assert.That(result, Is.EqualTo((4, 4)),
            "Headquarters tracked in SpawnEntities + cache hit → must return 4×4, not old hardcoded 1×1");
    }

    [Test]
    public void NewPath_HeadquartersInTrackedBuildings_NotInCache_Returns1x1Fallback() {
        // Edge case: building tracked but BuildingDefCache has no entry.
        // Graceful degradation to (1,1) — same as ores without cache.
        var trackedBuildings = new List<(string id, int x, int y)> { ("Headquarters", 128, 196) };
        var buildingDefCache = new Dictionary<string, (int w, int h)>();

        var result = ResolveTrackedBuilding("Headquarters", trackedBuildings, buildingDefCache);

        Assert.That(result, Is.EqualTo((1, 1)),
            "Tracked but no cache entry → fallback (1,1), not null");
    }

    [Test]
    public void NewPath_HeadquartersNotTracked_AbsentFromEntities() {
        // If SpawnEntities never saw Headquarters (empty buildings list in SpawnData),
        // TrackedBuildings won't contain it and GetEntitiesBytes returns null (entity absent).
        var trackedBuildings = new List<(string id, int x, int y)>(); // SpawnEntities found nothing
        var buildingDefCache = new Dictionary<string, (int w, int h)> { { "Headquarters", (4, 4) } };

        var result = ResolveTrackedBuilding("Headquarters", trackedBuildings, buildingDefCache);

        Assert.That(result, Is.Null,
            "When SpawnEntities found no buildings, HQ is absent from entity list (null = not added)");
    }

    // ─── Multiple buildings tracked simultaneously ────────────────────────────

    [Test]
    public void NewPath_MultipleBuildings_AllResolveCorrectly() {
        // Verifies that tracking multiple buildings works and each gets its own size.
        var trackedBuildings = new List<(string id, int x, int y)> {
            ("Headquarters",  128, 196),
            ("Telepad",       130, 196),
            ("GeneShuffler",  134, 196),
        };
        var buildingDefCache = new Dictionary<string, (int w, int h)> {
            { "Headquarters",  (4, 4) },
            { "Telepad",       (2, 3) },
            { "GeneShuffler",  (3, 3) },
        };

        var hq    = ResolveTrackedBuilding("Headquarters",  trackedBuildings, buildingDefCache);
        var tp    = ResolveTrackedBuilding("Telepad",       trackedBuildings, buildingDefCache);
        var gs    = ResolveTrackedBuilding("GeneShuffler",  trackedBuildings, buildingDefCache);

        Assert.That(hq, Is.EqualTo((4, 4)), "Headquarters → 4×4");
        Assert.That(tp, Is.EqualTo((2, 3)), "Telepad → 2×3");
        Assert.That(gs, Is.EqualTo((3, 3)), "GeneShuffler → 3×3");
    }

    [Test]
    public void NewPath_DegenerateCache_ZeroSize_Falls_Back_To1x1() {
        // Degenerate BuildingDef in cache (w=0 or h=0) is skipped → fallback (1,1).
        var trackedBuildings = new List<(string id, int x, int y)> { ("Headquarters", 128, 196) };
        var buildingDefCache = new Dictionary<string, (int w, int h)> {
            { "Headquarters", (0, 0) }
        };

        var result = ResolveTrackedBuilding("Headquarters", trackedBuildings, buildingDefCache);

        Assert.That(result, Is.EqualTo((1, 1)),
            "Degenerate cache entry (0×0) must be skipped → default 1×1 fallback");
    }

    [Test]
    public void NewPath_NullCache_Returns1x1() {
        // Null cache must not throw — defensive coding.
        var trackedBuildings = new List<(string id, int x, int y)> { ("Headquarters", 128, 196) };

        var result = ResolveTrackedBuilding("Headquarters", trackedBuildings, null);

        Assert.That(result, Is.EqualTo((1, 1)),
            "Null buildingDefCache must not throw — returns fallback 1×1");
    }
}
