using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace DedicatedServer.Game.Patches;

/// <summary>
/// Randomises idle cell selection so dupes spread out instead of all converging to the same cell.
///
/// Root cause: IdleCellQuery.IsMatch() overwrites targetCell with EVERY valid cell found
/// during BFS traversal — the last valid cell within maxCost distance is returned.
/// All dupes start at the same spawn cell → same BFS traversal order → same last-valid cell
/// → all 3 dupes target the same idle cell → they pile up at one corridor end.
///
/// Fix (two Transpilers + one Postfix):
///   1. Reset Postfix     — clears _validCells on each query reset.
///   2. IsMatch Transpiler — intercepts the targetCell assignment to also accumulate the cell
///      in _validCells (dup + call AccumulateCell before the original stfld).
///   3. GetResultCell Transpiler — replaces the field-load return with GetRandomValidCell()
///      which picks a uniformly random entry from all valid cells found during the traversal.
///
/// Safe to Harmony: IdleCellQuery derives from PathFinderQuery (not KMonoBehaviour).
/// IsMatch/GetResultCell are overrides of virtual methods on a non-KMonoBehaviour class → safe.
/// Reset is a concrete (non-virtual) method → safe.
/// No optional parameters on any patched method → no JIT deadlock risk.
/// </summary>
[HarmonyPatch(typeof(IdleCellQuery))]
public static class IdleCellQueryPatch {

    // All valid cells seen during the current BFS traversal.
    // IdleCellQuery is a shared static singleton (PathFinderQueries.idleCellQuery);
    // BFS runs single-threaded so no lock needed.
    private static readonly List<int> _validCells = new();
    private static readonly System.Random _rng = new();

    // ── Reset: clear accumulated cells for the new query ────────────────────────
    [HarmonyPostfix]
    [HarmonyPatch(nameof(IdleCellQuery.Reset))]
    static void Reset_Postfix() {
        _validCells.Clear();
    }

    // ── IsMatch: accumulate every valid cell found during BFS ───────────────────
    // Transpiler finds the `stfld IdleCellQuery::targetCell` instruction and inserts
    // a Dup + call to AccumulateCell before it, keeping the original assignment intact.
    // Stack state at the stfld instruction: [ this, cell ]
    // After inserting:                      [ this, cell, cell ] → call → [ this, cell ] → stfld
    [HarmonyTranspiler]
    [HarmonyPatch(nameof(IdleCellQuery.IsMatch))]
    static IEnumerable<CodeInstruction> IsMatch_Transpiler(IEnumerable<CodeInstruction> instructions) {
        var targetCellField = AccessTools.Field(typeof(IdleCellQuery), "targetCell");
        foreach (var instr in instructions) {
            if (instr.StoresField(targetCellField)) {
                // Insert: dup (copy cell value) + call AccumulateCell(cell)
                yield return new CodeInstruction(OpCodes.Dup);
                yield return CodeInstruction.Call(typeof(IdleCellQueryPatch), nameof(AccumulateCell));
            }
            yield return instr;
        }
    }

    public static void AccumulateCell(int cell) => _validCells.Add(cell);

    // ── GetResultCell: return a uniformly random valid cell instead of the last one ─
    // Replaces the entire method body with: call GetRandomValidCell(); ret
    [HarmonyTranspiler]
    [HarmonyPatch(nameof(IdleCellQuery.GetResultCell))]
    static IEnumerable<CodeInstruction> GetResultCell_Transpiler(IEnumerable<CodeInstruction> _) {
        yield return CodeInstruction.Call(typeof(IdleCellQueryPatch), nameof(GetRandomValidCell));
        yield return new CodeInstruction(OpCodes.Ret);
    }

    public static int GetRandomValidCell() {
        if (_validCells.Count == 0) return Grid.InvalidCell;
        return _validCells[_rng.Next(_validCells.Count)];
    }
}
