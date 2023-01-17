using System.Reflection;
using MultiplayerMod.patch;
using UnityEngine;

namespace MultiplayerMod.multiplayer.effect
{
    public abstract class DragToolEffect
    {
        public static void OnDragTool(DragTool dragTool, object payload)
        {
            var array = (int[])payload;
            var priority = array[0];
            var cell = array[1];
            var distFromOrigin = array[2];

            DragToolPatches.DisablePatch = true;

            var currentPriority = ToolMenu.Instance.PriorityScreen.GetLastSelectedPriority();
            ToolMenu.Instance.PriorityScreen.SetScreenPriority(
                new PrioritySetting(PriorityScreen.PriorityClass.basic, priority));

            var method = dragTool.GetType().GetMethod("OnDragTool", BindingFlags.NonPublic | BindingFlags.Instance);
            method?.Invoke(dragTool, new object[] { cell, distFromOrigin });
            ToolMenu.Instance.PriorityScreen.SetScreenPriority(currentPriority);

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

            var method = dragTool.GetType().GetMethod("OnDragComplete", BindingFlags.NonPublic | BindingFlags.Instance);
            method?.Invoke(dragTool, new object[] { downPos, upPos });
            ToolMenu.Instance.PriorityScreen.SetScreenPriority(currentPriority);

            DragToolPatches.DisablePatch = false;
        }
    }
}