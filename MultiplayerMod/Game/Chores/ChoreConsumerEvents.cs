using System;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.Multiplayer.State;

namespace MultiplayerMod.Game.Chores;

//[HarmonyPatch(typeof(ChoreConsumer), nameof(ChoreConsumer.FindNextChore))]
public class ChoreConsumerEvents {

    private static readonly Core.Logging.Logger log = LoggerFactory.GetLogger<ChoreConsumerEvents>();
    public static event Action<FindNextChoreEventArgs>? FindNextChore;

    // ReSharper disable once InconsistentNaming, UnusedMember.Global
    public static void Postfix(ChoreConsumer __instance, Chore.Precondition.Context out_context, ref bool __result) {
        if (Dependencies.Get<MultiplayerGame>().Role != MultiplayerRole.Host)
            return;

        if (!__result)
            return;

        var kPrefabID = __instance.gameObject.GetComponent<KPrefabID>();
        var instanceId = kPrefabID.InstanceID;
        var choreId = out_context.chore.id;

        var args = new FindNextChoreEventArgs(
            instanceId,
            __instance.ToString(),
            Grid.PosToCell(__instance.transform.position),
            choreId,
            out_context.chore.GetType(),
            Grid.PosToCell(out_context.chore.gameObject.transform.position),
            out_context.isAttemptingOverride
        );
        log.Debug(
            $"Triggering {args.InstanceId} {args.InstanceString} {args.InstanceCell} {args.ChoreId} {args.ChoreType} {args.ChoreCell}"
        );
        FindNextChore?.Invoke(
            args
        );
    }

}

public record FindNextChoreEventArgs(
    int InstanceId,
    string InstanceString,
    int InstanceCell,
    int ChoreId,
    Type ChoreType,
    int ChoreCell,
    bool IsAttemptingOverride
);
