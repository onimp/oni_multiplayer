using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using HarmonyLib;
using MultiplayerMod.Core.Patch;
using MultiplayerMod.Multiplayer.Objects;

namespace MultiplayerMod.Game.Mechanics.Objects;

public static class ObjectEvents {
    public static event Action<ObjectEventsArgs>? MethodCalled;

    private static readonly Dictionary<Type, string[]> methodsForPatch = new() {
        { typeof(Filterable), new[] { nameof(Filterable.SelectedTag) } }, {
            typeof(TreeFilterable),
            new[] { nameof(TreeFilterable.AddTagToFilter), nameof(TreeFilterable.RemoveTagFromFilter), }
        },
        { typeof(Door), new[] { nameof(Door.QueueStateChange), nameof(Door.OrderUnseal) } }, {
            typeof(ComplexFabricator),
            new[] {
                nameof(ComplexFabricator.IncrementRecipeQueueCount),
                nameof(ComplexFabricator.DecrementRecipeQueueCount),
                nameof(ComplexFabricator.SetRecipeQueueCount)
            }
        },
        { typeof(PassengerRocketModule), new[] { nameof(PassengerRocketModule.RequestCrewBoard) } },
        { typeof(RocketControlStation), new[] { nameof(RocketControlStation.RestrictWhenGrounded) } },
        { typeof(ICheckboxControl), new[] { nameof(ICheckboxControl.SetCheckboxValue) } },
        { typeof(SuitLocker), new[] { nameof(SuitLocker.ConfigNoSuit), nameof(SuitLocker.ConfigRequestSuit) } }, {
            typeof(IThresholdSwitch),
            new[] { nameof(IThresholdSwitch.Threshold), nameof(IThresholdSwitch.ActivateAboveThreshold) }
        },
        { typeof(ISliderControl), new[] { nameof(ISingleSliderControl.SetSliderValue) } },
        { typeof(Valve), new[] { nameof(Valve.ChangeFlow) } }, {
            typeof(SingleEntityReceptacle),
            new[] {
                nameof(SingleEntityReceptacle.OrderRemoveOccupant),
                nameof(SingleEntityReceptacle.CancelActiveRequest),
                nameof(SingleEntityReceptacle.CreateOrder),
                nameof(SingleEntityReceptacle.SetPreview)
            }
        },
        { typeof(LimitValve), new[] { nameof(LimitValve.Limit), nameof(LimitValve.ResetAmount) } }, {
            typeof(ILogicRibbonBitSelector),
            new[] { nameof(ILogicRibbonBitSelector.SetBitSelection), nameof(ILogicRibbonBitSelector.UpdateVisuals) }
        },
        { typeof(CreatureLure), new[] { nameof(CreatureLure.ChangeBaitSetting) } },
        { typeof(MonumentPart), new[] { nameof(MonumentPart.SetState) } },
        { typeof(INToggleSideScreenControl), new[] { nameof(INToggleSideScreenControl.QueueSelectedOption) } },
        { typeof(Artable), new[] { nameof(Artable.SetUserChosenTargetState), nameof(Artable.SetDefault) } },
        { typeof(Automatable), new[] { nameof(Automatable.SetAutomationOnly) } }, {
            typeof(IDispenser),
            new[] {
                nameof(IDispenser.OnCancelDispense), nameof(IDispenser.OnOrderDispense), nameof(IDispenser.SelectItem)
            }
        },
        { typeof(FlatTagFilterable), new[] { nameof(FlatTagFilterable.ToggleTag) } },
        { typeof(GeneShuffler), new[] { nameof(GeneShuffler.SetWorkTime), nameof(GeneShuffler.RequestRecharge) } }, {
            typeof(GeneticAnalysisStation.StatesInstance),
            new[] { nameof(GeneticAnalysisStation.StatesInstance.SetSeedForbidden) }
        },
        { typeof(IHighEnergyParticleDirection), new[] { nameof(IHighEnergyParticleDirection.Direction) } }, {
            typeof(CraftModuleInterface),
            new[] { nameof(CraftModuleInterface.CancelLaunch), nameof(CraftModuleInterface.TriggerLaunch) }
        }, {
            typeof(IActivationRangeTarget),
            new[] { nameof(IActivationRangeTarget.ActivateValue), nameof(IActivationRangeTarget.DeactivateValue) }
        },
        { typeof(ISidescreenButtonControl), new[] { nameof(ISidescreenButtonControl.OnSidescreenButtonPressed) } },
        { typeof(IUserControlledCapacity), new[] { nameof(IUserControlledCapacity.UserMaxCapacity) } },
        // TODO decide how to proper patch KMonoBehaviour#Trigger
        // {
        //     typeof(ReorderableBuilding),
        //     new[] {
        //         nameof(ReorderableBuilding.SwapWithAbove),
        //         nameof(ReorderableBuilding.SwapWithBelow),
        //         nameof(ReorderableBuilding.Trigger)
        //     }
        // },
        // TODO uncomment lines below after deciding on field patches.
        //  { typeof(RailGun), new[] { nameof(RailGun.launchMass) } },
        // {
        //     typeof(TemperatureControlledSwitch),
        //     new[] {
        //         nameof(TemperatureControlledSwitch.thresholdTemperature),
        //         nameof(TemperatureControlledSwitch.activateOnWarmerThan)
        //     }
        // },
        // {
        //     typeof(LogicTimeOfDaySensor),
        //     new[] { nameof(LogicTimeOfDaySensor.startTime), nameof(LogicTimeOfDaySensor.duration) }
        // },
        // {
        //     typeof(LogicTimerSensor),
        //     new[] {
        //         nameof(LogicTimerSensor.displayCyclesMode),
        //         nameof(LogicTimerSensor.onDuration),
        //         nameof(LogicTimerSensor.offDuration),
        //         nameof(LogicTimerSensor.timeElapsedInCurrentState),
        //         nameof(LogicTimerSensor.ResetTimer)
        //     }
        // },
        // {
        //          typeof(LogicAlarm),
        //          new[] {
        //              nameof(LogicAlarm.notificationName),
        //              nameof(LogicAlarm.notificationTooltip),
        //              nameof(LogicAlarm.pauseOnNotify),
        //              nameof(LogicAlarm.zoomOnNotify),
        //              nameof(LogicAlarm.notificationType),
        //              nameof(LogicAlarm.UpdateNotification)
        //          }
        //      },
        //      {
        //          typeof(LogicCounter),
        //          new[] {
        //              nameof(LogicCounter.currentCount),
        //              nameof(LogicCounter.maxCount),
        //              nameof(LogicCounter.SetCounterState),
        //              nameof(LogicCounter.UpdateLogicCircuit),
        //              nameof(LogicCounter.UpdateVisualState),
        //              nameof(LogicCounter.UpdateMeter)
        //          }
        //      },
        //  { typeof(LogicCritterCountSensor), new[] { nameof(LogicCritterCountSensor.countCritters) } },
        // TODO uncomment lines below after creating required surrogates.
        // { typeof(Assignable), new[] { nameof(Assignable.Assign), nameof(Assignable.Unassign) } },
        // { typeof(AccessControl),
        //     new[] {
        //         nameof(AccessControl.SetPermission), nameof(AccessControl.ClearPermission),
        //         nameof(AccessControl.DefaultPermission)
        //     }
        // }
        // {typeof(LogicBroadcastReceiver), new []{nameof(LogicBroadcastReceiver.SetChannel)}},
        // { typeof(LaunchConditionManager), new[] { nameof(LaunchConditionManager.Launch) } },
        // { typeof(GeoTuner.Instance), new[] { nameof(GeoTuner.Instance.AssignFutureGeyser) } },
        //  { typeof(IConfigurableConsumer), new[] { nameof(IConfigurableConsumer.SetSelectedOption) } },
    };

