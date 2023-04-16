using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;

namespace MultiplayerMod.Game.Building;

[HarmonyPatch]
public static class BuildValidation {

    public static bool Enabled { get; set; } = true;

    static IEnumerable<MethodBase> TargetMethods() => typeof(BuildingDef).GetMethods()
        .Where(it => it.Name.StartsWith("IsValidPlaceLocation"));

    [HarmonyPrefix]
    private static bool IsValidPlaceLocationPrefix(ref bool __result) {
        if (Enabled)
            return true;

        __result = true;
        return false;
    }

}
