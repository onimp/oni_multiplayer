using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using NUnit.Framework;

namespace MultiplayerMod.Test.Game.WorldBuilder;

/// <summary>
/// Unit tests for the ResolveBuildingSize logic used by RealWorldState.GetEntitiesBytes()
/// to determine cell dimensions of each building in /api/entities.
///
/// Regression: /api/entities returned HQ w=1 h=1 even when _buildingDefCache had correct 4×4.
/// Root cause (post 70b667d): ResolveBuildingSize called GetBuildingDef as a Func delegate;
/// if that throws (Assets.GetBuildingDef unavailable in headless after partial registration),
/// the silent catch fell through to PrefabSizeMap. PrefabSizeMap can also have stale (1,1)
/// if CaptureEntitySize failed at spawn time for that building.
///
/// Fix: pass _buildingDefCache directly as a new parameter so its (w,h) can be read without
/// any game API call — direct dict lookup + ReadBuildingDefSize, no exception possible.
///
/// Priority order in ResolveBuildingSize:
///   1. buildingDefCache — static, populated from configTable harvest BEFORE spawn crashes
///   2. getBuildingDef   — Assets wrapper (fully-registered buildings only)
///   3. sizeMap          — PrefabSizeMap, may have stale (1,1)
///
/// Tests replicate the resolution logic inline (project convention — no DedicatedServer import).
/// </summary>
[TestFixture]
[Parallelizable]
public class ResolveBuildingSizeTests {

    /// <summary>
    /// Replicates the three-source RealWorldState.ResolveBuildingSize logic.
    /// buildingDefCache is PRIMARY — checked before any delegate invocation.
    /// </summary>
    private static (int w, int h) ResolveBuildingSize(
        string id,
        IReadOnlyDictionary<string, (int w, int h)> sizeMap,
        IReadOnlyDictionary<string, (int w, int h)> buildingDefCache,
        Func<string, (int w, int h)?> getBuildingDefSize) {
        // Primary: direct dict lookup — no game API call, no exception risk
        if (buildingDefCache != null && buildingDefCache.TryGetValue(id, out var cs) && cs.w > 0 && cs.h > 0)
            return cs;
        // Secondary: getBuildingDef delegate (fully-registered buildings only)
        try {
            var defSize = getBuildingDefSize?.Invoke(id);
            if (defSize.HasValue && defSize.Value.w > 0 && defSize.Value.h > 0)
                return defSize.Value;
        } catch {
            // game API unavailable — fall through
        }
        // Fallback: PrefabSizeMap
        if (sizeMap != null && sizeMap.TryGetValue(id, out var sz) && sz.w > 0 && sz.h > 0)
            return sz;
        return (1, 1);
    }

    // Convenience: simulate ReadBuildingDefSize in tests
    private static (int w, int h)? ReadDefSize(int w, int h)
        => (w > 0 && h > 0) ? (w, h) : ((int, int)?)null;

    // --- Primary path: buildingDefCache ---

    [Test]
    public void ResolveBuildingSize_CacheHas4x4_ReturnsCorrectSize() {
        // Simulates HQ: _buildingDefCache populated from configTable harvest.
        var cache = new Dictionary<string, (int w, int h)> { { "Headquarters", (4, 4) } };

        var result = ResolveBuildingSize("Headquarters", sizeMap: new Dictionary<string, (int w, int h)>(), cache, _ => null);

        Assert.That(result, Is.EqualTo((4, 4)), "HQ must be 4×4 when buildingDefCache has (4,4)");
    }

    [Test]
    public void ResolveBuildingSize_CacheHas4x4_TakesPriorityOverStaleSizeMap() {
        // Core regression: PrefabSizeMap has stale (1,1), buildingDefCache has (4,4) — cache must win.
        var cache   = new Dictionary<string, (int w, int h)> { { "Headquarters", (4, 4) } };
        var sizeMap = new Dictionary<string, (int w, int h)> { { "Headquarters", (1, 1) } };

        var result = ResolveBuildingSize("Headquarters", sizeMap, cache, _ => null);

        Assert.That(result, Is.EqualTo((4, 4)),
            "buildingDefCache (4×4) must override stale PrefabSizeMap (1×1) — this was the regression");
    }

