using System;
using HarmonyLib;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.Multiplayer.State;

namespace MultiplayerMod.Game.Events;

//[HarmonyPatch(typeof(ChoreConsumer), nameof(ChoreConsumer.FindNextChore))]
public class ChoreConsumerEvents {

    private static readonly Core.Logging.Logger log = LoggerFactory.GetLogger<ChoreConsumerEvents>();
    public static event Action<FindNextChoreEventArgs> FindNextChore;

    public static void Postfix(ChoreConsumer __instance, Chore.Precondition.Context out_context, ref bool __result) {
        if (MultiplayerState.Role != MultiplayerRole.Host)
            return;

        if (!__result)
            return;

        var kPrefabID = __instance.gameObject.GetComponent<KPrefabID>();
        var instanceId = kPrefabID.InstanceID;
        var choreId = out_context.chore.id;

        var args = new FindNextChoreEventArgs {
            InstanceId = instanceId,
            InstanceString = __instance.ToString(),
            InstanceCell = Grid.PosToCell(__instance.transform.position),
            ChoreId = choreId,
            ChoreType = out_context.chore.GetType(),
            ChoreCell = Grid.PosToCell(out_context.chore.gameObject.transform.position),
            IsAttemptingOverride = out_context.isAttemptingOverride
        };
        log.Debug(
            $"Triggering {args.InstanceId} {args.InstanceString} {args.InstanceCell} {args.ChoreId} {args.ChoreType} {args.ChoreCell}"
        );
        FindNextChore?.Invoke(
            args
        );
    }

}

public class FindNextChoreEventArgs : EventArgs {
    public int InstanceId { get; init; }

    public string InstanceString { get; init; }

    public int InstanceCell { get; init; }
    public int ChoreId { get; init; }
    public Type ChoreType { get; init; }
    public int ChoreCell { get; init; }
    public bool IsAttemptingOverride { get; init; }
}
