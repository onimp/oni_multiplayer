using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using MultiplayerMod.Game.Context;

namespace MultiplayerMod.Game.Effects;

[HarmonyPatch]
public class DisablePopUpEffects : IGameContext {

    private static bool enabled = true;

    // ReSharper disable once UnusedMember.Local
    private static IEnumerable<MethodBase> TargetMethods() =>
        typeof(PopFXManager).GetMethods()
            .Where(it => it.Name.StartsWith("SpawnFX"));

    [HarmonyPrefix]
    // ReSharper disable once UnusedMember.Local
    private static bool SpawnFxPrefix() => enabled;

    public void Apply() {
        enabled = false;
    }

    public void Restore() {
        enabled = true;
    }

}