    private static IEnumerable<MethodBase> GetTargetMethods(int argsCount) {
        return TargetExtractor.GetTargetMethods(methodsForPatch, argsCount);
    }

    [HarmonyPatch]
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
// ReSharper disable once UnusedType.Local
    private class PatchFor0ArgumentMethods {
        private static IEnumerable<MethodBase> TargetMethods() {
            return GetTargetMethods(0);
        }

        [HarmonyPostfix]
        private static void ObjectEventsPostfix(object __instance, MethodBase __originalMethod) =>
            PatchControl.RunIfEnabled(
                () => {
                    MethodCalled?.Invoke(
                        new ObjectEventsArgs(
                            ((KMonoBehaviour) __instance).GetGridReference(),
                            __originalMethod.DeclaringType,
                            __originalMethod.Name,
                            new object[] { }
                        )
                    );
                }
            );
    }

    [HarmonyPatch]
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
// ReSharper disable once UnusedType.Local
    private class PatchFor1ArgumentMethods {
        private static IEnumerable<MethodBase> TargetMethods() {
            return GetTargetMethods(1);
        }

        [HarmonyPostfix]
        private static void ObjectEventsPostfix(object __instance, MethodBase __originalMethod, object __0) =>
            PatchControl.RunIfEnabled(
                () => {
                    MethodCalled?.Invoke(
                        new ObjectEventsArgs(
                            ((KMonoBehaviour) __instance).GetGridReference(),
                            __originalMethod.DeclaringType,
                            __originalMethod.Name,
                            new[] { __0 }
                        )
                    );
                }
            );
    }

    [HarmonyPatch]
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
// ReSharper disable once UnusedType.Local
    private class PatchFor2ArgumentMethods {
        private static IEnumerable<MethodBase> TargetMethods() {
            return GetTargetMethods(2);
        }

        [HarmonyPostfix]
        private static void ObjectEventsPostfix(
            object __instance,
            MethodBase __originalMethod,
            object __0,
            object __1
        ) =>
            PatchControl.RunIfEnabled(
                () => {
                    MethodCalled?.Invoke(
                        new ObjectEventsArgs(
                            ((KMonoBehaviour) __instance).GetGridReference(),
                            __originalMethod.DeclaringType,
                            __originalMethod.Name,
                            new[] { __0, __1 }
                        )
                    );
                }
            );
    }
}
