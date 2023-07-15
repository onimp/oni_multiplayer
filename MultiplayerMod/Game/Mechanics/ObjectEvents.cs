using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.Core.Patch;
using MultiplayerMod.Multiplayer.Objects;
using MultiplayerMod.Multiplayer.Objects.Reference;

namespace MultiplayerMod.Game.Mechanics;

public static class ObjectEvents {
    public static event Action<ObjectEventsArgs> MethodCalled;

    private static readonly Core.Logging.Logger log = LoggerFactory.GetLogger(typeof(ObjectEvents));

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
            typeof(ReorderableBuilding),
            new[] {
                nameof(ReorderableBuilding.SwapWithAbove),
                nameof(ReorderableBuilding.SwapWithBelow),
                nameof(ReorderableBuilding.Trigger)
            }
        }, {
            typeof(SingleEntityReceptacle),
            new[] {
                nameof(SingleEntityReceptacle.OrderRemoveOccupant),
                nameof(SingleEntityReceptacle.CancelActiveRequest),
                nameof(SingleEntityReceptacle.CreateOrder),
                nameof(SingleEntityReceptacle.SetPreview)
            }
        },
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
        // TODO uncomment lines below after creating required surrogates.
        // { typeof(Assignable), new[] { nameof(Assignable.Assign), nameof(Assignable.Unassign) } },
        // { typeof(AccessControl),
        //     new[] {
        //         nameof(AccessControl.SetPermission), nameof(AccessControl.ClearPermission),
        //         nameof(AccessControl.DefaultPermission)
        //     }
        // }
    };

    private static IEnumerable<MethodBase> GetTargetMethods(int argsCount) {
        var classTypes = methodsForPatch.Keys.Where(type => type.IsClass).ToList();
        var interfaceTypes = methodsForPatch.Keys.Where(type => type.IsInterface).ToList();
        var targetMethods = Assembly.GetAssembly(typeof(ISliderControl))
            .GetTypes()
            .Where(
                type => type.IsClass && (classTypes.Contains(type)
                                         || interfaceTypes.Any(interfaceType => interfaceType.IsAssignableFrom(type)))
            )
            .SelectMany(
                type => {
                    string[] methodNames;
                    Type interfaceType = null;
                    if (classTypes.Contains(type)) {
                        methodNames = methodsForPatch[type];
                    } else {
                        interfaceType =
                            interfaceTypes.Single(interfaceType => interfaceType.IsAssignableFrom(type));
                        methodNames = methodsForPatch[interfaceType];
                    }
                    return methodNames.Select(
                        methodName => {
                            var methodInfo = type.GetMethod(
                                methodName,
                                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance
                            );
                            if (methodInfo != null) {
                                return methodInfo;
                            }
                            if (interfaceType != null) {
                                // Some overrides names prefixed by interface e.g. Clinic#ISliderControl.SetSliderValue
                                methodInfo = type.GetMethod(
                                    interfaceType.Name + "." + methodName,
                                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance
                                );
                                if (methodInfo != null) return methodInfo;
                            }

                            var property = type.GetProperty(
                                methodName,
                                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance
                            );
                            if (property == null) {
                                log.Error($"Method {type}.{methodName} not found");
                            }
                            return property?.GetSetMethod();
                        }
                    );
                }
            )
            .Where(method => method.GetParameters().Length == argsCount)
            .ToList();
        return targetMethods;
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

    [Serializable]
    public class ObjectEventsArgs {
        public GameObjectReference Target { get; }

        public Type MethodType { get; }
        public string MethodName { get; }
        public object[] Args { get; }

        public ObjectEventsArgs(GameObjectReference target, Type methodType, string methodName, object[] args) {
            Target = target;
            MethodType = methodType;
            MethodName = methodName;
            Args = args;
        }
    }

}
