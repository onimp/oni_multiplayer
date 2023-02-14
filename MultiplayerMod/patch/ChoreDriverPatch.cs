using System;
using HarmonyLib;

namespace MultiplayerMod.patch
{
    [HarmonyPatch(typeof(ChoreDriver), nameof(ChoreDriver.SetChore))]
    public static class ChoreDriverPatch
    {
        public static event Action<Chore.Precondition.Context> OnChoreSet;

        public static void Prefix(ChoreDriver __instance, Chore.Precondition.Context context)
        {
            if (__instance.smi.GetCurrentChore() == context.chore)
                return;
            var kPrefabID = __instance.gameObject.GetComponent<KPrefabID>();
            Debug.Log($"{kPrefabID} - {kPrefabID.InstanceID}");
            Debug.Log(context.chore);
            OnChoreSet?.Invoke(context);
        }
    }
}