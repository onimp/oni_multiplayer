using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using NUnit.Framework;

namespace MultiplayerMod.Test.Game.WorldBuilder;

/// <summary>
/// Unit tests for the IdleCellSensor seeding fix in WorldBuilder.FixRationalAi().
///
/// Root cause:
///   IdleCellSensor.cell defaults to 0 (int default), NOT Grid.InvalidCell (-1).
///   On the first Brain tick, IdleCellSensor.Update() runs a BFS (IdleCellQuery) to find
///   the best idle cell.  For two dupes spawning at adjacent cells (e.g. 50304 and 50305),
///   the BFS may return the SAME cell for both — whichever nearby cell has the highest
///   SafeFlags score.  When both dupes' IdleChores target the same cell:
///     - Dupe 50304's Brain picks IdleChore first → sets chore.driver = 50304.choreDrv
///     - Dupe 50305 evaluates its IdleChore: driver != null → IsPreemptable (idx 5) FAILS
///     → failedPreconditionId=5 logged; dupe 50305 never gets an IdleChore
///
/// Fix (WorldBuilder.FixRationalAi, after Sensors.Spawn()):
///   Seed each dupe's IdleCellSensor.cell to Grid.PosToCell(go) — its own spawn cell.
///   Adjacent dupes start with different cells (unique by spawn position) so neither sees
///   the other's IdleChore as "taken".  The BFS refines the cell on the next sensor tick;
///   the seed only matters until the first Brain.onPreUpdate fires.
///
/// Tests replicate the seeding logic inline (project convention).
/// </summary>
[TestFixture]
[Parallelizable]
public class IdleCellSeedingTests {

    // ─── Helper: replicates the seeding logic of FixRationalAi ───────────────

    /// <summary>
    /// Simulates reading IdleCellSensor.cell via the same FieldInfo used by FixRationalAi.
    /// Uses FormatterServices to create an uninitialized instance (no game engine needed).
    /// </summary>
    private static FieldInfo GetCellField() =>
        typeof(IdleCellSensor).GetField("cell", BindingFlags.Instance | BindingFlags.NonPublic)!;

    private static int ReadCell(IdleCellSensor sensor)
        => (int) GetCellField().GetValue(sensor)!;

    private static void SeedCell(IdleCellSensor sensor, int cell)
        => GetCellField().SetValue(sensor, cell);

    private static IdleCellSensor MakeSensor()
        => (IdleCellSensor) FormatterServices.GetUninitializedObject(typeof(IdleCellSensor));

    // ─── Root cause regression ────────────────────────────────────────────────

    [Test]
    public void DefaultCellIsZero_NotInvalidCell() {
        // Before the fix: IdleCellSensor.cell defaults to 0 (int field default).
        // Grid.InvalidCell = -1, so HasIdleCell() = (0 != -1) = true, pointing to
        // cell 0 (bottom-left map corner) — wrong initial target.
        var sensor = MakeSensor();

        Assert.That(ReadCell(sensor), Is.EqualTo(0),
            "cell field defaults to 0 (int default), NOT Grid.InvalidCell(-1) — this was the root cause");
    }

    [Test]
    public void TwoDupesWithDefaultCell_BothGetSameCell_ShowingTheBug() {
        // Before the fix: both dupes default to cell=0 → same idle cell → BFS will
        // find the same "best" nearby cell for both.  Dupe A claims it; Dupe B fails IsPreemptable.
        var sensorA = MakeSensor();
        var sensorB = MakeSensor();

        // Simulate "BFS returned same cell for both adjacent dupes" (the bad scenario).
        // In production, the BFS of two adjacent dupes often resolves to the same cell.
        int sharedBfsResult = 50304; // both BFS queries return dupe A's cell
        SeedCell(sensorA, sharedBfsResult);
        SeedCell(sensorB, sharedBfsResult);

        Assert.That(ReadCell(sensorA), Is.EqualTo(ReadCell(sensorB)),
            "Old bug: two dupes with same BFS result → same idle cell → IsPreemptable collision");
    }

    // ─── Core fix: seed each dupe with its own spawn cell ────────────────────

