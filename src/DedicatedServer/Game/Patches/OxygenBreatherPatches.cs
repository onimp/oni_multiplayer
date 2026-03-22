using HarmonyLib;

namespace DedicatedServer.Game.Patches;

/// <summary>
/// Headless guard for OxygenBreather.OnSpawn.
///
/// OxygenBreather.OnSpawn has two headless NPE sites after accumulators is fixed:
///
///   Line 139: selectable.AddStatusItem(Db.Get().DuplicantStatusItems.BreathingO2Bionic, ...)
///   Line 140: selectable.AddStatusItem(Db.Get().DuplicantStatusItems.EmittingCO2, ...)
///
///   selectable is [MyCmpGet] private KSelectable, filled by InitializeComponent() via
///   GetComponent&lt;KSelectable&gt;().  In headless the MemberwiseClone save-load path copies
///   the PREFAB's KSelectable reference into each clone's field.  If KSelectable.OnPrefabInit
///   crashes (silently caught by TriggerLifecycle Phase 1), InitializeComponent may not
///   re-run the MyCmpGet resolution for this clone → field remains the prefab's stale ref
///   or null → AddStatusItem NPE → kills the dupe.
///
/// FIX (Harmony Prefix on OnSpawn):
///   Before OnSpawn runs, if selectable is null, resolve it from the GO.
///   KSelectable is always present on the dupe GO (cloned from selectableEntityTemplate).
///   AddOrGet is safe: a no-op if already present, creates a fresh one otherwise.
///   This ensures AddStatusItem calls never NPE regardless of clone/init order.
/// </summary>
[HarmonyPatch(typeof(OxygenBreather), nameof(OxygenBreather.OnSpawn))]
public static class OxygenBreatherOnSpawnPatches {

    [HarmonyPrefix]
    static void EnsureSelectableResolved(OxygenBreather __instance) {
        // selectable: [MyCmpGet] private KSelectable — may not be resolved in headless
        // if the clone's InitializeComponent ran before KSelectable was fully initialized.
        // Resolve it now: GetComponent returns the clone's own KSelectable (if present),
        // or AddOrGet creates a fresh one. Either way, AddStatusItem cannot NPE.
        if (__instance.selectable == null)
            __instance.selectable = __instance.gameObject.AddOrGet<KSelectable>();
    }
}
