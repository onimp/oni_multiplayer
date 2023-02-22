using System;
using HarmonyLib;
using MultiplayerMod.multiplayer;
using MultiplayerMod.multiplayer.effect;

namespace MultiplayerMod.patch
{

    // [HarmonyPatch(typeof(ChoreConsumer), nameof(ChoreConsumer.FindNextChore))]
    public static class ChoreConsumerPatch
    {
        public static event Action<object[]> OnFindNextChore;

        public static bool Prefix(
            ChoreConsumer __instance,
            ref Chore.Precondition.Context out_context,
            ref bool __result
        )
        {
            if (MultiplayerState.MultiplayerRole != MultiplayerState.Role.Client) return true;

            var instanceId = __instance.gameObject.GetComponent<KPrefabID>().InstanceID;

            var choreContext = FindNextChoreEffect.ServerInstanceChoreDictionary.GetValueSafe(instanceId);

            if (choreContext == null || choreContext.Value.chore?.id == out_context.chore?.id)
            {
                return true;
            }

            __result = true;
            out_context = choreContext.Value;

            return false;
        }

        public static void Postfix(
            ChoreConsumer __instance,
            Chore.Precondition.Context out_context,
            ref bool __result
        )
        {
            if (MultiplayerState.MultiplayerRole != MultiplayerState.Role.Server) return;
            if (!__result) return;

            var kPrefabID = __instance.gameObject.GetComponent<KPrefabID>();

            var instanceId = kPrefabID.InstanceID;
            var choreId = out_context.chore.id;

            OnFindNextChore?.Invoke(
                new object[]
                {
                    __instance.ToString(),
                    instanceId,
                    choreId,
                    out_context.chore.GetType().ToString(),
                    Grid.PosToCell(out_context.chore.gameObject.transform.position)
                }
            );
        }
    }

}
