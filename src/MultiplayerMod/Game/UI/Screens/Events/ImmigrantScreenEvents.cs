using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using HarmonyLib;
using MultiplayerMod.Core.Scheduling;
using MultiplayerMod.ModRuntime.Context;
using MultiplayerMod.ModRuntime.StaticCompatibility;

namespace MultiplayerMod.Game.UI.Screens.Events;

[HarmonyPatch(typeof(ImmigrantScreen))]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
public static class ImmigrantScreenEvents {
    public static event Action<List<ITelepadDeliverable?>?>? Initialize;

    [HarmonyPostfix]
    [HarmonyPatch(nameof(ImmigrantScreen.Initialize))]
    [RequireExecutionLevel(ExecutionLevel.Game)]
    private static void InitializePatch(ImmigrantScreen __instance) =>
        new TaskFactory(Dependencies.Get<UnityTaskScheduler>()).StartNew(
            async () => {
                if (ImmigrantScreenPatch.Deliverables != null) return;

                var readyDeliverables = await ScreensUtils.WaitForAllDeliverablesReady(__instance);
                if (readyDeliverables != null) {
                    Initialize?.Invoke(readyDeliverables);
                    ImmigrantScreenPatch.Deliverables = readyDeliverables;
                }
            }
        );

}
