using System;
using HarmonyLib;
using MultiplayerMod.Multiplayer.State;

namespace MultiplayerMod.Game.Chores;

//[HarmonyPatch(typeof(ChoreConsumer), nameof(ChoreConsumer.FindNextChore))]
public static class ChoreConsumerEvents {
    public static event Action<FindNextChoreEventArgs> FindNextChore;

    public static void Postfix(ChoreConsumer __instance, Chore.Precondition.Context out_context, ref bool __result) {
        if (MultiplayerState.Role != MultiplayerRole.Host)
            return;

        if (!__result)
            return;

        var kPrefabID = __instance.gameObject.GetComponent<KPrefabID>();
        var instanceId = kPrefabID.InstanceID;
        var choreId = out_context.chore.id;

        FindNextChore?.Invoke(
            new FindNextChoreEventArgs {
                InstanceId = instanceId,
                InstanceType = __instance.GetType(),
                ChoreId = choreId,
                ChoreType = out_context.chore.GetType(),
                ChoreCell = Grid.PosToCell(out_context.chore.gameObject.transform.position)
            }
        );
    }

}

public class FindNextChoreEventArgs : EventArgs {
    public int InstanceId { get; init; }

    public Type InstanceType { get; init; }
    public int ChoreId { get; init; }
    public Type ChoreType { get; init; }
    public int ChoreCell { get; init; }
}
