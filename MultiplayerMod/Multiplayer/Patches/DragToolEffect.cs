using System;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.Patches
{

    [Obsolete("These patches are deprecated, will be replaced soon. No command adaptation performed.")]
    public abstract class DragToolEffect
    {
        public static void OnDragTool(DragTool dragTool, object payload)
        {
            var array = (object[])payload;
            var priority = (int)array[0];
            var cell = (int)array[1];
            var distFromOrigin = (int)array[2];

            DragToolPatches.DisablePatch = true;

            var currentPriority = ToolMenu.Instance.PriorityScreen.GetLastSelectedPriority();
            var newPriority = new PrioritySetting(PriorityScreen.PriorityClass.basic, priority);

            ToolMenu.Instance.PriorityScreen.SetScreenPriority(newPriority);

            if (dragTool is BaseUtilityBuildTool tool)
            {
                tool.BuildPath();
            }
            else
            {
                dragTool.OnDragTool(cell, distFromOrigin);
            }

            ToolMenu.Instance.PriorityScreen.SetScreenPriority(currentPriority);
            if (dragTool is BuildTool buildToolAfter)
            {
                buildToolAfter.Deactivate();
                PlanScreen.Instance.ProductInfoScreen.materialSelectionPanel.PriorityScreen.SetScreenPriority(
                    currentPriority);
            }
            else if (dragTool is BaseUtilityBuildTool)
            {
                PlanScreen.Instance.ProductInfoScreen.materialSelectionPanel.PriorityScreen.SetScreenPriority(
                    currentPriority);
            }

            DragToolPatches.DisablePatch = false;
        }

        public static void OnDragComplete(DragTool dragTool, object payload)
        {
            var array = (object[])payload;
            var priority = (int)array[0];
            var downPos = new Vector3((float)array[1], (float)array[2], (float)array[3]);
            var upPos = new Vector3((float)array[4], (float)array[5], (float)array[6]);

            DragToolPatches.DisablePatch = true;

            var currentPriority = ToolMenu.Instance.PriorityScreen.GetLastSelectedPriority();
            ToolMenu.Instance.PriorityScreen.SetScreenPriority(
                new PrioritySetting(PriorityScreen.PriorityClass.basic, priority));

            dragTool.OnDragComplete(downPos, upPos);
            ToolMenu.Instance.PriorityScreen.SetScreenPriority(currentPriority);

            DragToolPatches.DisablePatch = false;
        }
    }
}
