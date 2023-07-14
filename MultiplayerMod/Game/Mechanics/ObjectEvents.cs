using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using MultiplayerMod.Core.Patch;
using MultiplayerMod.Multiplayer.Objects;

namespace MultiplayerMod.Game.Mechanics;

public static class ObjectEvents {

    public static event Action<ObjectEventsArgs> MethodCalled;

    private static readonly Dictionary<Type, string[]> MethodsForPatch = new() {
        { typeof(Filterable), new[] { nameof(Filterable.SelectedTag) } },
        { typeof(TreeFilterable),
            new[] { nameof(TreeFilterable.AddTagToFilter), nameof(TreeFilterable.RemoveTagFromFilter), }
        },
        { typeof(Clinic), new[] { "ISliderControl.SetSliderValue" } },
        { typeof(DevRadiationEmitter), new[] { nameof(ISingleSliderControl.SetSliderValue) } },
        { typeof(EnergyGenerator), new[] { nameof(ISingleSliderControl.SetSliderValue) } },
        { typeof(HEPBattery.Instance), new[] { nameof(ISingleSliderControl.SetSliderValue) } },
        { typeof(HighEnergyParticleSpawner), new[] { nameof(ISingleSliderControl.SetSliderValue) } },
        { typeof(LogicGateBuffer), new[] { nameof(ISingleSliderControl.SetSliderValue) } },
        { typeof(LogicGateFilter), new[] { nameof(ISingleSliderControl.SetSliderValue) } },
        { typeof(ManualGenerator), new[] { nameof(ISingleSliderControl.SetSliderValue) } },
        { typeof(OilWellCap), new[] { nameof(ISingleSliderControl.SetSliderValue) } },
        { typeof(Door), new[] { nameof(Door.QueueStateChange), nameof(Door.OrderUnseal) } },
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
        var targetMethods = Assembly.GetAssembly(typeof(ISliderControl))
            .GetTypes()
            .Where(type => MethodsForPatch.Keys.Contains(type))
            .SelectMany(
                type =>
                    MethodsForPatch[type].Select(
                        methodName => {
                            var methodInfo = type.GetMethod(
                                methodName,
                                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance |
                                BindingFlags.DeclaredOnly
                            );
                            if (methodInfo != null) return methodInfo;

                            var property = type.GetProperty(
                                methodName,
                                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance |
                                BindingFlags.DeclaredOnly
                            );
                            return property?.GetSetMethod();
                        }
                    )
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
                            ((KMonoBehaviour) __instance).GetMultiplayerReference(),
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
                            ((KMonoBehaviour) __instance).GetMultiplayerReference(),
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
                            ((KMonoBehaviour) __instance).GetMultiplayerReference(),
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
        public MultiplayerReference Target { get; }

        public Type MethodType { get; }
        public string MethodName { get; }
        public object[] Args { get; }

        public ObjectEventsArgs(MultiplayerReference target, Type methodType, string methodName, object[] args) {
            Target = target;
            MethodType = methodType;
            MethodName = methodName;
            Args = args;
        }
    }

}
