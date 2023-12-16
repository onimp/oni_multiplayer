using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using MultiplayerMod.ModRuntime.Context;
using MultiplayerMod.Multiplayer;
using MultiplayerMod.Multiplayer.CoreOperations;
using MultiplayerMod.Multiplayer.Objects;

namespace MultiplayerMod.Game.Chores.States;

[HarmonyPatch]
public static class ChoreStateEvents {
    public static event Action<ChoreTransitStateArgs>? OnStateTransition;

    [UsedImplicitly]
    private static IEnumerable<MethodBase> TargetMethods() {
        return ChoreList.Config.Values.Where(
                config => config.StateTransitionSync.Status == ChoreList.StateTransitionConfig.SyncStatus.On
            )
            .Select(config => config.StateTransitionSync.StateType.GetMethod("InitializeStates"));
    }

    [UsedImplicitly]
    [HarmonyPostfix]
    [RequireExecutionLevel(ExecutionLevel.Game)]
    [RequireMultiplayerMode(MultiplayerMode.Host)]
    public static void Postfix(object __instance) {
        var choreType = __instance.GetType().DeclaringType;
        var config = ChoreList.Config[choreType].StateTransitionSync;
        var stateToMonitor =
            (StateMachine.BaseState) __instance.GetType().GetField(config.StateToMonitorName).GetValue(__instance);
        BindExitCallback(stateToMonitor);
    }

    private static void BindExitCallback(StateMachine.BaseState state) {
        var method = state.GetType().GetMethods()
            .Single(method => method.Name == "Exit" && method.GetParameters().Length == 2);
        var dlgt = Delegate.CreateDelegate(
            method.GetParameters()[1].ParameterType,
            typeof(ChoreStateEvents).GetMethod(nameof(OnStateExit), BindingFlags.NonPublic | BindingFlags.Static)
        );
        method.Invoke(state, new object[] { "Trigger Multiplayer event", dlgt });
    }

    private static void OnStateExit(StateMachine.Instance smi) {
        var chore = (Chore) smi.GetMaster();
        var choreType = chore.GetType();
        var config = ChoreList.Config[choreType].StateTransitionSync;
        var goToStack = (Stack<StateMachine.BaseState>) smi.GetType().GetField("gotoStack").GetValue(smi);
        var state = goToStack.First();
        var args = config.ParameterName
            .ToDictionary(
                parameter => GetParameterIndex(smi, parameter),
                parameter => GetParameterValue(smi, parameter)
            );
        OnStateTransition?.Invoke(new ChoreTransitStateArgs(chore.MultiplayerId()!, state.name, args));
    }

    private static int GetParameterIndex(StateMachine.Instance smi, string parameterName) {
        var sm = smi.GetType().GetProperty("sm").GetValue(smi);
        var parameter = sm.GetType().GetField(parameterName).GetValue(sm);
        return (int) parameter.GetType().GetField("idx").GetValue(parameter);
    }

    private static object GetParameterValue(StateMachine.Instance smi, string parameterName) {
        var sm = smi.GetType().GetProperty("sm").GetValue(smi);
        var parameter = sm.GetType().GetField(parameterName).GetValue(sm);
        var parameterIndex = (int) parameter.GetType().GetField("idx").GetValue(parameter);
        var parameterContext = smi.parameterContexts[parameterIndex];
        return parameterContext.GetType().GetField("value").GetValue(parameterContext);
    }
}
