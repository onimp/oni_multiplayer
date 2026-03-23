using HarmonyLib;

namespace DedicatedServer.Game.Patches;

/// <summary>
/// Forces SafeCellQuery.GetFlags() to always report IsBreathable=true in headless.
///
/// Root cause: Grid.Element stays at Vacuum in headless (SimDLL gas sim does not populate
/// the C# Grid.Element array without a running client). GasBreatherFromWorldProvider.IsBreathable
/// reads Grid.Element → Vacuum → returns false → SafeCellQuery excludes ALL cells →
/// IdleCellQuery BFS finds no valid idle cell → idleCell=-1 → dupe never moves.
///
/// Primary fix: brain.OxygenBreather=null in MinionPrefab step 16c short-circuits line 60
///   flag5 = brain.OxygenBreather == null || GasBreatherFromWorldProvider...IsBreathable
/// making all cells pass without ever reaching GasBreatherFromWorldProvider.
///
/// This postfix is belt-and-suspenders: any code path that calls GetFlags() without going
/// through brain.OxygenBreather (e.g. creature AI, future refactors) still gets valid results.
/// </summary>
[HarmonyPatch(typeof(SafeCellQuery), nameof(SafeCellQuery.GetFlags))]
public static class SafeCellQueryGetFlagsPatch {

    static void Postfix(ref SafeCellQuery.SafeFlags __result) {
        // Headless: gas sim not running → Grid.Element is Vacuum everywhere.
        // Force IsBreathable so IdleCellQuery BFS can find valid distant idle cells.
        __result |= SafeCellQuery.SafeFlags.IsBreathable;
    }
}
