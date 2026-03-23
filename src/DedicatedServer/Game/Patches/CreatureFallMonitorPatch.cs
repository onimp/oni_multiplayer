using HarmonyLib;

namespace DedicatedServer.Game.Patches;

/// <summary>
/// Fixes swimming creatures (Pacu) stuck in cells where Grid.mass &lt; IsSubstantialLiquid threshold.
///
/// Root cause: Grid.IsSubstantialLiquid(cell) checks:
///   element.IsLiquid AND Grid.mass[cell] &gt;= element.defaultValues.mass * 0.35f
/// At water-biome boundaries, cells can have a liquid element (Grid.elementIdx shows liquid)
/// but Grid.mass is near-zero — the mass threshold is too strict for cells at the surface
/// of a liquid body or where SimDLL has not yet fully populated mass data.
///
/// Result: CreatureFallMonitor.CanSwimAtCurrentLocation() returns false for Pacu at these cells
/// → ShouldFall() returns true → FallStates starts → PlayAnim NPE in headless →
/// StateMachine.Instance.error=true → SM transitions blocked → fish permanently stuck.
///
/// Fix: after the standard check, if def.canSwim is true and the cell's element IS a liquid type
/// (Grid.Element[cell].IsLiquid), override result to true. This is more reliable than
/// Grid.mass (unreliable at boundary/startup) while still correctly excluding gas cells.
/// Fish in genuinely gas cells (Grid.Element.IsLiquid=false) are unaffected and still fall.
/// </summary>
[HarmonyPatch(typeof(CreatureFallMonitor.Instance), "CanSwimAtCurrentLocation")]
public static class CreatureFallMonitorCanSwimPatch {

    static void Postfix(CreatureFallMonitor.Instance __instance, ref bool __result) {
        if (!__result && __instance.def.canSwim) {
            var cell = Grid.PosToCell(__instance.transform.GetPosition());
            if (Grid.IsValidCell(cell)) {
                var element = Grid.Element[cell];
                if (element != null && element.IsLiquid) {
                    __result = true;
                }
            }
        }
    }
}
