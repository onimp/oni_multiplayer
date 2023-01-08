using HarmonyLib;
using UnityEngine;

namespace MultiplayerMod.patch
{
    [HarmonyPatch(typeof(WorldSelector), "OnSpawn")]
    public class PinnedResourcesPanelPatch
    {
        private static bool first = true;

        public static void Postfix(WorldSelector __instance)
        {
            if (!first) return;
            first = false;
            // var t = __instance.headerButton.gameObject;
            // GameObject topRight = t;
            // while (t != null)
            // {
            //     Debug.Log(t.name);
            //     if (t.name == "TopRight")
            //         topRight = t;
            //     t = t?.transform?.parent?.gameObject;
            // }
            //
            // Debug.Log("Childrens");
            // var componentsInChildren = topRight.GetComponentsInChildren(typeof(KScreen));
            // foreach (var c in componentsInChildren)
            // {
            //     Debug.Log(c);
            // }

         //   var newButton = GameObject.Instantiate(__instance);
        //    newButton.GetComponentInChildren<LocText>().SetText("Players");
        }
    }
}