using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using MultiplayerMod.Game.Context;

namespace MultiplayerMod.Game.UI.Tools.Context;

[HarmonyPatch]
public class DisableBuildingValidation : IGameContext {

    private static bool validationEnabled = true;

    private static IEnumerable<MethodBase> TargetMethods() => typeof(BuildingDef).GetMethods()
        .Where(it => it.Name.StartsWith("IsValidPlaceLocation"));

    [HarmonyPrefix]
    private static bool IsValidPlaceLocationPrefix(ref bool __result) {
        if (validationEnabled)
            return true;

        __result = true;
        return false;
    }

    public void Apply() {
        validationEnabled = false;
    }

    public void Restore() {
        validationEnabled = true;
    }

}