    [Test]
    public void ResolveBuildingSize_CacheHas4x4_GetBuildingDefThrows_StillReturns4x4() {
        // getBuildingDef throws in headless — buildingDefCache must shield from the exception.
        var cache = new Dictionary<string, (int w, int h)> { { "Headquarters", (4, 4) } };

        var result = ResolveBuildingSize(
            "Headquarters", sizeMap: new Dictionary<string, (int w, int h)>(), cache,
            _ => throw new InvalidOperationException("Assets unavailable in headless"));

        Assert.That(result, Is.EqualTo((4, 4)), "cache is primary and must not be affected by delegate throw");
    }

    // --- Secondary path: getBuildingDef ---

    [Test]
    public void ResolveBuildingSize_CacheMissDefHas4x4_ReturnsFromDef() {
        // Building not in cache (e.g., fully-registered tile) — delegate must provide size.
        var result = ResolveBuildingSize("Tile", sizeMap: new Dictionary<string, (int w, int h)>(), buildingDefCache: new Dictionary<string, (int w, int h)>(), _ => ReadDefSize(1, 1));

        Assert.That(result, Is.EqualTo((1, 1)), "delegate fallback works when cache misses");
    }

    // --- Tertiary path: PrefabSizeMap ---

    [Test]
    public void ResolveBuildingSize_CacheMissDefNullSizeMapHas4x4_ReturnsSizeMap() {
        // Neither cache nor def — fall back to sizeMap.
        var sizeMap = new Dictionary<string, (int w, int h)> { { "Headquarters", (4, 4) } };

        var result = ResolveBuildingSize("Headquarters", sizeMap, buildingDefCache: new Dictionary<string, (int w, int h)>(), _ => null);

        Assert.That(result, Is.EqualTo((4, 4)), "sizeMap fallback must return 4×4 when cache and def both unavailable");
    }

    [Test]
    public void ResolveBuildingSize_GetBuildingDefThrows_FallsBackToSizeMap() {
        // Cache empty, game API throws — sizeMap must rescue.
        var sizeMap = new Dictionary<string, (int w, int h)> { { "Headquarters", (4, 4) } };

        var result = ResolveBuildingSize(
            "Headquarters", sizeMap, buildingDefCache: new Dictionary<string, (int w, int h)>(),
            _ => throw new InvalidOperationException("game API unavailable"));

        Assert.That(result, Is.EqualTo((4, 4)), "exception from getBuildingDef must be silenced; sizeMap used");
    }

    // --- Default (1,1) ---

    [Test]
    public void ResolveBuildingSize_AllSourcesMissing_ReturnsDefault1x1() {
        var result = ResolveBuildingSize("UnknownBuilding", sizeMap: new Dictionary<string, (int w, int h)>(), buildingDefCache: new Dictionary<string, (int w, int h)>(), _ => null);

        Assert.That(result, Is.EqualTo((1, 1)), "missing building defaults to 1×1");
    }

    [Test]
    public void ResolveBuildingSize_CacheDegenerateZero_FallsBackToSizeMap() {
        // Cache has entry but width=0 — degenerate, must not be used.
        var cache   = new Dictionary<string, (int w, int h)> { { "BadBuilding", (0, 0) } };
        var sizeMap = new Dictionary<string, (int w, int h)> { { "BadBuilding", (3, 2) } };

        var result = ResolveBuildingSize("BadBuilding", sizeMap, cache, _ => null);

        Assert.That(result, Is.EqualTo((3, 2)), "degenerate cache entry (0×0) must fall back to sizeMap");
    }

    // --- Legitimate 1×1 building ---

    [Test]
    public void ResolveBuildingSize_Tile1x1_ReturnsCorrect1x1() {
        // 1×1 buildings must not be confused with the default fallback.
        var cache = new Dictionary<string, (int w, int h)> { { "Tile", (1, 1) } };

        var result = ResolveBuildingSize("Tile", sizeMap: new Dictionary<string, (int w, int h)>(), cache, _ => ReadDefSize(1, 1));

        Assert.That(result, Is.EqualTo((1, 1)), "legitimate 1×1 buildings must stay 1×1");
    }

}
