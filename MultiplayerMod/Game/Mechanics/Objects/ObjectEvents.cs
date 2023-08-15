using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using MultiplayerMod.Core.Patch;
using MultiplayerMod.Core.Patch.Context;
using MultiplayerMod.Multiplayer.Objects;
using UnityEngine;

namespace MultiplayerMod.Game.Mechanics.Objects;

[HarmonyPatch]
public static class ObjectEvents {

    public static event Action<ComponentEventsArgs>? ComponentMethodCalled;
    public static event Action<StateMachineEventsArgs>? StateMachineMethodCalled;

    private static readonly PatchTargetResolver targets = new PatchTargetResolver.Builder()
        .AddMethods(typeof(Filterable), nameof(Filterable.SelectedTag))
        .AddMethods(
            typeof(TreeFilterable),
            nameof(TreeFilterable.AddTagToFilter),
            nameof(TreeFilterable.RemoveTagFromFilter)
        )
        .AddMethods(typeof(Storage), nameof(Storage.SetOnlyFetchMarkedItems))
        .AddMethods(typeof(Door), nameof(Door.QueueStateChange), nameof(Door.OrderUnseal))
        .AddMethods(
            typeof(ComplexFabricator),
            nameof(ComplexFabricator.IncrementRecipeQueueCount),
            nameof(ComplexFabricator.DecrementRecipeQueueCount),
            nameof(ComplexFabricator.SetRecipeQueueCount)
        )
        .AddMethods(typeof(PassengerRocketModule), nameof(PassengerRocketModule.RequestCrewBoard))
        .AddMethods(typeof(RocketControlStation), nameof(RocketControlStation.RestrictWhenGrounded))
        .AddMethods(typeof(ICheckboxControl), nameof(ICheckboxControl.SetCheckboxValue))
        .AddMethods(typeof(SuitLocker), nameof(SuitLocker.ConfigNoSuit), nameof(SuitLocker.ConfigRequestSuit))
        .AddMethods(
            typeof(IThresholdSwitch),
            nameof(IThresholdSwitch.Threshold),
            nameof(IThresholdSwitch.ActivateAboveThreshold)
        )
        .AddMethods(typeof(ISliderControl), nameof(ISingleSliderControl.SetSliderValue))
        .AddMethods(typeof(Valve), nameof(Valve.ChangeFlow))
        .AddMethods(
            typeof(SingleEntityReceptacle),
            nameof(SingleEntityReceptacle.OrderRemoveOccupant),
            nameof(SingleEntityReceptacle.CancelActiveRequest),
            nameof(SingleEntityReceptacle.CreateOrder),
            nameof(SingleEntityReceptacle.SetPreview)
        )
        .AddMethods(typeof(LimitValve), nameof(LimitValve.Limit), nameof(LimitValve.ResetAmount))
        .AddMethods(
            typeof(ILogicRibbonBitSelector),
            nameof(ILogicRibbonBitSelector.SetBitSelection),
            nameof(ILogicRibbonBitSelector.UpdateVisuals)
        )
        .AddMethods(typeof(CreatureLure), nameof(CreatureLure.ChangeBaitSetting))
        .AddMethods(typeof(MonumentPart), nameof(MonumentPart.SetState))
        .AddMethods(typeof(INToggleSideScreenControl), nameof(INToggleSideScreenControl.QueueSelectedOption))
        .AddMethods(typeof(Artable), nameof(Artable.SetUserChosenTargetState), nameof(Artable.SetDefault))
        .AddMethods(typeof(Automatable), nameof(Automatable.SetAutomationOnly))
        .AddMethods(
            typeof(IDispenser),
            nameof(IDispenser.OnCancelDispense),
            nameof(IDispenser.OnOrderDispense),
            nameof(IDispenser.SelectItem)
        )
        .AddMethods(typeof(FlatTagFilterable), nameof(FlatTagFilterable.ToggleTag))
        .AddMethods(typeof(GeneShuffler), nameof(GeneShuffler.RequestRecharge))
        .AddMethods(
            typeof(GeneticAnalysisStation.StatesInstance),
            nameof(GeneticAnalysisStation.StatesInstance.SetSeedForbidden)
        )
        .AddMethods(typeof(IHighEnergyParticleDirection), nameof(IHighEnergyParticleDirection.Direction))
        .AddMethods(
            typeof(CraftModuleInterface),
            nameof(CraftModuleInterface.CancelLaunch),
            nameof(CraftModuleInterface.TriggerLaunch)
        )
        .AddMethods(
            typeof(IActivationRangeTarget),
            nameof(IActivationRangeTarget.ActivateValue),
            nameof(IActivationRangeTarget.DeactivateValue)
        )
        .AddMethods(typeof(ISidescreenButtonControl), nameof(ISidescreenButtonControl.OnSidescreenButtonPressed))
        .AddMethods(typeof(IUserControlledCapacity), nameof(IUserControlledCapacity.UserMaxCapacity))
        .AddMethods(typeof(Assignable), nameof(Assignable.Assign), nameof(Assignable.Unassign))
        .AddMethods(
            typeof(AccessControl),
            nameof(AccessControl.SetPermission),
            nameof(AccessControl.ClearPermission),
            nameof(AccessControl.DefaultPermission)
        )
        .AddMethods(typeof(LogicBroadcastReceiver), nameof(LogicBroadcastReceiver.SetChannel))
        .AddMethods(typeof(LaunchConditionManager), nameof(LaunchConditionManager.Launch))
        .AddMethods(typeof(GeoTuner.Instance), nameof(GeoTuner.Instance.AssignFutureGeyser))
        .AddMethods(typeof(IConfigurableConsumer), nameof(IConfigurableConsumer.SetSelectedOption))
        .AddMethods(typeof(LogicTimerSensor), nameof(LogicTimerSensor.ResetTimer))
        .AddMethods(
            typeof(IEmptyableCargo),
            nameof(IEmptyableCargo.AutoDeploy),
            nameof(IEmptyableCargo.EmptyCargo),
            nameof(IEmptyableCargo.ChosenDuplicant)
        )
        .AddMethods(
            typeof(IPlayerControlledToggle),
            nameof(IPlayerControlledToggle.ToggleRequested),
            nameof(IPlayerControlledToggle.ToggledByPlayer)
        )
        // TODO decide how to proper patch KMonoBehaviour#Trigger
        // .AddMethods(
        //     typeof(ReorderableBuilding),
        //     nameof(ReorderableBuilding.SwapWithAbove),
        //     nameof(ReorderableBuilding.SwapWithBelow),
        //     nameof(ReorderableBuilding.Trigger)
        // )
        .AddBaseType(typeof(KMonoBehaviour))
        .AddBaseType(typeof(StateMachine.Instance))
        .CheckArgumentsSerializable(true)
        .Build();

