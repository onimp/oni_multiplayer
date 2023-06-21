using HarmonyLib;
using MultiplayerMod.Game.Context;

namespace MultiplayerMod.Game.Effects;

[HarmonyPatch(typeof(PriorityScreen))]
public class DisablePriorityConfirmSound : IGameContext {

    private static bool enabled = true;

    [HarmonyPrefix]
    [HarmonyPatch(nameof(PriorityScreen.PlayPriorityConfirmSound))]
    private static bool PlayPriorityConfirmSoundPrefix() => enabled;

    public void Apply() => enabled = false;

    public void Restore() => enabled = true;

}
