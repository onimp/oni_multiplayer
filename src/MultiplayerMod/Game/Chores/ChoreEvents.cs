using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using MultiplayerMod.Game.Chores.Types;
using MultiplayerMod.ModRuntime.Context;
using MultiplayerMod.Multiplayer;
using MultiplayerMod.Multiplayer.CoreOperations;
using MultiplayerMod.Multiplayer.Objects.Extensions;

namespace MultiplayerMod.Game.Chores;

// [HarmonyPatch]
public static class ChoreEvents {

    public static event Action<CreateNewChoreArgs>? CreateNewChore;

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
    [RequireExecutionLevel(ExecutionLevel.Multiplayer)]
    [RequireMultiplayerMode(MultiplayerMode.Host)]
    private static void Chore_Constructor(Chore __instance, object[] __args) {
        var choreId = __instance.Register();
        CreateNewChore?.Invoke(new CreateNewChoreArgs(choreId, __instance.GetType(), __args));
    }
}
