using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.Patches
{

    [Obsolete("These patches are deprecated, will be replaced soon. No command adaptation performed.")]
    public static class DragToolPatches
    {
        public static bool DisablePatch;

        [HarmonyPatch(typeof(BuildTool), "OnDragTool")]
        public static class BuildToolPatch
        {
            public static event Action<object> OnDragTool;

            public static void Prefix(BuildTool __instance, int cell, int distFromOrigin)
            {
                if (DisablePatch) return;

                OnDragTool?.Invoke(
                    new object[]
                    {
                        PlanScreen.Instance.ProductInfoScreen.materialSelectionPanel.PriorityScreen
                            .GetLastSelectedPriority().priority_value,
                        cell,
                        distFromOrigin,
                        new object[]
                        {
                            __instance.def.PrefabID,
                            __instance.facadeID,
                            __instance.selectedElements,
                            __instance.buildingOrientation
                        }
                    }
                );
            }
        }



        [HarmonyPatch(typeof(CopySettingsTool), "OnDragTool")]
        public static class CopySettingsToolPatch
        {
            public static event Action<object> OnDragTool;

            public static void Postfix(int cell, int distFromOrigin)
            {
                if (DisablePatch) return;

                OnDragTool?.Invoke(
                    new object[]
                    {
                        ToolMenu.Instance.PriorityScreen.GetLastSelectedPriority().priority_value, cell, distFromOrigin
                    }
                );
            }
        }



        [HarmonyPatch(typeof(DebugTool), "OnDragTool")]
        public static class DebugToolPatch
        {
            public static event Action<object> OnDragTool;

            public static void Postfix(int cell, int distFromOrigin)
            {
                if (DisablePatch) return;

                OnDragTool?.Invoke(
                    new object[]
                    {
                        ToolMenu.Instance.PriorityScreen.GetLastSelectedPriority().priority_value, cell, distFromOrigin
                    }
                );
            }
        }


        [HarmonyPatch(typeof(PlaceTool), "OnDragTool")]
        public static class PlaceToolPatch
        {
            public static event Action<object> OnDragTool;

            public static void Postfix(int cell, int distFromOrigin)
            {
                if (DisablePatch) return;

                OnDragTool?.Invoke(
                    new object[]
                    {
                        ToolMenu.Instance.PriorityScreen.GetLastSelectedPriority().priority_value, cell, distFromOrigin
                    }
                );
            }
        }


    }

}
