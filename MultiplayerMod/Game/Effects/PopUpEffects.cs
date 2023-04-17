using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;

namespace MultiplayerMod.Game.Effects;

[HarmonyPatch]
public static class PopUpEffects {

    public static bool Enabled { get; set; } = true;

    static IEnumerable<MethodBase> TargetMethods() => typeof(PopFXManager).GetMethods()
        .Where(it => it.Name.StartsWith("SpawnFX"));

    [HarmonyPrefix]
    private static bool SpawnFxPrefix() => Enabled;

}
