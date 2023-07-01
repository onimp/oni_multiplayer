using HarmonyLib;
using ImGuiNET;

namespace MultiplayerMod.Core.Unity.Dev;

[HarmonyPatch(typeof(DevToolSceneInspector))]
public class DevToolSceneInspectorPatch {

    [HarmonyPrefix]
    [HarmonyPatch(nameof(DevToolSceneInspector.DisplayField))]
    private static bool DisplayFieldPrefix(ref bool __result, string name, System.Type ft, ref object obj) {
        if (ft != typeof(long?))
            return true;

        var v = (long?) obj;
        ImGui.LabelText(name, v.Value.ToString());
        __result = true;
        return false;
    }

}
