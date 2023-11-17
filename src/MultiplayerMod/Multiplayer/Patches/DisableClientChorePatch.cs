using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using MultiplayerMod.Core.Scheduling;
using MultiplayerMod.Game.Chores;
using MultiplayerMod.ModRuntime;
using MultiplayerMod.ModRuntime.Context;
using MultiplayerMod.Multiplayer.CoreOperations;

namespace MultiplayerMod.Multiplayer.Patches;

[HarmonyPatch(typeof(Chore))]
public static class DisableClientChorePatch {

    [UsedImplicitly]
    private static IEnumerable<MethodBase> TargetMethods() => new[] { typeof(Chore).GetConstructors()[0] };

    [UsedImplicitly]
    [HarmonyPostfix]
    [RequireMultiplayerMode(MultiplayerMode.Client)]
    [RequireExecutionLevel(ExecutionLevel.Game)]
    private static void Chore_Constructor(Chore __instance, object[] __args) {
        var type = __instance.GetType();
        if (!ChoreList.SupportedChores.Contains(type) &&
            !(type.IsGenericType && ChoreList.SupportedChores.Contains(type.GetGenericTypeDefinition()))) {
            return;
        }

        Runtime.Instance.Dependencies.Get<UnityTaskScheduler>().Run(
            () => { __instance.Cancel($"Client chore {type} must be created by host only."); }
        );
    }
}
