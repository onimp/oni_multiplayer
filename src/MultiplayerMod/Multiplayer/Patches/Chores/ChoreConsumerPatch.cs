using HarmonyLib;
using JetBrains.Annotations;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.ModRuntime.Context;
using MultiplayerMod.ModRuntime.StaticCompatibility;
using MultiplayerMod.Multiplayer.Objects;
using MultiplayerMod.Multiplayer.World;

namespace MultiplayerMod.Multiplayer.Patches.Chores;

// [HarmonyPatch(typeof(ChoreConsumer), nameof(ChoreConsumer.FindNextChore))]
public class ChoreConsumerPatch {

    private static readonly Core.Logging.Logger log = LoggerFactory.GetLogger<ChoreConsumerPatch>();

    [UsedImplicitly]
    public static bool Prefix(ChoreConsumer __instance, ref Chore.Precondition.Context out_context, ref bool __result) {
        if (Dependencies.Get<MultiplayerGame>().Mode != MultiplayerMode.Client)
            return true;

        if (!Dependencies.Get<ExecutionLevelManager>().LevelIsActive(ExecutionLevel.Game))
            return true;

        var reference = __instance.GetReference();
        var queue = HostChores.Index.GetValueSafe(reference);
        var currentChore = __instance.choreDriver.GetCurrentChore();
        var queueNotEmpty = (queue?.Count ?? 0) > 0;
        var needWaitForChoreCompletion = NeedWaitForChoreCompletion(currentChore);
        if (needWaitForChoreCompletion && queueNotEmpty) {
            log.Debug(
                $"{reference.Resolve()} Waiting for" +
                $" {currentChore?.choreType.Name} to finish before assigning next one from the host ({queue?.Peek().Invoke()?.chore?.choreType?.Name})."
            );
            return false;
        }
        var choreContextFunc = queueNotEmpty ? queue?.Dequeue() : null;
        var choreContext = choreContextFunc?.Invoke();
        __result = choreContext != null;
        if (choreContext != null) out_context = choreContext.Value;

        return false;
    }

    private static bool NeedWaitForChoreCompletion(Chore? currentChore) {
        if (currentChore == null) return false;

        // Idle is not finishable chore and must be interrupted.
        var choreTypeName = currentChore.choreType.Name.Length > 0
            ? currentChore.choreType.Name
            : currentChore.choreType.Id;
        if (choreTypeName.Contains("Idle")) return false;

        return !currentChore.isComplete;
    }
}
