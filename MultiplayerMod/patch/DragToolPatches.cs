using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace MultiplayerMod.patch
{

    public static class DragToolPatches
    {
        public static bool DisablePatch;

        [HarmonyPatch(typeof(AttackTool), "OnDragComplete")]
        public static class AttackToolPatch
        {
            public static event Action<object> OnDragComplete;

            public static void Postfix(Vector3 downPos, Vector3 upPos)
            {
                if (DisablePatch) return;

                OnDragComplete?.Invoke(
                    new object[]
                    {
                        ToolMenu.Instance.PriorityScreen.GetLastSelectedPriority().priority_value,
                        downPos.x, downPos.y, downPos.z, upPos.x, upPos.y, upPos.z
                    }
                );
            }
        }

        [HarmonyPatch(typeof(BaseUtilityBuildTool), "OnLeftClickUp")]
        public static class BaseUtilityBuildToolPatch
        {
            public static event Action<object> OnWireTool;
            public static event Action<object> OnUtilityTool;

            public static void Prefix(BaseUtilityBuildTool __instance, Vector3 cursor_pos)
            {
                if (DisablePatch) return;

                (__instance is UtilityBuildTool ? OnUtilityTool : OnWireTool)?.Invoke(
                    new object[]
                    {
                        ToolMenu.Instance.PriorityScreen.GetLastSelectedPriority().priority_value,
                        Grid.PosToCell(cursor_pos),
                        0,
                        new object[]
                        {
                            __instance.GetPrivateField<BaseUtilityBuildTool, BuildingDef>("def").PrefabID,
                            __instance.GetPrivateField<BaseUtilityBuildTool, IList<Tag>>("selectedElements")!,
                            GetPath(__instance)
                        }
                    }
                );
            }

            private static List<int> GetPath(BaseUtilityBuildTool buildTool)
            {
                var path = buildTool.GetPrivateField<object>("path");
                var intPath = new List<int>();
                for (var index = 0; index < path.GetProperty<int>("Count"); index++)
                {
                    var node = path.Invoke("get_Item", index);
                    intPath.Add(node.GetField<int>("cell"));
                }

                return intPath;
            }
        }

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
                            __instance.GetPrivateField<BuildingDef>("def").PrefabID,
                            __instance.GetPrivateField<string>("facadeID"),
                            __instance.GetPrivateField<IList<Tag>>("selectedElements"),
                            __instance.GetPrivateField<Orientation>("buildingOrientation")
                        }
                    }
                );
            }
        }

        [HarmonyPatch(typeof(CancelTool), "OnDragTool")]
        public static class CancelToolPatch
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

        [HarmonyPatch(typeof(CaptureTool), "OnDragComplete")]
        public static class CaptureToolPatch
        {
            public static event Action<object> OnDragComplete;

            public static void Postfix(Vector3 downPos, Vector3 upPos)
            {
                if (DisablePatch) return;

                OnDragComplete?.Invoke(
                    new object[]
                    {
                        ToolMenu.Instance.PriorityScreen.GetLastSelectedPriority().priority_value,
                        downPos.x, downPos.y, downPos.z, upPos.x, upPos.y, upPos.z
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

        [HarmonyPatch(typeof(ClearTool), "OnDragTool")]
        public static class ClearToolPatch
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

        [HarmonyPatch(typeof(DeconstructTool), "OnDragTool")]
        public static class DeconstructToolPatch
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

        [HarmonyPatch(typeof(DigTool), "OnDragTool")]
        public static class DigToolPatch
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

        [HarmonyPatch(typeof(DisinfectTool), "OnDragTool")]
        public static class DisinfectToolPatch
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

        [HarmonyPatch(typeof(EmptyPipeTool), "OnDragTool")]
        public static class EmptyPipeToolPatch
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

        [HarmonyPatch(typeof(HarvestTool), "OnDragTool")]
        public static class HarvestToolPatch
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

        [HarmonyPatch(typeof(MopTool), "OnDragTool")]
        public static class MopToolPatch
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

        [HarmonyPatch(typeof(PrioritizeTool), "OnDragTool")]
        public static class PrioritizeToolPatch
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
