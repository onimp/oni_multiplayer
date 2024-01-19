using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using MultiplayerMod.Game.Chores;
using MultiplayerMod.Game.Chores.Types;
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
                config => config.StatesTransitionSync.Status == StatesTransitionStatus.On
            )
            .Select(config => config.StatesTransitionSync.StateType.GetMethod("InitializeStates"));
    }

    [UsedImplicitly]
    [HarmonyPostfix]
    [RequireMultiplayerMode(MultiplayerMode.Client)]
    [RequireExecutionLevel(ExecutionLevel.Game)]
    public static void InitializeStatesPatch(StateMachine __instance) {
        var config = ChoreList.Config[__instance.GetType().DeclaringType].StatesTransitionSync;

        foreach (var stateTransitionConfig in config.StateTransitionConfigs.Where(
                     it => it.TransitionType == TransitionTypeEnum.Exit
                 )) {
            var stateToBeSynced = stateTransitionConfig.GetMonitoredState(__instance);
            Runtime.Instance.Dependencies.Get<StatesManager>().ReplaceWithWaitState(stateToBeSynced);
        }
    }
}
