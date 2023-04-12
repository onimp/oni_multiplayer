using HarmonyLib;
using MultiplayerMod.Game.Chores;
using MultiplayerMod.Multiplayer.State;

namespace MultiplayerMod.Multiplayer.Patches;

//[HarmonyPatch(typeof(ChoreConsumer), nameof(ChoreConsumer.FindNextChore))]
public class ChoreConsumerPatch {

    // ReSharper disable once InconsistentNaming
    public static bool Prefix(ChoreConsumer __instance, ref Chore.Precondition.Context out_context, ref bool __result) {
        if (MultiplayerState.Role != MultiplayerRole.Client)
            return true;

        var instanceId = __instance.gameObject.GetComponent<KPrefabID>().InstanceID;
        var choreContext = HostChores.Index.GetValueSafe(instanceId);
        __result = choreContext != null;
        if (choreContext != null) out_context = choreContext.Value;
        HostChores.Index.Remove(instanceId);

        return false;
    }
}
