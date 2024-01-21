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

namespace MultiplayerMod.Game.Chores.States;

[HarmonyPatch]
public static class ChoreStateEvents {

    // TODO filter duplicate events to reduce network load
    public static event Action<ChoreTransitStateArgs>? OnStateEnter;

    // TODO filter duplicate events to reduce network load
    public static event Action<ChoreTransitStateArgs>? OnStateExit;

    // TODO filter duplicate events to reduce network load
    public static event Action<ChoreTransitStateArgs>? OnStateUpdate;

    // TODO filter duplicate events to reduce network load
    public static event Action<ChoreTransitStateArgs>? OnStateEventHandler;

    [UsedImplicitly]
    private static IEnumerable<MethodBase> TargetMethods() {
        var choreSyncConfigs = ChoreList.Config.Values
            .Where(config => config.StatesTransitionSync.Status == StatesTransitionStatus.On).ToArray();
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
            BindCallback(__instance, stateTransitionConfig);
        }
    }

    private static void BindCallback(StateMachine sm, StateTransitionConfig config) {
        if (config.TransitionType == TransitionTypeEnum.MoveTo) {
            // TODO handle moveTo transition types.
            return;
        }
        var state = config.GetMonitoredState(sm);

        var method = state.GetType().GetMethods().First(
            it => config.TransitionType switch {
                TransitionTypeEnum.Update => it.Name == config.TransitionType.ToString() &&
                                             it.GetParameters().Length == 4,
                _ => it.Name == config.TransitionType.ToString() && it.GetParameters().Length == 2
            }
        );
        var dlgt = Delegate.CreateDelegate(
            method.GetParameters()[1].ParameterType,
            config,
            typeof(ChoreStateEvents).GetMethod(
                config.TransitionType != TransitionTypeEnum.Update
                    ? nameof(EventHandlerCallback)
                    : nameof(UpdateHandlerCallback),
                BindingFlags.NonPublic | BindingFlags.Static
            )!
        );
        var args = config.TransitionType switch {
            TransitionTypeEnum.Enter => new object[] { "Trigger Multiplayer event", dlgt },
            TransitionTypeEnum.Exit => new object[] { "Trigger Multiplayer event", dlgt },
            TransitionTypeEnum.Update => new object[] { "Trigger Multiplayer event", dlgt, UpdateRate.SIM_200ms, true },
            TransitionTypeEnum.EventHandler => new object[] { config.EventGameHash!, dlgt },
            TransitionTypeEnum.MoveTo => throw new ArgumentOutOfRangeException(),
            _ => throw new ArgumentOutOfRangeException()
        };
        method.Invoke(state, args);
    }

    private static void UpdateHandlerCallback(StateTransitionConfig config, StateMachine.Instance smi, float dt) {
        EventHandlerCallback(config, smi);
    }

    private static void EventHandlerCallback(StateTransitionConfig config, StateMachine.Instance smi) {
        var chore = (Chore) smi.GetMaster();
        var goToStack = (Stack<StateMachine.BaseState>) smi.GetType().GetField("gotoStack").GetValue(smi);
        var newState = goToStack.FirstOrDefault();
        var args = config.ParameterName
            .ToDictionary(
                parameter => GetParameterIndex(smi, parameter),
                parameter => GetParameterValue(smi, parameter)
            );
        var eventCallback = config.TransitionType switch {
            TransitionTypeEnum.Enter => OnStateEnter,
            TransitionTypeEnum.Exit => OnStateExit,
            TransitionTypeEnum.MoveTo => throw new ArgumentOutOfRangeException(),
            TransitionTypeEnum.Update => OnStateUpdate,
            TransitionTypeEnum.EventHandler => OnStateEventHandler,
            _ => throw new ArgumentOutOfRangeException()
        };
        eventCallback?.Invoke(new ChoreTransitStateArgs(chore, newState?.name, args));
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