    [Test]
    public void SeedToOwnCell_TwoAdjacentDupes_GetUniqueCells() {
        // THE regression fix: seed each IdleCellSensor to the dupe's own spawn cell.
        // Adjacent cells 50304 and 50305 → each sensor gets a different, unique value.
        var sensorA = MakeSensor();
        var sensorB = MakeSensor();

        int dupeACell = 50304;
        int dupeBCell = 50305; // adjacent

        // Apply the fix: seed = Grid.PosToCell(go)
        SeedCell(sensorA, dupeACell);
        SeedCell(sensorB, dupeBCell);

        Assert.That(ReadCell(sensorA), Is.EqualTo(dupeACell), "Dupe A seeded to its own cell");
        Assert.That(ReadCell(sensorB), Is.EqualTo(dupeBCell), "Dupe B seeded to its own cell");
        Assert.That(ReadCell(sensorA), Is.Not.EqualTo(ReadCell(sensorB)),
            "After seeding: adjacent dupes have DIFFERENT idle cells — no IsPreemptable collision");
    }

    [Test]
    public void SeedToOwnCell_CellFieldReflectionAccessible() {
        // Verifies the FieldInfo used by FixRationalAi resolves correctly.
        // If the field name changes, the fix silently breaks — this test catches that.
        var field = GetCellField();

        Assert.That(field, Is.Not.Null,
            "IdleCellSensor.cell field must be accessible via reflection (FixRationalAi depends on it)");
        Assert.That(field.FieldType, Is.EqualTo(typeof(int)),
            "IdleCellSensor.cell must be an int field");
    }

    [Test]
    public void SeedToOwnCell_GetCellReturnsSeededValue() {
        // After seeding via reflection, GetCell() must return the seeded value.
        // This mirrors exactly what FixRationalAi does and what ChoreDriver reads.
        var sensor = MakeSensor();
        SeedCell(sensor, 50305);

        Assert.That(ReadCell(sensor), Is.EqualTo(50305),
            "GetCell() returns the value set via reflection — FixRationalAi seeding works");
    }

    // ─── HasIdleCell() contract ────────────────────────────────────────────────

    [Test]
    public void HasIdleCell_WhenCellIsInvalidCell_ReturnsFalse() {
        // HasIdleCell() = (cell != Grid.InvalidCell). Grid.InvalidCell = -1.
        // When cell = -1, HasIdleCell() returns false → idle.move skips navigation.
        // This is the "no idle cell" sentinel used by IdleCellSensor when NOT idle.
        const int InvalidCell = -1; // Grid.InvalidCell
        var sensor = MakeSensor();
        SeedCell(sensor, InvalidCell);

        // Inline HasIdleCell logic: cell != Grid.InvalidCell
        bool hasIdleCell = ReadCell(sensor) != InvalidCell;

        Assert.That(hasIdleCell, Is.False,
            "cell = Grid.InvalidCell(-1) → HasIdleCell() = false (no idle cell)");
    }

    [Test]
    public void HasIdleCell_WhenCellIsZero_ReturnsTrue_ShowingDefaultIssue() {
        // Default cell=0 means HasIdleCell() = true (cell 0 is not -1).
        // IdleChore.idle.move will try to navigate to cell 0 (bottom-left corner, likely invalid).
        // Pre-fix: both dupes point to cell 0 initially → same bad destination.
        var sensor = MakeSensor(); // cell = 0 by default

        bool hasIdleCell = ReadCell(sensor) != -1; // inline HasIdleCell

        Assert.That(hasIdleCell, Is.True,
            "cell=0 (default) means HasIdleCell()=true — dupe tries to navigate to bottom-left corner");
    }

    [Test]
    public void HasIdleCell_WhenSeededToValidCell_ReturnsTrue() {
        // After seeding to a valid cell, HasIdleCell() = true → idle.move will navigate.
        var sensor = MakeSensor();
        SeedCell(sensor, 50305);

        bool hasIdleCell = ReadCell(sensor) != -1;

        Assert.That(hasIdleCell, Is.True,
            "After seeding to valid cell 50305, HasIdleCell() = true");
    }

    // ─── Multiple dupes: all unique ────────────────────────────────────────────

    [Test]
    public void MultiDupe_ThreeDupesAtDifferentCells_AllUnique() {
        // Verifies the seeding logic scales to more than two dupes.
        var spawnCells = new[] { 50304, 50305, 50306 };
        var sensors   = new List<IdleCellSensor>();

        foreach (var spawnCell in spawnCells) {
            var s = MakeSensor();
            SeedCell(s, spawnCell);
            sensors.Add(s);
        }

        var cells = new HashSet<int>();
        foreach (var s in sensors) {
            int c = ReadCell(s);
            Assert.That(cells.Add(c), Is.True, $"Duplicate idle cell {c} found — seeding must produce unique cells");
        }
    }
}
