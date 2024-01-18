using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using MultiplayerMod.ModRuntime.Context;
using MultiplayerMod.Multiplayer;
using MultiplayerMod.Multiplayer.CoreOperations;

namespace MultiplayerMod.Game.Chores.States;

[HarmonyPatch]
public static class ChoreStateEvents {
    public static event Action<ChoreTransitStateArgs>? OnStateTransition;

    [UsedImplicitly]
    private static IEnumerable<MethodBase> TargetMethods() {
        var choreSyncConfigs = ChoreList.Config.Values.Where(
            config => config.StatesTransitionSync.Status == ChoreList.StatesTransitionConfig.SyncStatus.On
        ).ToArray();
        var targetMethods = choreSyncConfigs
            .Select(config => config.StatesTransitionSync.StateType.GetMethod("InitializeStates")).ToArray();
        return targetMethods;
    }

    [UsedImplicitly]
    [HarmonyPostfix]
    [RequireExecutionLevel(ExecutionLevel.Game)]
    [RequireMultiplayerMode(MultiplayerMode.Host)]
    public static void Postfix(StateMachine __instance) {
        var choreType = __instance.GetType().DeclaringType;
        var config = ChoreList.Config[choreType].StatesTransitionSync;
        foreach (var stateTransitionConfig in config.StateTransitionConfigs) {
            if (stateTransitionConfig.TransitionType == ChoreList.StateTransitionConfig.TransitionTypeEnum.Exit) {
                BindExitCallback(__instance, stateTransitionConfig);
            }
        }
    }

    private static void BindExitCallback(StateMachine sm, ChoreList.StateTransitionConfig config) {
        var state = config.GetMonitoredState(sm);
        var method = state.GetType().GetMethods()
            .Single(method => method.Name == "Exit" && method.GetParameters().Length == 2);
        var dlgt = Delegate.CreateDelegate(
            method.GetParameters()[1].ParameterType,
            config,
            typeof(ChoreStateEvents).GetMethod(nameof(OnStateExit), BindingFlags.NonPublic | BindingFlags.Static)
        );
        method.Invoke(state, new object[] { "Trigger Multiplayer event", dlgt });
    }

    private static void OnStateExit(ChoreList.StateTransitionConfig config, StateMachine.Instance smi) {
        var chore = (Chore) smi.GetMaster();
        var goToStack = (Stack<StateMachine.BaseState>) smi.GetType().GetField("gotoStack").GetValue(smi);
        var newState = goToStack.First();
        var args = config.ParameterName
            .ToDictionary(
                parameter => GetParameterIndex(smi, parameter),
                parameter => GetParameterValue(smi, parameter)
            );
        OnStateTransition?.Invoke(new ChoreTransitStateArgs(chore, newState?.name, args));
    }

    private static int GetParameterIndex(StateMachine.Instance smi, string parameterName) {
        var sm = smi.GetType().GetProperty("sm").GetValue(smi);
        var parameter = sm.GetType().GetField(parameterName).GetValue(sm);
        return (int) parameter.GetType().GetField("idx").GetValue(parameter);
    }

    private static object? GetParameterValue(StateMachine.Instance smi, string parameterName) {
        var sm = smi.GetType().GetProperty("sm").GetValue(smi);
        var parameter = sm.GetType().GetField(parameterName).GetValue(sm);
        var parameterIndex = (int) parameter.GetType().GetField("idx").GetValue(parameter);
        var parameterContext = smi.parameterContexts[parameterIndex];
        return parameterContext.GetType().GetField("value").GetValue(parameterContext);
    }
}
