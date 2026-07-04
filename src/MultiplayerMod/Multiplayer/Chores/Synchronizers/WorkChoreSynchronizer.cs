using HarmonyLib;
using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.ModRuntime.Context;
using MultiplayerMod.Multiplayer.Commands.Objects;
using MultiplayerMod.Network;

namespace MultiplayerMod.Multiplayer.Chores.Synchronizers;

// Duplicant work sync for the generic WorkChore<WorkableType> (dig / build / sweep / deliver / ...).
//
// WorkChore<T> is an open generic, so it can't use the closed-type DSL ChoreSynchronizer. Instead we
// hook the *non-generic* Workable seam that every WorkChore shares: StartWork / CompleteWork. Chore
// creation + driver assignment are already replicated (ChoreDriverSynchronization + CreateChore), so
// both host and client run the *same* WorkChore on the same duplicant toward the same workable.
//
// The workable sits on a fixed grid cell that is identical on both machines, so the client's copy of
// the state machine navigates to it and enters `work` on its own — we must NOT also push it into
// `work` with a GoToState command: GoTo re-enters the state unconditionally, which fires a second
// StandardWorker.StartWork on an already-Working duplicant ("state should be idle but it's Working").
// So this class only corrects the residual position drift: snap the client duplicant onto the host's
// cell when work starts and again when it completes, leaving state entry to the client's own SM.
[Dependency, UsedImplicitly]
[HarmonyPatch(typeof(Workable))]
public class WorkChoreSynchronizer {

    private static IMultiplayerServer server = null!;
    private static MultiplayerGame multiplayer = null!;
    private static ExecutionLevelManager manager = null!;
    private static ChoresPatcher processor = null!;

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
    private static void StartWorkPostfix(WorkerBase __0) {
        // Only snap position; the client's own state machine enters `work` on its own (see class doc).
        if (IsSyncedWorkChore(__0))
            server.Send(new SynchronizeObjectPosition(__0.gameObject));
    }

    [HarmonyPostfix, UsedImplicitly]
    [HarmonyPatch(nameof(Workable.CompleteWork))]
    private static void CompleteWorkPostfix(WorkerBase __0) {
        if (IsSyncedWorkChore(__0))
            server.Send(new SynchronizeObjectPosition(__0.gameObject));
    }

    private static bool IsSyncedWorkChore(WorkerBase workerBase) {
        if (workerBase == null)
            return false;
        if (!manager.LevelIsActive(ExecutionLevel.Multiplayer) || multiplayer.Mode != MultiplayerMode.Host)
            return false;

        // Duplicants only for now — critters are out of scope.
        if (workerBase.gameObject.GetComponent<MinionIdentity>() == null)
            return false;

        var current = workerBase.gameObject.GetComponent<ChoreDriver>()?.GetCurrentChore();
        return current != null && processor.Supported(current);
    }

}
