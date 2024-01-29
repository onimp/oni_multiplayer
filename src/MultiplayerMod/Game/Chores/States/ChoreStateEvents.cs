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

    public static event Action<ChoreTransitStateArgs>? OnStateEnter;
    public static event Action<ChoreTransitStateArgs>? OnStateExit;
    public static event Action<ChoreTransitStateArgs>? OnStateTransition;

    // TODO filter duplicate events to reduce network load
    public static event Action<ChoreTransitStateArgs>? OnStateUpdate;

    // TODO filter duplicate events to reduce network load
    public static event Action<ChoreTransitStateArgs>? OnStateEventHandler;
    public static event Action<MoveToArgs>? OnStartMoveTo;

    // TODO filter duplicate events to reduce network load
    public static event Action<MoveToArgs>? OnUpdateMoveTo;

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
            BindMoveEnter(sm, config);
            BindMoveUpdate(sm, config);
            BindCallback(sm, config with { TransitionType = TransitionTypeEnum.Exit });
            return;
        }

        var state = config.GetMonitoredState(sm);

        var method = state.GetType().GetMethods().First(
            it => config.TransitionType switch {
                TransitionTypeEnum.Update => it.Name == config.TransitionType.ToString() &&
                                             it.GetParameters().Length == 4,
                // Transition being trigger upon state exit.
                TransitionTypeEnum.Transition => it.Name == "Exit" && it.GetParameters().Length == 2,
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
            TransitionTypeEnum.Transition => new object[] { "Trigger Multiplayer event", dlgt },
            TransitionTypeEnum.Update => new object[] { "Trigger Multiplayer event", dlgt, UpdateRate.SIM_200ms, true },
            TransitionTypeEnum.EventHandler => new object[] { config.EventGameHash!, dlgt },
            TransitionTypeEnum.MoveTo => throw new ArgumentOutOfRangeException(),
            _ => throw new ArgumentOutOfRangeException()
        };
        method.Invoke(state, args);
    }

    private static void BindMoveEnter(StateMachine sm, StateTransitionConfig config) {
        var state = config.GetMonitoredState(sm);

        var enterMethod = state.GetType().GetMethods()
            .First(it => it.Name == "Enter" && it.GetParameters().Length == 2);
        var enterDelegate = Delegate.CreateDelegate(
            enterMethod.GetParameters()[1].ParameterType,
            config,
            typeof(ChoreStateEvents).GetMethod(
                nameof(MoveEnterHandler),
                BindingFlags.NonPublic | BindingFlags.Static
            )!
        );
        enterMethod.Invoke(state, new object[] { "Trigger Multiplayer move event", enterDelegate });
    }

    private static void BindMoveUpdate(StateMachine sm, StateTransitionConfig config) {
        var state = config.GetMonitoredState(sm);
        if (!state.updateActions?.Any(it => it.buckets.Any(bucket => bucket.name.Equals("MoveTo()"))) ?? true) {
            return;
        }

        var updateMethod = state.GetType().GetMethods()
            .First(it => it.Name == "Update" && it.GetParameters().Length == 4);
        var updateDelegate = Delegate.CreateDelegate(
            updateMethod.GetParameters()[1].ParameterType,
            config,
            typeof(ChoreStateEvents).GetMethod(
                nameof(MoveUpdateHandler),
                BindingFlags.NonPublic | BindingFlags.Static
            )!
        );
        updateMethod.Invoke(
            state,
            new object[] { "Trigger Multiplayer move event", updateDelegate, UpdateRate.SIM_200ms, true }
        );
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
            TransitionTypeEnum.Transition => OnStateTransition,
            TransitionTypeEnum.MoveTo => throw new ArgumentOutOfRangeException(),
            TransitionTypeEnum.Update => OnStateUpdate,
            TransitionTypeEnum.EventHandler => OnStateEventHandler,
            _ => throw new ArgumentOutOfRangeException()
        };
        eventCallback?.Invoke(new ChoreTransitStateArgs(chore, newState?.name, args));
    }

    private static void MoveEnterHandler(StateTransitionConfig config, StateMachine.Instance smi) {
        var chore = (Chore) smi.GetMaster();
        var state = config.GetMonitoredState(smi.stateMachine);
        var target = state.GetType().GetMethod("GetStateTarget")!.Invoke(state, new object[] { });
        var navigator = (Navigator) target.GetType().GetMethod("Get")
            .MakeGenericMethod(typeof(Navigator))
            .Invoke(target, new object[] { smi });
        var cell = Grid.PosToCell(navigator.targetLocator);

        OnStartMoveTo?.Invoke(new MoveToArgs(chore, cell, navigator.targetOffsets));
    }

    private static void MoveUpdateHandler(StateTransitionConfig config, StateMachine.Instance smi, float dt) {
        var chore = (Chore) smi.GetMaster();
        var state = config.GetMonitoredState(smi.stateMachine);
        var target = state.GetType().GetMethod("GetStateTarget")!.Invoke(state, new object[] { });
        var navigator = (Navigator) target.GetType().GetMethod("Get")
            .MakeGenericMethod(typeof(Navigator))
            .Invoke(target, new object[] { smi });
        var cell = Grid.PosToCell(navigator.targetLocator);

        OnUpdateMoveTo?.Invoke(new MoveToArgs(chore, cell, navigator.targetOffsets));
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
