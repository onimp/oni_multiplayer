using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using NUnit.Framework;

namespace MultiplayerMod.Test.Game.WorldBuilder;

/// <summary>
/// Unit tests for the ResolveBuildingSize logic used by RealWorldState.GetEntitiesBytes()
/// to determine cell dimensions of each building in /api/entities.
///
/// Regression: /api/entities returned HQ w=1 h=1 even though PrefabSizeMap had 4×4.
/// Root cause: when PrefabSizeMap had a stale (1,1) entry from a failed CaptureEntitySize
/// run (spawned GO had no Building component in headless → fell through to KBoxCollider2D=0),
/// the old code accepted (1,1) and never tried GetBuildingDef.
///
/// Fix (RealWorldState.ResolveBuildingSize): GetBuildingDef is now PRIMARY — backed by static
/// _buildingDefCache populated during RegisterBuildingDefs() configTable harvest.
/// PrefabSizeMap is FALLBACK — used when GetBuildingDef is unavailable or degenerate.
///
/// Following the project convention these tests replicate the resolution logic inline rather
/// than importing the DedicatedServer.exe assembly directly.
/// </summary>
[TestFixture]
[Parallelizable]
public class ResolveBuildingSizeTests {

    /// <summary>
    /// Replicates RealWorldState.ResolveBuildingSize logic.
    /// Primary: getBuildingDef (static _buildingDefCache — not overwritten by spawn-time logic).
    /// Fallback: sizeMap (PrefabSizeMap — instance, may have stale 1×1 from failed CaptureEntitySize).
    /// </summary>
    private static (int w, int h) ResolveBuildingSize(
        string id,
        IReadOnlyDictionary<string, (int w, int h)> sizeMap,
        Func<string, (int w, int h)?> getBuildingDefSize) {
        // Primary: BuildingDef from static _buildingDefCache
        try {
            var defSize = getBuildingDefSize?.Invoke(id);
            if (defSize.HasValue && defSize.Value.w > 0 && defSize.Value.h > 0)
                return defSize.Value;
        } catch {
            // game API unavailable — fall through
        }
        // Fallback: instance PrefabSizeMap
        if (sizeMap != null && sizeMap.TryGetValue(id, out var sz) && sz.w > 0 && sz.h > 0)
            return sz;
        return (1, 1);
    }

    // Mirrors ReadBuildingDefSize from WorldBuilder:
    private static (int w, int h)? ReadDefSize(int w, int h)
        => (w > 0 && h > 0) ? (w, h) : ((int, int)?)null;

    // --- Primary path: GetBuildingDef ---

    [Test]
    public void ResolveBuildingSize_DefHas4x4_ReturnsCorrectSize() {
        // Simulates HQ with BuildingDef from _buildingDefCache (configTable harvest).
        var sizeMap = new Dictionary<string, (int w, int h)>();

        var result = ResolveBuildingSize("Headquarters", sizeMap, _ => ReadDefSize(4, 4));

        Assert.That(result, Is.EqualTo((4, 4)), "HQ must be 4×4 when def says so");
    }

    [Test]
    public void ResolveBuildingSize_DefHas4x4_TakesPriorityOverStaleSizeMap() {
        // PrefabSizeMap has stale (1,1) from a failed CaptureEntitySize run — def must win.
        var sizeMap = new Dictionary<string, (int w, int h)> { { "Headquarters", (1, 1) } };

        var result = ResolveBuildingSize("Headquarters", sizeMap, _ => ReadDefSize(4, 4));

        Assert.That(result, Is.EqualTo((4, 4)),
            "BuildingDef (4×4) must override stale PrefabSizeMap (1×1) — this was the regression");
    }

    // --- Fallback path: PrefabSizeMap ---

    [Test]
    public void ResolveBuildingSize_DefNullSizeMapHas4x4_ReturnsSizeMap() {
        // Def not available — fall back to sizeMap.
        var sizeMap = new Dictionary<string, (int w, int h)> { { "Headquarters", (4, 4) } };

        var result = ResolveBuildingSize("Headquarters", sizeMap, _ => null);

        Assert.That(result, Is.EqualTo((4, 4)), "sizeMap fallback must return 4×4 when def is unavailable");
    }

    [Test]
    public void ResolveBuildingSize_GetBuildingDefThrows_FallsBackToSizeMap() {
        // Game API throws — sizeMap must rescue.
        var sizeMap = new Dictionary<string, (int w, int h)> { { "Headquarters", (4, 4) } };

        var result = ResolveBuildingSize(
            "Headquarters", sizeMap, _ => throw new InvalidOperationException("game API unavailable"));

        Assert.That(result, Is.EqualTo((4, 4)), "exception from getBuildingDef must be silenced; sizeMap used");
    }

    // --- Default (1,1) ---

    [Test]
    public void ResolveBuildingSize_BothSourcesMissing_ReturnsDefault1x1() {
        var sizeMap = new Dictionary<string, (int w, int h)>();

        var result = ResolveBuildingSize("UnknownBuilding", sizeMap, _ => null);

        Assert.That(result, Is.EqualTo((1, 1)), "missing building defaults to 1×1");
    }

    [Test]
    public void ResolveBuildingSize_DefDegenerateZero_FallsBackToSizeMap() {
        // Def exists but width=0 — degenerate, must not be used.
        var sizeMap = new Dictionary<string, (int w, int h)> { { "BadBuilding", (3, 2) } };

        var result = ResolveBuildingSize("BadBuilding", sizeMap, _ => ReadDefSize(0, 0));

        Assert.That(result, Is.EqualTo((3, 2)), "degenerate def (0×0) must fall back to sizeMap");
    }

    // --- Legitimate 1×1 building ---

    [Test]
    public void ResolveBuildingSize_Tile1x1_ReturnsCorrect1x1() {
        // 1×1 buildings must not be confused with the default fallback.
        var sizeMap = new Dictionary<string, (int w, int h)> { { "Tile", (1, 1) } };

        var result = ResolveBuildingSize("Tile", sizeMap, _ => ReadDefSize(1, 1));

        Assert.That(result, Is.EqualTo((1, 1)), "legitimate 1×1 buildings must stay 1×1");
    }

}
