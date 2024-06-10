using System;
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

namespace MultiplayerMod.Multiplayer.Chores.Patches.States;

// [HarmonyPatch]
public static class AdjustStateMachineTransitions {

    [UsedImplicitly]
    private static IEnumerable<MethodBase> TargetMethods() {
        return ChoreList.Config.Values.Where(config => config.StatesTransitionSync.Status == StatesTransitionStatus.On)
            .Select(config => config.StatesTransitionSync.StateType.GetMethod(nameof(StateMachine.InitializeStates)));
    }

    [UsedImplicitly]
    [HarmonyPostfix]
    [RequireMultiplayerMode(MultiplayerMode.Client)]
    [RequireExecutionLevel(ExecutionLevel.Multiplayer)]
    public static void InitializeStatesPatch(StateMachine __instance) {
        var config = ChoreList.Config[__instance.GetType().DeclaringType].StatesTransitionSync;

        foreach (var stateTransitionConfig in config.StateTransitionConfigs) {
            var statesManager = Runtime.Instance.Dependencies.Get<StatesManager>();
            var stateToBeSynced = stateTransitionConfig.GetMonitoredState(__instance);
            switch (stateTransitionConfig.TransitionType) {
                case TransitionTypeEnum.Exit: {
                    stateToBeSynced.enterActions.Clear();
                    statesManager.AddAndTransitToWaiStateUponEnter(stateToBeSynced);
                    break;
                }
                case TransitionTypeEnum.Enter: {
                    stateToBeSynced.enterActions.Clear();
                    statesManager.AddAndTransitToWaiStateUponEnter(stateToBeSynced);
                    statesManager.AddContinuationState(stateToBeSynced);
                    break;
                }
                case TransitionTypeEnum.Transition: {
                    stateToBeSynced.enterActions.RemoveAll(action => action.name.Contains("Transition"));
                    stateToBeSynced.updateActions.RemoveAll(
                        action => action.buckets.Any(bucket => bucket.name.Contains("Transition"))
                    );
                    break;
                }
                case TransitionTypeEnum.MoveTo:
                    stateToBeSynced.transitions.RemoveAll(
                        it => it.name.Equals(GameHashes.DestinationReached.ToString())
                    );
                    stateToBeSynced.transitions.RemoveAll(
                        it => it.name.Equals(GameHashes.NavigationFailed.ToString())
                    );
                    stateToBeSynced.enterActions.RemoveAll(action => action.name.Equals("MoveTo()"));
                    stateToBeSynced.updateActions?.RemoveAll(
                        action => action.buckets.Any(bucket => bucket.name.Equals("MoveTo()"))
                    );
                    break;
                case TransitionTypeEnum.Update:
                    stateToBeSynced.updateActions.Clear();
                    break;
                case TransitionTypeEnum.EventHandler: {
                    stateToBeSynced.events.Clear();
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
