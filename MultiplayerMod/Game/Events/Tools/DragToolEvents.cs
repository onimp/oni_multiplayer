using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using MultiplayerMod.Core.Patch;
using UnityEngine;

namespace MultiplayerMod.Game.Events.Tools;

[HarmonyPatch]
public static class DragToolEvents {

    public static event EventHandler<DragCompleteEventArgs> DragComplete;

    private static DragTool lastTool;
    private static readonly List<int> selection = new();

    #region Dig

    [HarmonyPostfix]
    [HarmonyPatch(typeof(DigTool), nameof(DigTool.OnDragTool))]
    private static void DigToolOnDragToolPostfix(DigTool __instance, int cell) =>
        AddDragCell(__instance, cell);

    #endregion

    #region Cancel

    [HarmonyPostfix]
    [HarmonyPatch(typeof(CancelTool), nameof(CancelTool.OnDragTool))]
    private static void CancelToolOnDragToolPostfix(CancelTool __instance, int cell) =>
        AddDragCell(__instance, cell);

    [HarmonyPostfix]
    [HarmonyPatch(typeof(CancelTool), nameof(CancelTool.OnDragComplete))]
    private static void CancelToolOnDragCompletePostfix(CancelTool __instance, Vector3 downPos, Vector3 upPos) =>
        CompleteDrag(__instance, downPos, upPos);

    #endregion

    #region Deconstruct

    [HarmonyPostfix]
    [HarmonyPatch(typeof(DeconstructTool), nameof(DeconstructTool.OnDragTool))]
    private static void DeconstructToolOnDragToolPostfix(DeconstructTool __instance, int cell) =>
        AddDragCell(__instance, cell);

    #endregion

    #region Prioritize

    [HarmonyPostfix]
    [HarmonyPatch(typeof(PrioritizeTool), nameof(PrioritizeTool.OnDragTool))]
    private static void PrioritizeToolOnDragToolPostfix(PrioritizeTool __instance, int cell) =>
        AddDragCell(__instance, cell);

    #endregion

    #region Disinfect

    [HarmonyPostfix]
    [HarmonyPatch(typeof(DisinfectTool), nameof(DisinfectTool.OnDragTool))]
    private static void DisinfectToolOnDragToolPostfix(DisinfectTool __instance, int cell) =>
        AddDragCell(__instance, cell);

    #endregion

    #region Clear (Sweep)

    [HarmonyPostfix]
    [HarmonyPatch(typeof(ClearTool), nameof(ClearTool.OnDragTool))]
    private static void ClearToolOnDragToolPostfix(ClearTool __instance, int cell) =>
        AddDragCell(__instance, cell);

    #endregion

    #region Attack

    [HarmonyPostfix]
    [HarmonyPatch(typeof(AttackTool), nameof(AttackTool.OnDragComplete))]
    private static void AttackToolOnDragCompletePostfix(AttackTool __instance, Vector3 downPos, Vector3 upPos) =>
        CompleteDrag(__instance, downPos, upPos);

    #endregion

    #region Mop

    [HarmonyPostfix]
    [HarmonyPatch(typeof(MopTool), nameof(MopTool.OnDragTool))]
    private static void MopToolOnDragToolPostfix(MopTool __instance, int cell) =>
        AddDragCell(__instance, cell);

    #endregion

    #region Capture (Wrangle)

    [HarmonyPostfix]
    [HarmonyPatch(typeof(CaptureTool), nameof(CaptureTool.OnDragComplete))]
    private static void CaptureToolOnDragCompletePostfix(CaptureTool __instance, Vector3 downPos, Vector3 upPos) =>
        CompleteDrag(__instance, downPos, upPos);

    #endregion

    #region Harvest

    [HarmonyPostfix]
    [HarmonyPatch(typeof(HarvestTool), nameof(HarvestTool.OnDragTool))]
    private static void HarvestToolOnDragToolPostfix(HarvestTool __instance, int cell) =>
        AddDragCell(__instance, cell);

    #endregion

    #region Empty pipe

    [HarmonyPostfix]
    [HarmonyPatch(typeof(EmptyPipeTool), nameof(EmptyPipeTool.OnDragTool))]
    private static void EmptyPipeToolOnDragToolPostfix(EmptyPipeTool __instance, int cell) =>
        AddDragCell(__instance, cell);

    #endregion

    #region Disconnect

    [HarmonyPostfix]
    [HarmonyPatch(typeof(DisconnectTool), nameof(DisconnectTool.OnDragComplete))]
    private static void DisconnectToolOnDragCompletePostfix(DisconnectTool __instance, Vector3 downPos, Vector3 upPos) {
        CompleteDrag(__instance, downPos, upPos);
    }

    #endregion

    [HarmonyPostfix]
    [HarmonyPatch(typeof(DragTool), nameof(DragTool.OnDragTool))]
    private static void DragToolOnDragToolPostfix(DragTool __instance, int cell) =>
        AddDragCell(__instance, cell);

    [HarmonyPostfix]
    [HarmonyPatch(typeof(DragTool), nameof(DragTool.OnDragComplete))]
    private static void DragToolOnDragCompletePostfix(DragTool __instance, Vector3 cursorDown, Vector3 cursorUp) =>
        CompleteDrag(__instance, cursorDown, cursorUp);

    private static void AddDragCell(DragTool __instance, int cell) => PatchControl.RunIfEnabled(
        () => {
            AssertSameInstance(__instance);
            selection.Add(cell);
            lastTool = __instance;
        }
    );

    private static void CompleteDrag(DragTool instance, Vector3 cursorDown, Vector3 cursorUp) =>
        PatchControl.RunIfEnabled(
            () => {
                AssertSameInstance(instance);

                var args = new DragCompleteEventArgs {
                    Cells = selection,
                    CursorDown = cursorDown,
                    CursorUp = cursorUp,
                    Priority = ToolMenu.Instance.PriorityScreen.GetLastSelectedPriority(),
                    Parameters = instance switch {
                        FilteredDragTool filtered => GetActiveParameters(filtered.currentFilterTargets),
                        HarvestTool harvest => GetActiveParameters(harvest.options),
                        _ => null
                    }
                };

                DragComplete?.Invoke(instance, args);

                selection.Clear();
                lastTool = null;
            }
        );

    private static void AssertSameInstance(DragTool instance) {
        if (lastTool != null && lastTool != instance)
            throw new Exception("Concurrent drag events detected");
    }

    private static string[] GetActiveParameters(Dictionary<string, ToolParameterMenu.ToggleState> parameters) {
        return parameters.Where(it => it.Value == ToolParameterMenu.ToggleState.On).Select(it => it.Key).ToArray();
    }

}
