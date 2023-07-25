using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using MultiplayerMod.Core.Patch;
using MultiplayerMod.Multiplayer.Objects;
using UnityEngine;

namespace MultiplayerMod.Game.Mechanics.Objects;

[HarmonyPatch]
public static class ObjectEvents {

    public static event Action<ObjectEventsArgs>? MethodCalled;

    private static readonly Dictionary<Type, string[]> methodsForPatch = new() {
        {
            typeof(Filterable),
            new[] { nameof(Filterable.SelectedTag) }
        }, {
            typeof(TreeFilterable),
            new[] { nameof(TreeFilterable.AddTagToFilter), nameof(TreeFilterable.RemoveTagFromFilter) }
        },
        { typeof(Storage), new[] { nameof(Storage.SetOnlyFetchMarkedItems) } }, {
            typeof(Door),
            new[] { nameof(Door.QueueStateChange), nameof(Door.OrderUnseal) }
        }, {
            typeof(ComplexFabricator),
            new[] {
                nameof(ComplexFabricator.IncrementRecipeQueueCount),
                nameof(ComplexFabricator.DecrementRecipeQueueCount),
                nameof(ComplexFabricator.SetRecipeQueueCount)
            }
        }, {
            typeof(PassengerRocketModule),
            new[] { nameof(PassengerRocketModule.RequestCrewBoard) }
        }, {
            typeof(RocketControlStation),
            new[] { nameof(RocketControlStation.RestrictWhenGrounded) }
        }, {
            typeof(ICheckboxControl),
            new[] { nameof(ICheckboxControl.SetCheckboxValue) }
        }, {
            typeof(SuitLocker),
            new[] { nameof(SuitLocker.ConfigNoSuit), nameof(SuitLocker.ConfigRequestSuit) }
        }, {
            typeof(IThresholdSwitch),
            new[] { nameof(IThresholdSwitch.Threshold), nameof(IThresholdSwitch.ActivateAboveThreshold) }
        }, {
            typeof(ISliderControl),
            new[] { nameof(ISingleSliderControl.SetSliderValue) }
        }, {
            typeof(Valve),
            new[] { nameof(Valve.ChangeFlow) }
        }, {
            typeof(SingleEntityReceptacle),
            new[] {
                nameof(SingleEntityReceptacle.OrderRemoveOccupant),
                nameof(SingleEntityReceptacle.CancelActiveRequest),
                nameof(SingleEntityReceptacle.CreateOrder),
                nameof(SingleEntityReceptacle.SetPreview)
            }
        }, {
            typeof(LimitValve),
            new[] { nameof(LimitValve.Limit), nameof(LimitValve.ResetAmount) }
        }, {
            typeof(ILogicRibbonBitSelector),
            new[] { nameof(ILogicRibbonBitSelector.SetBitSelection), nameof(ILogicRibbonBitSelector.UpdateVisuals) }
        }, {
            typeof(CreatureLure),
            new[] { nameof(CreatureLure.ChangeBaitSetting) }
        }, {
            typeof(MonumentPart),
            new[] { nameof(MonumentPart.SetState) }
        }, {
            typeof(INToggleSideScreenControl),
            new[] { nameof(INToggleSideScreenControl.QueueSelectedOption) }
        }, {
            typeof(Artable),
            new[] { nameof(Artable.SetUserChosenTargetState), nameof(Artable.SetDefault) }
        }, {
            typeof(Automatable),
            new[] { nameof(Automatable.SetAutomationOnly) }
        }, {
            typeof(IDispenser),
            new[] {
                nameof(IDispenser.OnCancelDispense),
                nameof(IDispenser.OnOrderDispense),
                nameof(IDispenser.SelectItem)
            }
        }, {
            typeof(FlatTagFilterable),
            new[] { nameof(FlatTagFilterable.ToggleTag) }
        }, {
            typeof(GeneShuffler),
            new[] { nameof(GeneShuffler.SetWorkTime), nameof(GeneShuffler.RequestRecharge) }
        }, {
            typeof(GeneticAnalysisStation.StatesInstance),
            new[] { nameof(GeneticAnalysisStation.StatesInstance.SetSeedForbidden) }
        }, {
            typeof(IHighEnergyParticleDirection),
            new[] { nameof(IHighEnergyParticleDirection.Direction) }
        }, {
            typeof(CraftModuleInterface),
            new[] { nameof(CraftModuleInterface.CancelLaunch), nameof(CraftModuleInterface.TriggerLaunch) }
        }, {
            typeof(IActivationRangeTarget),
            new[] { nameof(IActivationRangeTarget.ActivateValue), nameof(IActivationRangeTarget.DeactivateValue) }
        }, {
            typeof(ISidescreenButtonControl),
            new[] { nameof(ISidescreenButtonControl.OnSidescreenButtonPressed) }
        }, {
            typeof(IUserControlledCapacity),
            new[] { nameof(IUserControlledCapacity.UserMaxCapacity) }
        },
        { typeof(Assignable), new[] { nameof(Assignable.Assign), nameof(Assignable.Unassign) } }, {
            typeof(AccessControl),
            new[] {
                nameof(AccessControl.SetPermission),
                nameof(AccessControl.ClearPermission),
                nameof(AccessControl.DefaultPermission)
            }
        },
        { typeof(LogicBroadcastReceiver), new[] { nameof(LogicBroadcastReceiver.SetChannel) } },
        { typeof(LaunchConditionManager), new[] { nameof(LaunchConditionManager.Launch) } },
        { typeof(GeoTuner.Instance), new[] { nameof(GeoTuner.Instance.AssignFutureGeyser) } },
        { typeof(IConfigurableConsumer), new[] { nameof(IConfigurableConsumer.SetSelectedOption) } },
        { typeof(LogicTimerSensor), new[] { nameof(LogicTimerSensor.ResetTimer) } }, {
            typeof(IEmptyableCargo),
            new[] {
                nameof(IEmptyableCargo.AutoDeploy),
                nameof(IEmptyableCargo.EmptyCargo),
                nameof(IEmptyableCargo.ChosenDuplicant)
            }
        }, {
            typeof(IPlayerControlledToggle),
            new[] { nameof(IPlayerControlledToggle.ToggleRequested), nameof(IPlayerControlledToggle.ToggledByPlayer) }
        }
        // TODO decide how to proper patch KMonoBehaviour#Trigger
        // {
        //     typeof(ReorderableBuilding),
        //     new[] {
        //         nameof(ReorderableBuilding.SwapWithAbove),
        //         nameof(ReorderableBuilding.SwapWithBelow),
        //         nameof(ReorderableBuilding.Trigger)
        //     }
        // }
    };

    // ReSharper disable once UnusedMember.Local
    private static IEnumerable<MethodBase> TargetMethods() => TargetExtractor.GetTargetMethods(methodsForPatch);

    [HarmonyPostfix]
    // ReSharper disable once UnusedMember.Local
    private static void ObjectEventsPostfix(KMonoBehaviour __instance, MethodBase __originalMethod, object[] __args) =>
        PatchControl.RunIfEnabled(
            () => {
                MethodCalled?.Invoke(
                    new ObjectEventsArgs(
                        __instance.GetReference(),
                        __originalMethod.DeclaringType!,
                        __originalMethod.Name,
                        __args.Select(
                            obj =>
                                obj switch {
                                    GameObject gameObject => gameObject.GetGridReference(),
                                    KMonoBehaviour kMonoBehaviour => kMonoBehaviour.GetReference(),
                                    _ => obj
                                }
                        ).ToArray()
                    )
                );
            }
        );

}
