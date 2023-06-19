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
    public static event Action<List<ITelepadDeliverable>> Initialize;
    public static event System.Action Reject;
    public static event Action<ITelepadDeliverable> Proceed;

    [HarmonyPostfix]
    [HarmonyPatch(nameof(ImmigrantScreen.Initialize))]
    private static void InitializePatch(ImmigrantScreen __instance) => PatchControl.RunIfEnabled(
        () => new TaskFactory(Container.Get<UnityTaskScheduler>()).StartNew(
            async () => {
                if (ImmigrantScreenPatch.Deliverables != null) return;

                var readyDeliverables = await ImmigrantScreenPatch.WaitForAllDeliverablesReady(__instance);
                if (readyDeliverables != null) {
                    Initialize?.Invoke(readyDeliverables);
                    ImmigrantScreenPatch.Deliverables = readyDeliverables;
                }
            }
        )
    );

    [HarmonyPostfix]
    [HarmonyPatch(nameof(ImmigrantScreen.OnRejectionConfirmed))]
    private static void OnRejectionConfirmed() => PatchControl.RunIfEnabled(() => Reject?.Invoke());

    [HarmonyPrefix]
    [HarmonyPatch("OnProceed")]
    private static void OnProceed(ImmigrantScreen __instance) =>
        PatchControl.RunIfEnabled(() => Proceed?.Invoke(__instance.selectedDeliverables[0]));

}
