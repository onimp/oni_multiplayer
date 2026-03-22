using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace MultiplayerMod.Test.Game.WorldBuilder;

/// <summary>
/// Unit tests for the GetEntitySize resolution logic used by RealWorldState.GetEntitiesBytes().
///
/// Root cause history:
///   Pass 1: HQ not in buildings loop → no [Size] log.
///   Pass 2: HQ in GetEntitySize but reached BuildingDefCache only AFTER PrefabSizeMap.
///   Pass 3 (this fix): PrefabSizeMap["Headquarters"]=(1,1) from stale CaptureEntitySize →
///     silent early-return with no log. Fix: move BuildingDefCache BEFORE PrefabSizeMap so
///     configTable-authoritative data always wins for buildings.
///
/// Priority order (post-fix):
///   1. buildingDefCache — static configTable harvest, never overwritten by spawn logic
///   2. prefabSizeMap    — from SpawnEntities CaptureEntitySize (may be stale 1,1)
///   3. assetsPrefabSize — Assets.GetPrefab OccupyArea/KBoxCollider2D (null in headless)
/// </summary>
[TestFixture]
[Parallelizable]
public class GetEntitySizeTests {

    /// <summary>
    /// Replicates the updated GetEntitySize resolution logic (inline — project convention).
    /// BuildingDefCache is now PRIMARY (checked before PrefabSizeMap).
    /// </summary>
    private static (int w, int h) GetEntitySize(
        string id,
        IReadOnlyDictionary<string, (int w, int h)> prefabSizeMap,
        IReadOnlyDictionary<string, (int w, int h)> buildingDefCache,
        Func<string, (int w, int h)?> getAssetsPrefabSize) {
        // 1. BuildingDefCache — PRIMARY (configTable harvest, authoritative for buildings)
        try {
            if (buildingDefCache != null && buildingDefCache.TryGetValue(id, out var cs) && cs.w > 0 && cs.h > 0)
                return cs;
        } catch {
            // ignore — fall through
        }
        // 2. PrefabSizeMap — secondary (may be stale 1×1 from CaptureEntitySize in headless)
        if (prefabSizeMap != null && prefabSizeMap.TryGetValue(id, out var live) && live.w > 0 && live.h > 0)
            return live;
        // 3. Assets prefab (OccupyArea / KBoxCollider2D)
        try {
            var sz = getAssetsPrefabSize?.Invoke(id);
            if (sz.HasValue && sz.Value.w > 0 && sz.Value.h > 0) return sz.Value;
        } catch {
            // ignore — fall through
        }
        return (1, 1);
    }

    // --- BuildingDefCache path: PRIMARY (the fix) ---

    [Test]
    public void GetEntitySize_BuildingDefCacheHas4x4_Returns4x4() {
        // Core regression test: BuildingDefCache has correct HQ size → must return it.
        var cache = new Dictionary<string, (int w, int h)> { { "Headquarters", (4, 4) } };

        var result = GetEntitySize(
            "Headquarters",
            prefabSizeMap: new Dictionary<string, (int w, int h)>(),
            buildingDefCache: cache,
            getAssetsPrefabSize: _ => null);

        Assert.That(result, Is.EqualTo((4, 4)), "HQ must be 4×4 from BuildingDefCache");
    }

    [Test]
    public void GetEntitySize_BuildingDefCacheHas4x4_WinsOverStalePrefabSizeMap1x1() {
        // THE regression fix: PrefabSizeMap has stale (1,1), BuildingDefCache has (4,4).
        // BuildingDefCache must win — PrefabSizeMap silent return was the "third path" bug.
        var cache   = new Dictionary<string, (int w, int h)> { { "Headquarters", (4, 4) } };
        var sizeMap = new Dictionary<string, (int w, int h)> { { "Headquarters", (1, 1) } };

        var result = GetEntitySize(
            "Headquarters",
            prefabSizeMap: sizeMap,
            buildingDefCache: cache,
            getAssetsPrefabSize: _ => null);

        Assert.That(result, Is.EqualTo((4, 4)),
            "BuildingDefCache (4×4) must override stale PrefabSizeMap (1×1) — this was the silent third-path regression");
    }

    [Test]
    public void GetEntitySize_BuildingDefCacheHas4x4_TakesPriorityOverAssets() {
        // BuildingDefCache wins over assets fallback too.
        var cache = new Dictionary<string, (int w, int h)> { { "Headquarters", (4, 4) } };

        var result = GetEntitySize(
            "Headquarters",
            prefabSizeMap: new Dictionary<string, (int w, int h)>(),
            buildingDefCache: cache,
            getAssetsPrefabSize: _ => (2, 2));

        Assert.That(result, Is.EqualTo((4, 4)), "BuildingDefCache (4×4) must win over assets (2×2)");
    }

    // --- PrefabSizeMap path: secondary ---

    [Test]
    public void GetEntitySize_BuildingDefCacheMissPrefabSizeMapHas4x4_ReturnsPrefabSizeMap() {
        // Building not in cache (e.g. critter) — PrefabSizeMap is used.
        var sizeMap = new Dictionary<string, (int w, int h)> { { "HatchHard", (1, 1) } };

        var result = GetEntitySize(
            "HatchHard",
            prefabSizeMap: sizeMap,
            buildingDefCache: new Dictionary<string, (int w, int h)>(),
            getAssetsPrefabSize: _ => null);

        Assert.That(result, Is.EqualTo((1, 1)), "PrefabSizeMap used when BuildingDefCache misses");
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
