using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using HarmonyLib;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Patch;
using MultiplayerMod.Core.Scheduling;

namespace MultiplayerMod.Game.UI.Screens.Events;

[HarmonyPatch(typeof(ImmigrantScreen))]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
public static class ImmigrantScreenEvents {
    public static event Action<List<ITelepadDeliverable?>?>? Initialize;

    [HarmonyPostfix]
    [HarmonyPatch(nameof(ImmigrantScreen.Initialize))]
    private static void InitializePatch(ImmigrantScreen __instance) => PatchControl.RunIfEnabled(
        () => new TaskFactory(Dependencies.Get<UnityTaskScheduler>()).StartNew(
            async () => {
                if (ImmigrantScreenPatch.Deliverables != null) return;

                var readyDeliverables = await ScreensUtils.WaitForAllDeliverablesReady(__instance);
                if (readyDeliverables != null) {
                    Initialize?.Invoke(readyDeliverables);
                    ImmigrantScreenPatch.Deliverables = readyDeliverables;
                }
            }
        )
    );

}
