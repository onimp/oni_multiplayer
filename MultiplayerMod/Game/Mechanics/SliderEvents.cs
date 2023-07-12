using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using MultiplayerMod.Core.Patch;
using MultiplayerMod.Multiplayer.Objects;

namespace MultiplayerMod.Game.Mechanics;

public static class SliderEvents {

    public static event Action<MultiplayerReference, float, int> SetSliderValue;

    [HarmonyPatch]
    // ReSharper disable once UnusedType.Local
    private static class SliderControlEvents {
        // ReSharper disable once UnusedMember.Local
        private static IEnumerable<MethodBase> TargetMethods() {
            return Assembly.GetAssembly(typeof(ISliderControl))
                .GetTypes()
                .Where(type => type.IsClass && !type.IsAbstract && typeof(ISliderControl).IsAssignableFrom(type))
                .Distinct()
                .Select(
                    type => {
                        var method = type.GetMethod("SetSliderValue");
                        return method != null
                            ? method
                            : type.GetMethod(
                                "ISliderControl.SetSliderValue",
                                BindingFlags.NonPublic | BindingFlags.Instance
                            );
                    }
                )
                .ToList();
        }

        [HarmonyPostfix]
        private static void SetSliderValuePostfix(ISliderControl __instance, float __0, int index) =>
            PatchControl.RunIfEnabled(
                () => SetSliderValue?.Invoke(
                    ((KMonoBehaviour) __instance).GetMultiplayerReference(),
                    __0,
                    index
                )
            );
    }

}
