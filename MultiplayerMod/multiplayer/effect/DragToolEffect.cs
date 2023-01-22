using System.Collections.Generic;
using MultiplayerMod.patch;
using UnityEngine;

namespace MultiplayerMod.multiplayer.effect
{
    public abstract class DragToolEffect
    {
        public static void OnDragTool(DragTool dragTool, object payload)
        {
            var array = (object[])payload;
            var priority = (int)array[0];
            var cell = (int)array[1];
            var distFromOrigin = (int)array[2];
            var isBuildTool = array.Length > 3;
            if (isBuildTool)
            {
                PlanScreen.Instance.ProductInfoScreen.Awake();
                PlanScreen.Instance.ProductInfoScreen.materialSelectionPanel.InvokePrivate("OnPrefabInit");
                var additionalObject = (object[])array[3];
                var def = Assets.GetBuildingDef((string)additionalObject[0]);
                var facadeId = (string)additionalObject[1];
                var selectedElements = (IList<Tag>)additionalObject[2];
                if (def.isKAnimTile && def.isUtility)
                {
                    (def.BuildingComplete.GetComponent<Wire>() != null
                        ? WireBuildTool.Instance
                        : (BaseUtilityBuildTool)UtilityBuildTool.Instance).Activate(def, selectedElements);
                }
                else
                    BuildTool.Instance.Activate(def, selectedElements, facadeId);
            }

            DragToolPatches.DisablePatch = true;

            var currentPriority = ToolMenu.Instance.PriorityScreen.GetLastSelectedPriority();
            var newPriority = new PrioritySetting(PriorityScreen.PriorityClass.basic, priority);
            ToolMenu.Instance.PriorityScreen.SetScreenPriority(newPriority);
            if (isBuildTool)
            {
                PlanScreen.Instance.ProductInfoScreen.materialSelectionPanel.PriorityScreen.SetScreenPriority(
                    newPriority);
            }

            dragTool.InvokePrivate("OnDragTool", cell, distFromOrigin);

            ToolMenu.Instance.PriorityScreen.SetScreenPriority(currentPriority);
            if (isBuildTool)
            {
                BuildTool.Instance.Deactivate();
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

            dragTool.InvokePrivate("OnDragComplete", downPos, upPos);
            ToolMenu.Instance.PriorityScreen.SetScreenPriority(currentPriority);

            DragToolPatches.DisablePatch = false;
        }
    }
}