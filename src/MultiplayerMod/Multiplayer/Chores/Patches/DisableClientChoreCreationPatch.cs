using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using MultiplayerMod.Core.Scheduling;
using MultiplayerMod.Game.Chores;
using MultiplayerMod.Game.Chores.Types;
using MultiplayerMod.ModRuntime;
using MultiplayerMod.ModRuntime.Context;
using MultiplayerMod.Multiplayer.CoreOperations;

namespace MultiplayerMod.Multiplayer.Chores.Patches;

// [HarmonyPatch(typeof(Chore))]
public static class DisableClientChoreCreationPatch {

    [UsedImplicitly]
    private static IEnumerable<MethodBase> TargetMethods() {
        return ChoreList.Config
            .Where(config => config.Value.CreationSync == CreationStatusEnum.On)
            .Select(config => config.Key)
            .Select(
                type => {
                    if (!type.IsGenericType) return type.GetConstructors()[0];
                    if (type == typeof(WorkChore<>))
                        return type.MakeGenericType(typeof(Workable)).GetConstructors()[0];

                    return type.GetConstructors()[0];
                }
            );
    }

    [UsedImplicitly]
    [HarmonyPostfix]
    [RequireMultiplayerMode(MultiplayerMode.Client)]
    [RequireExecutionLevel(ExecutionLevel.Game)]
    private static void Chore_Constructor(Chore __instance, object[] __args) {
        var type = __instance.GetType();

        Runtime.Instance.Dependencies.Get<UnityTaskScheduler>().Run(
            () => { __instance.Cancel($"Client chore {type} must be created by host only."); }
        );
    }
}
