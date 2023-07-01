using HarmonyLib;
using MultiplayerMod.Multiplayer.State;
using MultiplayerMod.Multiplayer.World;

namespace MultiplayerMod.Multiplayer.Patches;

//[HarmonyPatch(typeof(ChoreConsumer), nameof(ChoreConsumer.FindNextChore))]
public class ChoreConsumerPatch {

    // ReSharper disable once InconsistentNaming
    public static bool Prefix(ChoreConsumer __instance, ref Chore.Precondition.Context out_context, ref bool __result) {
        if (MultiplayerGame.Role != MultiplayerRole.Client)
            return true;

        var instanceId = __instance.gameObject.GetComponent<KPrefabID>().InstanceID;
        var queue = HostChores.Index.GetValueSafe(instanceId);
        var choreContext = (queue?.Count ?? 0) > 0 ? queue?.Dequeue() : null;
        __result = choreContext != null;
        if (choreContext != null) out_context = choreContext.Value;

        return false;
    }
}