    // ReSharper disable once UnusedMember.Local
    private static IEnumerable<MethodBase> TargetMethods() => targets.Resolve();

    [HarmonyPrefix]
    // ReSharper disable once UnusedMember.Local
    private static void ObjectEventsPrefix() => PatchContext.Enter(PatchControl.DisablePatches);

    [HarmonyPostfix]
    // ReSharper disable once UnusedMember.Local
    private static void ObjectEventsPostfix(object __instance, MethodBase __originalMethod, object[] __args) {
        PatchContext.Leave();
        PatchControl.RunIfEnabled(
            () => {
                var args = __args.Select(
                    obj =>
                        obj switch {
                            GameObject gameObject => gameObject.GetGridReference(),
                            KMonoBehaviour kMonoBehaviour => kMonoBehaviour.GetReference(),
                            _ => obj
                        }
                ).ToArray();
                switch (__instance) {
                    case KMonoBehaviour kMonoBehaviour:
                        ComponentMethodCalled?.Invoke(
                            new ComponentEventsArgs(
                                kMonoBehaviour.GetReference(),
                                __originalMethod.DeclaringType!,
                                __originalMethod.Name,
                                args
                            )
                        );
                        return;
                    case StateMachine.Instance stateMachine:
                        StateMachineMethodCalled?.Invoke(
                            new StateMachineEventsArgs(
                                stateMachine.GetReference(),
                                __originalMethod.DeclaringType!,
                                __originalMethod.Name,
                                args
                            )
                        );
                        return;
                    default:
                        throw new NotSupportedException($"{__instance} has un supported type");
                }
            }
        );
    }

}
