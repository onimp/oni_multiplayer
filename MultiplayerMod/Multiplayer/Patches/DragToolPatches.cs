using System;
using HarmonyLib;

namespace MultiplayerMod.Multiplayer.Patches
{

    [Obsolete("These patches are deprecated, will be replaced soon. No command adaptation performed.")]
    public static class DragToolPatches
    {
        public static bool DisablePatch;

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
