using HarmonyLib;
using JetBrains.Annotations;

namespace MultiplayerMod.Test.GameRuntime.Patches;

[UsedImplicitly]
[HarmonyPatch(typeof(ElementLoader))]
public class ElementLoaderPatch {

    [UsedImplicitly]
    [HarmonyPrefix]
    [HarmonyPatch(nameof(ElementLoader.FindElementByHash))]
    private static bool ElementLoader_FindElementByHash(ref Element __result) {
        __result = new Element() { name = "Patched element", substance = new Substance() { anim = new KAnimFile() {IsBuildLoaded = true} } };
        return false;
    }

}
