using System;
using System.Linq;
using HarmonyLib;
using MultiplayerMod.Core.Patch;

namespace MultiplayerMod.Game.UI.Tools.Events;

[HarmonyPatch(typeof(BaseUtilityBuildTool))]
public static class UtilityBuildEvents {

    public static EventHandler<UtilityBuildEventArgs> Build;

    [HarmonyPrefix]
    [HarmonyPatch(nameof(BaseUtilityBuildTool.BuildPath))]
    private static void BuildPathPrefix(BaseUtilityBuildTool __instance) => PatchControl.RunIfEnabled(
        () => {
            var args = new UtilityBuildEventArgs {
                PrefabId = __instance.def.PrefabID,
                Materials = __instance.selectedElements.ToArray(),
                Path = __instance.path,
                Priority = GameState.BuildToolPriority
            };
            Build?.Invoke(__instance, args);
        }
    );

}
