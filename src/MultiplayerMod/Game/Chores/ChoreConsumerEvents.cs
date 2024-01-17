using System;
using JetBrains.Annotations;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.ModRuntime.Context;
using MultiplayerMod.Multiplayer;
using MultiplayerMod.Multiplayer.CoreOperations;
using MultiplayerMod.Multiplayer.Objects;
using MultiplayerMod.Multiplayer.Objects.Reference;

namespace MultiplayerMod.Game.Chores;

// [HarmonyPatch(typeof(ChoreConsumer), nameof(ChoreConsumer.FindNextChore))]
[Obsolete(
    "Approach has been changed to go with smaller patches. See https://github.com/onimp/oni_multiplayer/discussions/301"
)]
public class ChoreConsumerEvents {

    private static readonly Core.Logging.Logger log = LoggerFactory.GetLogger<ChoreConsumerEvents>();
    public static event Action<FindNextChoreEventArgs>? FindNextChore;

    [UsedImplicitly]
    [RequireMultiplayerMode(MultiplayerMode.Host)]
    [RequireExecutionLevel(ExecutionLevel.Game)]
    public static void Postfix(ChoreConsumer __instance, Chore.Precondition.Context out_context, bool __result) {
        if (!__result)
            return;

        var choreId = out_context.chore.id;
        var reference = __instance.GetReference();

        var args = new FindNextChoreEventArgs(
            reference,
            Grid.PosToCell(__instance.transform.position),
            choreId,
            out_context.chore.GetType(),
            Grid.PosToCell(out_context.chore.gameObject.transform.position),
            out_context.isAttemptingOverride
        );
        log.Debug(
            $"Triggering {reference} {args.InstanceCell} {args.ChoreId} {args.ChoreType} {args.ChoreCell}"
        );
        FindNextChore?.Invoke(args);
    }

}

[Obsolete(
    "Approach has been changed to go with smaller patches. See https://github.com/onimp/oni_multiplayer/discussions/301"
)]
public record FindNextChoreEventArgs(
    ComponentReference<ChoreConsumer> Reference,
    int InstanceCell,
    int ChoreId,
    Type ChoreType,
    int ChoreCell,
    bool IsAttemptingOverride
);
