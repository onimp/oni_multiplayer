using System;
using System.Linq;

namespace MultiplayerMod.patch
{

    //  [HarmonyPatch(typeof(ChoreDriver), nameof(ChoreDriver.SetChore))]
    public static class ChoreDriverPatch
    {
        public static event Action<object[]> OnChoreSet;

        public static void Prefix(ChoreDriver __instance, Chore.Precondition.Context context)
        {
            if (__instance.smi.GetCurrentChore() == context.chore)
                return;

            var kPrefabID = __instance.gameObject.GetComponent<KPrefabID>();
            var instanceId = kPrefabID.InstanceID;
            var choreId = context.chore.id;

            var allChoreNames = GetAllChoreNames(kPrefabID);

            OnChoreSet?.Invoke(new object[] { instanceId, choreId, context.chore.GetType().ToString(), allChoreNames });
        }

        public static string GetAllChoreNames(KPrefabID kPrefabID)
        {
            var consumer = kPrefabID.GetComponent<ChoreConsumer>();
            return string.Join(
                "; ",
                consumer.GetProviders().Select(
                    choreProvider =>
                    {
                        return !choreProvider.choreWorldMap.TryGetValue(
                            consumer.consumerState.gameObject.GetMyParentWorldId(),
                            out var choreList
                        )
                            ? ""
                            : string.Join(", ", choreList.Select(a => a.GetType().ToString()));
                    }
                )
            );
        }
    }

}
