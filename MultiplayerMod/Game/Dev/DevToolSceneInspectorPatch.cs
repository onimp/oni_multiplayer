using HarmonyLib;
using ImGuiNET;

namespace MultiplayerMod.Game.Dev;

// ReSharper disable once UnusedType.Global
[HarmonyPatch(typeof(DevToolSceneInspector))]
public class DevToolSceneInspectorPatch {

    // ReSharper disable once UnusedMember.Local
    [HarmonyPrefix]
    [HarmonyPatch(nameof(DevToolSceneInspector.DisplayField))]
    private static bool DisplayFieldPrefix(ref bool __result, string name, System.Type ft, ref object obj) {
        var longType = ft == typeof(long) || ft == typeof(ulong);
        if (!longType)
            return true;

        ImGui.LabelText(name, obj.ToString());
        __result = true;
        return false;
    }

}
