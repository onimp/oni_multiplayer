using MultiplayerMod.ModRuntime.Context;

namespace MultiplayerMod.Multiplayer;

// Shared host-authoritative / client-suppression gate for the work-result synchronizers (Dig, Toilet,
// Mop, Harvest, WorkChore, ...). Every one of them re-derived the same two things by hand; this folds
// them into one place:
//   1. The null guard. A Harmony patch can fire during world gen / load before the DI container has built
//      the synchronizer and populated its injected dependencies, so both are nullable here and a null in
//      the pair means "not in an active multiplayer session" -> the gate is inactive (original runs).
//   2. The level-active + mode test.
//
// Centralizing it keeps the bool-prefix footgun in exactly one place. A bool Harmony prefix must return
// true to RUN the original and false to SKIP it (this is why the synchronizers avoid [RequireMultiplayerMode],
// whose wrapper defaults a skipped prefix in a way that would also skip the original on the host). So:
//   - Client suppression (bool prefix):  return !ReplicationGate.IsActiveClient(multiplayer, manager);
//       deps still null -> IsActiveClient false -> returns true -> the original runs, the safe default.
//   - Host-authoritative replication:    if (!ReplicationGate.IsActiveHost(multiplayer, manager)) return;
public static class ReplicationGate {

    // True only on the machine that is the active multiplayer host (safe to broadcast authoritative results).
    public static bool IsActiveHost(MultiplayerGame? multiplayer, ExecutionLevelManager? manager) =>
        IsActiveSession(multiplayer, manager) && multiplayer!.Mode == MultiplayerMode.Host;

    // True only on a machine that is an active multiplayer client (should suppress a host-authoritative
    // side effect it would otherwise double-run).
    public static bool IsActiveClient(MultiplayerGame? multiplayer, ExecutionLevelManager? manager) =>
        IsActiveSession(multiplayer, manager) && multiplayer!.Mode == MultiplayerMode.Client;

    private static bool IsActiveSession(MultiplayerGame? multiplayer, ExecutionLevelManager? manager) =>
        multiplayer != null && manager != null && manager.LevelIsActive(ExecutionLevel.Multiplayer);

}
