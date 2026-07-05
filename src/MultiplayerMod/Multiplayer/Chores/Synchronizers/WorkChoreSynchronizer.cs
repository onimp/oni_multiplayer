using System.Collections.Generic;
using HarmonyLib;
using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.ModRuntime.Context;
using MultiplayerMod.Multiplayer.Commands.Gameplay;
using MultiplayerMod.Multiplayer.Commands.Objects;
using MultiplayerMod.Network;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.Chores.Synchronizers;

// Duplicant work sync for the generic WorkChore<WorkableType> (dig / build / sweep / deliver / research /
// cook / operate / ...). WorkChore<T> is an open generic, so it can't use the closed-type DSL
// ChoreSynchronizer. Instead we hook the *non-generic* Workable seam that every WorkChore shares:
// StartWork / WorkTick / CompleteWork. Chore creation + driver assignment are already replicated
// (ChoreDriverSynchronization + CreateChore), so both host and client run the *same* WorkChore on the same
// duplicant toward the same workable, which sits on a fixed grid cell identical on both machines - the
// client's copy of the state machine navigates to it and enters `work` on its own.
//
// This class does two things:
//
//   1. Position drift (both StartWork and CompleteWork): snap the client duplicant onto the host's cell.
//      We must NOT push the client into `work` with a GoToState command: GoTo re-enters the state
//      unconditionally, which fires a second StandardWorker.StartWork on an already-Working duplicant
//      ("state should be idle but it's Working").
//
//   2. Completion timing gate (Layer 1). Work speed depends on unsynced dupe attributes/skills, so even
//      starting the same tick the two machines finish a stint at different times. We host-gate it:
//      Workable.WorkTick returns true the instant a stint is done (that's what drives StandardWorker into
//      its completion path), so on the CLIENT we force WorkTick to never report completion on its own -
//      the dupe keeps animating at the workable - and only let it complete once the host has sent
//      SyncWorkComplete for this workable. The completion then flows through the worker's own state machine
//      exactly as a natural finish would, so there is still no forced GoToState / double StartWork. This
//      subsumes the ad-hoc client suppressions for mop / harvest / toilet: their side effects are cut at
//      the specific-method level (SpawnSomeFruit / MopCell / FlushMultiple), while the *timing* of every
//      work stint is now uniformly host-driven here.
[Dependency, UsedImplicitly]
[HarmonyPatch(typeof(Workable))]
public class WorkChoreSynchronizer {

    private static readonly Core.Logging.Logger log = LoggerFactory.GetLogger<WorkChoreSynchronizer>();

    private static IMultiplayerServer server = null!;
    private static MultiplayerGame multiplayer = null!;
    private static ExecutionLevelManager manager = null!;
    private static ChoresPatcher processor = null!;

    // Client-side gate state: how many work completions the host has signalled for a workable (keyed by
    // its Unity instance id) that the client has not yet applied. A count (not a flag) so a workable that
    // is worked repeatedly - a research station, a generator - stays 1:1 with the host across stints.
    // Populated by SyncWorkComplete.MarkHostCompleted, drained in WorkTickPostfix. Runs entirely on the
    // Unity main thread (commands + Harmony patches), so no locking is needed.
    private static readonly Dictionary<int, int> pendingHostCompletions = new();

    // Client-side fallback timer (keyed by workable instance id): game-seconds a workable has spent animating
    // PAST its own natural completion while still waiting for the host's SyncWorkComplete. The gate would
    // otherwise loop the work animation forever if the host signal is late (host lag) or was dropped
    // (SyncWorkComplete couldn't resolve the workable on this client - e.g. a just-dug diggable). Once this
    // exceeds fallbackGraceSeconds we complete locally so a dupe can never get permanently stuck. This is
    // safe because every work side effect (dug cells, built buildings, research, harvested items) is
    // host-authoritative or already suppressed on the client, so a local completion only releases the
    // animation and lets the ChoreDriver advance - the host's result streams still reconcile actual state.
    private static readonly Dictionary<int, float> fallbackElapsed = new();

    // How long (game-seconds of active work) to keep animating past our own finish before giving up on the
    // host signal and completing locally. Comfortably above normal reliable-lane latency, low enough that a
    // stuck dupe recovers within a few seconds instead of looping indefinitely.
    private const float fallbackGraceSeconds = 5f;

    public WorkChoreSynchronizer(
        IMultiplayerServer server,
        MultiplayerGame multiplayer,
        ExecutionLevelManager manager,
        ChoresPatcher processor
    ) {
        WorkChoreSynchronizer.server = server;
        WorkChoreSynchronizer.multiplayer = multiplayer;
        WorkChoreSynchronizer.manager = manager;
        WorkChoreSynchronizer.processor = processor;
    }

