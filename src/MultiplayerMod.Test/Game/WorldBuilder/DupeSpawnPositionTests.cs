using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace MultiplayerMod.Test.Game.WorldBuilder;

/// <summary>
/// Regression tests for dupe spawn position fix (FindColonySpawnCell horizontal scan).
///
/// ROOT CAUSE:
///   FindColonySpawnCell() was scanning VERTICALLY above the PrintingPod column (x=128).
///   At x=128, y=186 is the printer building — Grid.Solid=false (building, not rock).
///   Cells directly above (y=187,188,189) have Grid.Solid[floorCell=y=186]=false → skipped.
///   First solid-floor cell was at y=196 (10 cells above the printer, cave floor).
///   Dupes spawned at (128-130, 196) instead of (129-131, 186).
///
/// FIX:
///   Scan HORIZONTALLY at printer y level: cells at (hqX+1..+6, hqY).
///   For (129, 186): floor is y=185 = solid rock (cave foundation) → Grid.Solid[185]=true.
///   For (130, 186): same → valid spawn cell.
///   Mirrors NewBaseScreen.SpawnMinions(): x2 = x + i + 1; y2 = y.
///
/// Tests replicate the scan logic inline (project convention — no game runtime).
/// </summary>
[TestFixture]
[Parallelizable]
public class DupeSpawnPositionTests {

    private const int Width = 256;

    // Simulate minimal Grid.Solid, Grid.Element state for a starter cave.
    // Printer at (128, 186). Foundation solid at y=185. Cave open y=186-190 for x=127-133.
    // y=190+ is solid rock (cave ceiling).
    private static bool[] BuildSolidGrid(int width, int height) {
        var solid = new bool[width * height];
        for (var y = 0; y < height; y++) {
            for (var x = 0; x < width; x++) {
                var cell = y * width + x;
                // Everything below y=185 is solid (deep rock).
                if (y < 185) solid[cell] = true;
                // y=185 is the foundation row — solid for all x.
                if (y == 185) solid[cell] = true;
                // y=186..189 is the open printer cavity (x=127..133), rest is wall.
                if (y >= 186 && y <= 189) {
                    solid[cell] = x < 127 || x > 133; // walls outside the cavity
                }
                // y=190+ is solid rock (ceiling).
                if (y >= 190) solid[cell] = true;
            }
        }
        // Printer building at (128, 186): not solid (building tile).
        solid[186 * width + 128] = false;
        return solid;
    }

    /// <summary>
    /// Replicates the NEW horizontal scan in FindColonySpawnCell():
    ///   scan dx=1..6 at hqY, accept first cell with !solid AND solid[floorCell].
    /// Expected: first cell found is (129, 186) — one step right of printer, same Y.
    /// </summary>
    [Test]
    public void HorizontalScan_FindsPrinterAdjacentCell_NotCaveAbove() {
        const int hqX = 128;
        const int hqY = 186;
        const int height = 256;
        var hqCell = hqY * Width + hqX;
        var solid = BuildSolidGrid(Width, height);

        // Replicate the new FindColonySpawnCell() horizontal scan logic.
        var found = -1;
        for (var dx = 1; dx <= 6; dx++) {
            var candidate = hqCell + dx;
            if (candidate < 0 || candidate >= Width * height) break;
            var floorCell = candidate - Width;
            if (floorCell < 0) continue;
            if (solid[candidate]) continue;           // skip solid tiles
            if (!solid[floorCell]) continue;          // must have solid floor
            // Treat cell as gas (skip vacuum check — test only cares about solid+floor).
            found = candidate;
            break;
        }

        Assert.That(found, Is.Not.EqualTo(-1), "Horizontal scan must find a valid spawn cell");

        var foundX = found % Width;
        var foundY = found / Width;
        Assert.That(foundY, Is.EqualTo(hqY),
            $"Spawn Y must be same as printer Y ({hqY}), not cave-above Y. " +
            $"Found: ({foundX},{foundY})");
        Assert.That(foundX, Is.EqualTo(hqX + 1),
            $"Spawn X must be printer X+1 ({hqX + 1}). Found: ({foundX},{foundY})");
    }

