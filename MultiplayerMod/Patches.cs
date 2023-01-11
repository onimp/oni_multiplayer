using System;
using System.Reflection;
using HarmonyLib;
using Steamworks;
using UnityEngine;

namespace MultiplayerMod
{
    public class Patches
    {
   /*   
        [HarmonyPatch(typeof(InterfaceTool), nameof(InterfaceTool.OnLeftClickUp))]
        public class InterfaceToolOnLeftClickUpPatch
        {
            public static void Prefix(object __instance)
            {
                Debug.Log("InterfaceTool OnLeftClickUp Before");
                Debug.Log(__instance);
                Debug.Log(Environment.StackTrace);
            }
        }

        [HarmonyPatch(typeof(InterfaceTool), nameof(InterfaceTool.OnMouseMove))]
        public class InterfaceToolOnMouseMovePatch
        {
            public static void Prefix(Vector3 cursor_pos)
            {
                // Mouse move
                // Need to send cursor pos to other participants/server
            }
        }

        [HarmonyPatch(typeof(AutoDisinfectable), "OnPrefabInit")]
        public class AutoDisinfectablePatch
        {
            public static void Prefix()
            {
                Debug.Log("AutoDisinfectable after");
                Debug.Log(Environment.StackTrace);
            }
        }

        // move to location smi.MoveToLocation(mouseCell);

        // InterfaceTool are user actions in the UI


        [HarmonyPatch(typeof(DragTool), "OnDragTool")]
        public class DragToolPatch
        {
            public static void Prefix()
            {
                Debug.Log("DragTool OnDragTool");
            }
        }

        [HarmonyPatch(typeof(SelectTool), "Select")]
        public class SelectToolPatch
        {
            public static void Prefix(SelectTool __instance, KSelectable new_selected)
            {
                Debug.Log("SelectTool Select");
                Debug.Log(new_selected);
                Debug.Log(new_selected?.GetType());
            }
        }*/


        // actionable items 493375141
        // sliders
        // KScreem

        //SaveLoader
        /**
         * 
  at MultiplayerMod.Patches+WorkablePatch.Postfix () [0x00000] in <d9cd0e63f1904cb89637b0e5891ccde2>:0 
  at Workable.Workable.OnPrefabInit_Patch1 (Workable ) [0x00000] in <d13cbb0b55a94ef09bb55c2436a6b8ee>:0 
  at Repairable.OnPrefabInit () [0x00000] in <d13cbb0b55a94ef09bb55c2436a6b8ee>:0 
  at KMonoBehaviour.InitializeComponent () [0x00000] in <fa50cdddb14a483d94f547557a674a42>:0 
  at KMonoBehaviour.Awake () [0x00000] in <fa50cdddb14a483d94f547557a674a42>:0 
  at UnityEngine.GameObject.SetActive (System.Boolean value) [0x00000] in <72b60a3dd8cd4f12a155b761a1af9144>:0 
  at SaveLoadRoot.Load (UnityEngine.GameObject prefab, UnityEngine.Vector3 position, UnityEngine.Quaternion rotation, UnityEngine.Vector3 scale, IReader reader) [0x00000] in <d13cbb0b55a94ef09bb55c2436a6b8ee>:0 
  at SaveLoadRoot.Load (UnityEngine.GameObject prefab, IReader reader) [0x00000] in <d13cbb0b55a94ef09bb55c2436a6b8ee>:0 
  at SaveManager.Load (IReader reader) [0x00000] in <d13cbb0b55a94ef09bb55c2436a6b8ee>:0 
  at SaveLoader.Load (IReader reader) [0x00000] in <d13cbb0b55a94ef09bb55c2436a6b8ee>:0 
  at SaveLoader.Load (System.String filename) [0x00000] in <d13cbb0b55a94ef09bb55c2436a6b8ee>:0 
  at SaveLoader.OnSpawn () [0x00000] in <d13cbb0b55a94ef09bb55c2436a6b8ee>:0 
  at KMonoBehaviour.Spawn () [0x00000] in <fa50cdddb14a483d94f547557a674a42>:0 
  at KMonoBehaviour.Start () [0x00000] in <fa50cdddb14a483d94f547557a674a42>:0 
         */
    }
}