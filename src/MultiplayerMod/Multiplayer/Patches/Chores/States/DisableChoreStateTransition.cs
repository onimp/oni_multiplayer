using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using MultiplayerMod.Game.Chores;
using MultiplayerMod.ModRuntime;
using MultiplayerMod.ModRuntime.Context;
using MultiplayerMod.Multiplayer.CoreOperations;
using MultiplayerMod.Multiplayer.States;

namespace MultiplayerMod.Multiplayer.Patches.Chores.States;

[HarmonyPatch]
public static class DisableChoreStateTransition {

    [UsedImplicitly]
    private static IEnumerable<MethodBase> TargetMethods() {
        return ChoreList.Config.Values.Where(
                config => config.StateTransitionSync.Status == ChoreList.StateTransitionConfig.SyncStatus.On
            )
            .Select(config => config.StateTransitionSync.StateType.GetMethod("InitializeStates"));
    }

    [UsedImplicitly]
    [HarmonyPostfix]
    [RequireMultiplayerMode(MultiplayerMode.Client)]
    [RequireExecutionLevel(ExecutionLevel.Game)]
    public static void InitializeStatesPatch(object __instance) {
        var config = ChoreList.Config[__instance.GetType().DeclaringType].StateTransitionSync;

        var stateToBeSynced =
            (StateMachine.BaseState) __instance.GetType().GetField(config.StateToMonitorName).GetValue(__instance);
        Runtime.Instance.Dependencies.Get<StatesManager>().DisableChoreStateTransition(stateToBeSynced);
    }
}