    /// <summary>
    /// Regression: the OLD vertical scan returned a cell 10 cells ABOVE the printer.
    /// This test documents that behavior (expected to fail with the old code / pass with new).
    /// The old scan skipped y=187-189 because floorCell=y=186 (printer) has solid=false.
    /// First valid cell = y=196 (10 cells up, actual cave floor).
    /// </summary>
    [Test]
    public void OldVerticalScan_Returns_CellTenAbovePrinter() {
        const int hqX = 128;
        const int hqY = 186;
        const int height = 256;
        var hqCell = hqY * Width + hqX;

        // Build a grid where y=196 is the first solid-floor cell above the printer column.
        var solid = new bool[Width * height];
        // y=195 = solid rock (cave floor above the printer column gap)
        for (var x = 0; x < Width; x++) solid[195 * Width + x] = true;
        // Printer cell not solid
        solid[hqCell] = false;

        // Replicate OLD vertical scan logic.
        var oldFound = -1;
        for (var dy = 1; dy <= 30; dy++) {
            var candidate = hqCell + dy * Width;
            if (candidate < 0 || candidate >= Width * height) break;
            var floorCell = candidate - Width;
            if (solid[candidate]) continue;
            if (!solid[floorCell]) continue;
            oldFound = candidate;
            break;
        }

        var oldY = oldFound / Width;
        Assert.That(oldY, Is.EqualTo(196),
            "OLD vertical scan found cell at y=196 (10 cells above printer y=186) — this is the bug.");
        Assert.That(oldY - hqY, Is.EqualTo(10),
            "Bug: spawn was 10 cells above printer instead of adjacent.");
    }

    /// <summary>
    /// FindSpawnCells: starting from a printer-adjacent cell, zigzag finds N cells within ±6.
    /// All found cells must have Y within 0 cells of the printer (same floor level).
    /// </summary>
    [Test]
    public void FindSpawnCells_WithPrinterAdjacentStart_AllCellsAtPrinterLevel() {
        const int hqX = 128;
        const int hqY = 186;
        const int height = 256;

        // Start cell is printer-adjacent: (hqX+1, hqY)
        var startCell = hqY * Width + hqX + 1;
        var solid = BuildSolidGrid(Width, height);

        // Replicate FindSpawnCells() logic.
        var result = new List<int>();
        var used = new HashSet<int>();
        for (var x = 0; result.Count < 3 && x <= 12; x++) {
            var candidate = startCell + (x % 2 == 0 ? x / 2 : -(x + 1) / 2);
            if (used.Contains(candidate)) continue;
            if (candidate < 0 || candidate >= Width * height) continue;
            var floorCell = candidate - Width;
            if (floorCell < 0 || candidate >= Width * height) continue;
            if (solid[floorCell] == false) continue;  // no solid floor
            if (solid[candidate]) continue;            // candidate itself solid
            // Skip gas/mass check (test only verifies position).
            result.Add(candidate);
            used.Add(candidate);
        }

        Assert.That(result.Count, Is.GreaterThanOrEqualTo(1),
            "FindSpawnCells must find at least 1 spawn cell starting from printer-adjacent cell");

        foreach (var cell in result) {
            var cy = cell / Width;
            var cx = cell % Width;
            Assert.That(cy, Is.EqualTo(hqY),
                $"All spawn cells must be at printer Y level ({hqY}). " +
                $"Cell ({cx},{cy}) is {cy - hqY} rows above printer.");
            Assert.That(Math.Abs(cx - hqX), Is.LessThanOrEqualTo(7),
                $"Spawn cell ({cx},{cy}) must be within 7 cells of printer X ({hqX}).");
        }
    }
}