    [HarmonyPostfix, UsedImplicitly]
    [HarmonyPatch(nameof(Workable.StartWork))]
    private static void StartWorkPostfix(Workable __instance, WorkerBase __0) {
        if (ReplicationGate.IsActiveHost(multiplayer, manager)) {
            // Only snap position; the client's own state machine enters `work` on its own (see class doc).
            if (IsSyncedWorkChore(__0))
                server.Send(new SynchronizeObjectPosition(__0.gameObject));
            return;
        }

        // Client: a fresh stint begins on this workable, so clear any stale fallback timer - the grace
        // period must be measured from THIS stint's own completion, not a previous stint's.
        if (ReplicationGate.IsActiveClient(multiplayer, manager))
            fallbackElapsed.Remove(__instance.gameObject.GetInstanceID());
    }

    [HarmonyPostfix, UsedImplicitly]
    [HarmonyPatch(nameof(Workable.CompleteWork))]
    private static void CompleteWorkPostfix(Workable __instance, WorkerBase __0) {
        if (!ReplicationGate.IsActiveHost(multiplayer, manager))
            return;

        // Snap the worker's final position (broad predicate, unchanged behaviour).
        if (IsSyncedWorkChore(__0))
            server.Send(new SynchronizeObjectPosition(__0.gameObject));

        // Tell the client this stint is done so its gated copy completes in lockstep (see WorkTickPostfix)
        // instead of on its own independent clock. Scoped to the generic WorkChore<T> only, so the DSL-synced
        // chores (Idle / Pee / Attack / MoveToSafety) keep completing through their own synchronizers.
        if (IsGenericWorkChore(__0))
            server.Send(new SyncWorkComplete(__instance.gameObject));
    }

    // Client completion gate (see class doc, point 2). Runs after the original WorkTick has already done
    // its per-tick work (decrement worktime, play effects), so overriding only the *return value* keeps
    // the dupe animating at 0 remaining without ever registering completion until the host signals it.
    [HarmonyPostfix, UsedImplicitly]
    [HarmonyPatch(nameof(Workable.WorkTick))]
    private static void WorkTickPostfix(Workable __instance, WorkerBase __0, float __1, ref bool __result) {
        if (!ReplicationGate.IsActiveClient(multiplayer, manager) || !IsGenericWorkChore(__0))
            return;

        var id = __instance.gameObject.GetInstanceID();

        // Host has signalled this stint is done -> complete in lockstep and drop any fallback timer.
        if (ConsumeHostCompletion(id)) {
            fallbackElapsed.Remove(id);
            __result = true;
            return;
        }

        // Host hasn't signalled yet. While our own copy is still labouring, keep animating (host-gated).
        if (!__result)
            return;

        // Our own copy just finished but the host signal is late or was dropped. Hold briefly, then complete
        // locally so the dupe can't loop its work animation forever (see fallbackElapsed doc). __1 is the
        // work tick's dt, so the timer accumulates game-time and pauses/scales with the sim, not wall clock.
        fallbackElapsed.TryGetValue(id, out var elapsed);
        elapsed += __1;
        if (elapsed >= fallbackGraceSeconds) {
            fallbackElapsed.Remove(id);
            log.Warning($"Work completion fallback fired for workable {id} after {elapsed:F1}s without host signal");
            __result = true;
            return;
        }

        fallbackElapsed[id] = elapsed;
        __result = false;
    }

    // Called on the client by SyncWorkComplete when the host finishes a work stint on this workable.
    public static void MarkHostCompleted(GameObject workable) {
        if (workable == null)
            return;
        var id = workable.GetInstanceID();
        pendingHostCompletions.TryGetValue(id, out var pending);
        pendingHostCompletions[id] = pending + 1;
    }

    private static bool ConsumeHostCompletion(int workableInstanceId) {
        if (!pendingHostCompletions.TryGetValue(workableInstanceId, out var pending) || pending <= 0)
            return false;
        if (pending <= 1)
            pendingHostCompletions.Remove(workableInstanceId);
        else
            pendingHostCompletions[workableInstanceId] = pending - 1;
        return true;
    }

    // Broad structural test (duplicant + any supported chore) used for the position snaps. The host/client
    // mode gate is applied at each call site.
    private static bool IsSyncedWorkChore(WorkerBase workerBase) {
        var current = GetDuplicantChore(workerBase);
        return current != null && processor.Supported(current);
    }

    // Narrow test: duplicant + a generic WorkChore<T> specifically. Used for the completion gate + signal
    // so it covers exactly the "generic work" (dig/build/research/cook/operate/…) and never the DSL-synced
    // chores (Idle / Pee / Attack / MoveToSafety), which complete through their own synchronizers.
    private static bool IsGenericWorkChore(WorkerBase workerBase) {
        var current = GetDuplicantChore(workerBase);
        return current != null && ChoresPatcher.IsWorkChore(current.GetType());
    }

    // The duplicant's current chore, or null if this worker isn't a duplicant (critters are out of scope).
    private static Chore? GetDuplicantChore(WorkerBase workerBase) {
        if (workerBase == null)
            return null;
        if (workerBase.gameObject.GetComponent<MinionIdentity>() == null)
            return null;
        return workerBase.gameObject.GetComponent<ChoreDriver>()?.GetCurrentChore();
    }

}
