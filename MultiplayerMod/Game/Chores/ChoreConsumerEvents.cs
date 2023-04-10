using System;
using HarmonyLib;
using MultiplayerMod.Multiplayer.State;

namespace MultiplayerMod.Game.Chores;

// [HarmonyPatch(typeof(ChoreConsumer), nameof(ChoreConsumer.FindNextChore))]
public static class ChoreConsumerEvents {

    public static event EventHandler<FindNextChoreEventArgs> FindNextChore;

    public static bool Prefix(ChoreConsumer __instance, ref Chore.Precondition.Context out_context, ref bool __result) {
        if (MultiplayerState.Role != MultiplayerRole.Client)
            return true;

        var instanceId = __instance.gameObject.GetComponent<KPrefabID>().InstanceID;
        var choreContext = HostChores.Index.GetValueSafe(instanceId);
        if (choreContext == null || choreContext.Value.chore?.id == out_context.chore?.id)
            return true;

        __result = true;
        out_context = choreContext.Value;

        return false;
    }

    public static void Postfix(ChoreConsumer __instance, Chore.Precondition.Context out_context, ref bool __result) {
        if (MultiplayerState.Role != MultiplayerRole.Host)
            return;

        if (!__result)
            return;

        var kPrefabID = __instance.gameObject.GetComponent<KPrefabID>();
        var instanceId = kPrefabID.InstanceID;
        var choreId = out_context.chore.id;

        FindNextChore?.Invoke(
            __instance,
            new FindNextChoreEventArgs {
                InstanceId = instanceId,
                ChoreId = choreId,
                ChoreType = out_context.chore.GetType(),
                ChoreCell = Grid.PosToCell(out_context.chore.gameObject.transform.position)
            }
        );
    }

}

public class FindNextChoreEventArgs : EventArgs {
    public int InstanceId { get; set; }
    public int ChoreId { get; set; }
    public Type ChoreType { get; set; }
    public int ChoreCell { get; set; }
}
