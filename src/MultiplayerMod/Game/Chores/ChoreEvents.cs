using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using MultiplayerMod.ModRuntime.Context;
using MultiplayerMod.Multiplayer;
using MultiplayerMod.Multiplayer.CoreOperations;

namespace MultiplayerMod.Game.Chores;

[HarmonyPatch]
public static class ChoreEvents {

    public static event Action<CreateNewChoreArgs>? CreateNewChore;

    [UsedImplicitly]
    private static IEnumerable<MethodBase> TargetMethods() {
        return ChoreList.SupportedChores
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
    [RequireExecutionLevel(ExecutionLevel.Game)]
    [RequireMultiplayerMode(MultiplayerMode.Host)]
    private static void Chore_Constructor(Chore __instance, object[] __args) {
        var type = __instance.GetType();
        if (!ChoreList.SupportedChores.Contains(type) &&
            !(type.IsGenericType && ChoreList.SupportedChores.Contains(type.GetGenericTypeDefinition()))) {
            return;
        }

        CreateNewChore?.Invoke(new CreateNewChoreArgs(__instance.GetType(), __args));
    }
}
