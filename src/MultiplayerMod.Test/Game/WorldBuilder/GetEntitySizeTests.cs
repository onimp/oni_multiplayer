using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace MultiplayerMod.Test.Game.WorldBuilder;

/// <summary>
/// Unit tests for the GetEntitySize fallback logic used by RealWorldState.GetEntitiesBytes().
/// Covers the path taken by entities that are NOT in spawnData.buildings
/// (e.g. Headquarters in otherEntities/DirectlySpawnedEntities when cluster.currentWorld
/// differs from the world that SpawnEntities iterates).
///
/// Root cause of HQ 1×1: HQ ends up in GetEntitySize path (not buildings loop).
/// PrefabSizeMap may not have HQ under that code path.
/// Assets.GetPrefab("Headquarters") returns null (HQ never completed Add2DComponents registration).
/// → falls to (1,1) default.
///
/// Fix: after PrefabSizeMap miss, consult WorldBuilder.BuildingDefCache (static, populated from
/// configTable harvest — has correct 4×4 for HQ regardless of spawn-time failures).
/// </summary>
[TestFixture]
[Parallelizable]
public class GetEntitySizeTests {

    /// <summary>
    /// Replicates the three-source GetEntitySize resolution logic (inline — project convention).
    /// Sources in priority order:
    ///   1. prefabSizeMap  — from SpawnEntities CaptureEntitySize
    ///   2. buildingDefCache — static configTable harvest (NEW — fixes HQ 1×1)
    ///   3. assetsPrefabSize — Assets.GetPrefab OccupyArea/KBoxCollider2D (null in headless)
    /// </summary>
    private static (int w, int h) GetEntitySize(
        string id,
        IReadOnlyDictionary<string, (int w, int h)> prefabSizeMap,
        IReadOnlyDictionary<string, (int w, int h)> buildingDefCache,
        Func<string, (int w, int h)?> getAssetsPrefabSize) {
        // 1. PrefabSizeMap
        if (prefabSizeMap != null && prefabSizeMap.TryGetValue(id, out var live))
            return live;
        // 2. BuildingDefCache (static configTable harvest)
        try {
            if (buildingDefCache != null && buildingDefCache.TryGetValue(id, out var cs) && cs.w > 0 && cs.h > 0)
                return cs;
        } catch {
            // ignore — fall through
        }
        // 3. Assets prefab (OccupyArea / KBoxCollider2D)
        try {
            var sz = getAssetsPrefabSize?.Invoke(id);
            if (sz.HasValue && sz.Value.w > 0 && sz.Value.h > 0) return sz.Value;
        } catch {
            // ignore — fall through
        }
        return (1, 1);
    }

    // --- BuildingDefCache path (the fix) ---

    [Test]
    public void GetEntitySize_PrefabSizeMapMiss_BuildingDefCacheHas4x4_Returns4x4() {
        // Core regression: HQ in otherEntities, PrefabSizeMap doesn't have it,
        // BuildingDefCache (from configTable harvest) has 4×4 → must return 4×4.
        var cache = new Dictionary<string, (int w, int h)> { { "Headquarters", (4, 4) } };

        var result = GetEntitySize(
            "Headquarters",
            prefabSizeMap: new Dictionary<string, (int w, int h)>(),
            buildingDefCache: cache,
            getAssetsPrefabSize: _ => null);

        Assert.That(result, Is.EqualTo((4, 4)), "HQ must be 4×4 from BuildingDefCache when PrefabSizeMap misses");
    }

    [Test]
    public void GetEntitySize_PrefabSizeMapMiss_BuildingDefCacheHas4x4_TakesPriorityOverAssets() {
        // BuildingDefCache result must be used even if assets would give a different answer.
        var cache = new Dictionary<string, (int w, int h)> { { "Headquarters", (4, 4) } };

        var result = GetEntitySize(
            "Headquarters",
            prefabSizeMap: new Dictionary<string, (int w, int h)>(),
            buildingDefCache: cache,
            getAssetsPrefabSize: _ => (2, 2));

        Assert.That(result, Is.EqualTo((4, 4)), "BuildingDefCache (4×4) must take priority over assets (2×2)");
    }

    [Test]
    public void GetEntitySize_BuildingDefCacheThrows_FallsBackToAssets() {
        // If BuildingDefCache path throws (WidthInCells getter), assets path must still work.
        var result = GetEntitySize(
            "Tile",
            prefabSizeMap: new Dictionary<string, (int w, int h)>(),
            buildingDefCache: new Dictionary<string, (int w, int h)>(),
            getAssetsPrefabSize: _ => (1, 1));

        Assert.That(result, Is.EqualTo((1, 1)), "assets fallback works when cache is unavailable");
    }

    // --- PrefabSizeMap path (existing primary) ---

    [Test]
    public void GetEntitySize_PrefabSizeMapHas4x4_ReturnsPrefabSizeMap() {
        // PrefabSizeMap is still primary — if it has the entry, use it.
        var sizeMap = new Dictionary<string, (int w, int h)> { { "Headquarters", (4, 4) } };
        var cache   = new Dictionary<string, (int w, int h)> { { "Headquarters", (3, 3) } };

        var result = GetEntitySize(
            "Headquarters",
            prefabSizeMap: sizeMap,
            buildingDefCache: cache,
            getAssetsPrefabSize: _ => null);

        Assert.That(result, Is.EqualTo((4, 4)), "PrefabSizeMap must win over BuildingDefCache");
    }

    // --- Default ---

    [Test]
    public void GetEntitySize_AllSourcesMissing_Returns1x1() {
        var result = GetEntitySize(
            "UnknownEntity",
            prefabSizeMap: new Dictionary<string, (int w, int h)>(),
            buildingDefCache: new Dictionary<string, (int w, int h)>(),
            getAssetsPrefabSize: _ => null);

        Assert.That(result, Is.EqualTo((1, 1)), "unknown entity defaults to 1×1");
    }
}
