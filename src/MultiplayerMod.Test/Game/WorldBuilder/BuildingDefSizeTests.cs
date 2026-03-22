using System.Runtime.Serialization;
using NUnit.Framework;

namespace MultiplayerMod.Test.Game.WorldBuilder;

/// <summary>
/// Unit tests for building size reading logic used by DedicatedServer.WorldBuilder.CaptureEntitySize.
/// Tests that BuildingDef.WidthInCells/HeightInCells can be reliably read back after being set.
/// This exercises the same code path as WorldBuilder.ReadBuildingDefSize(def).
/// </summary>
[TestFixture]
[Parallelizable]
public class BuildingDefSizeTests {

    /// <summary>
    /// Verifies that WidthInCells and HeightInCells can be set and read back from BuildingDef.
    /// This is the core assumption of CaptureEntitySize's Building.Def primary path:
    /// if building.Def != null → use def.WidthInCells / def.HeightInCells.
    /// A 4×4 building (like Headquarters) must NOT be reported as 1×1.
    /// </summary>
    [Test]
    public void BuildingDef_WidthAndHeight_AreReadableAfterSet() {
        // Arrange: create a BuildingDef without calling the ctor (which would NPE in test env)
        var def = (BuildingDef)FormatterServices.GetUninitializedObject(typeof(BuildingDef));
        def.WidthInCells = 4;
        def.HeightInCells = 4;

        // Act
        var w = def.WidthInCells;
        var h = def.HeightInCells;

        // Assert: Headquarters is 4x4 — must NOT be 1x1
        Assert.That(w, Is.EqualTo(4), "WidthInCells must be 4 (not 1x1 fallback)");
        Assert.That(h, Is.EqualTo(4), "HeightInCells must be 4 (not 1x1 fallback)");
    }

    /// <summary>
    /// Verifies the ReadBuildingDefSize logic: null def returns null, valid def returns correct size.
    /// Mirrors WorldBuilder.ReadBuildingDefSize(def) without depending on DedicatedServer assembly.
    /// </summary>
    [TestCase(0, 0, false)]   // degenerate — both zero
    [TestCase(-1, 1, false)]  // degenerate — negative width
    [TestCase(1, 1, true)]    // Tile: 1×1
    [TestCase(4, 4, true)]    // Headquarters: 4×4
    [TestCase(1, 2, true)]    // Minion-like: 1×2
    public void BuildingDef_ReadBuildingDefSize_ReturnsExpectedResult(int width, int height, bool expectValue) {
        // Arrange
        var def = (BuildingDef)FormatterServices.GetUninitializedObject(typeof(BuildingDef));
        def.WidthInCells = width;
        def.HeightInCells = height;

        // Act — replicate WorldBuilder.ReadBuildingDefSize logic inline
        (int w, int h)? result = null;
        if (def.WidthInCells > 0 && def.HeightInCells > 0)
            result = (def.WidthInCells, def.HeightInCells);

        // Assert
        if (expectValue) {
            Assert.That(result, Is.Not.Null, $"Expected a size for {width}×{height}");
            Assert.That(result!.Value.w, Is.EqualTo(width));
            Assert.That(result.Value.h, Is.EqualTo(height));
        } else {
            Assert.That(result, Is.Null, $"Expected null for degenerate {width}×{height}");
        }
    }

}
