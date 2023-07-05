using System;
using HarmonyLib;
using MultiplayerMod.Core.Patch;
using MultiplayerMod.Multiplayer.Objects;

namespace MultiplayerMod.Game.Mechanics;

[HarmonyPatch(typeof(Door))]
public static class DoorToggleEvents {

    public static event EventHandler<DoorStateChangedEventArgs> StateChanged;

    [HarmonyPostfix]
    [HarmonyPatch(nameof(Door.QueueStateChange))]
    private static void QueueStateChangePostfix(Door __instance, Door.ControlState nextState) =>
        PatchControl.RunIfEnabled(
            () => StateChanged?.Invoke(
                __instance,
                new DoorStateChangedEventArgs {
                    Target = __instance.GetMultiplayerReference(),
                    State = nextState
                }
            )
        );

}
