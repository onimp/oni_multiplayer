using System;
using HarmonyLib;
using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.Core.Patch;
using MultiplayerMod.Multiplayer.Objects;
using MultiplayerMod.Multiplayer.Objects.Reference;
using MultiplayerMod.Multiplayer.State;

namespace MultiplayerMod.Game.Chores;

[HarmonyPatch(typeof(ChoreConsumer), nameof(ChoreConsumer.FindNextChore))]
public class ChoreConsumerEvents {

    private static readonly Core.Logging.Logger log = LoggerFactory.GetLogger<ChoreConsumerEvents>();
    public static event Action<FindNextChoreEventArgs>? FindNextChore;

    [UsedImplicitly]
    public static void Postfix(ChoreConsumer __instance, Chore.Precondition.Context out_context, bool __result) {
        if (!__result)
            return;

        PatchControl.RunIfSpawned(
            () => {
                if (Dependencies.Get<MultiplayerGame>().Role != MultiplayerRole.Host)
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
        );
    }

}

public record FindNextChoreEventArgs(
    ComponentReference<ChoreConsumer> Reference,
    int InstanceCell,
    int ChoreId,
    Type ChoreType,
    int ChoreCell,
    bool IsAttemptingOverride
);
