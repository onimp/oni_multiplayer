using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using MultiplayerMod.Core.Patch;
using UnityEngine;

namespace MultiplayerMod.Game.UI.Tools.Events;

public static class DragToolEvents {

    public static event EventHandler<DragCompleteEventArgs>? DragComplete;

    private static DragTool? lastTool;
    private static readonly List<int> selection = new();

    private static readonly Type[] onDragToolClasses = {
        typeof(DragTool),
        typeof(DigTool),
        typeof(CancelTool),
        typeof(DeconstructTool),
        typeof(PrioritizeTool),
        typeof(DisinfectTool),
        typeof(ClearTool),
        typeof(MopTool),
        typeof(HarvestTool),
        typeof(EmptyPipeTool),
        typeof(DebugTool),
        typeof(CopySettingsTool)
    };

    private static readonly Type[] onDragCompleteClasses = {
        typeof(DragTool),
        typeof(CancelTool),
        typeof(AttackTool),
        typeof(CaptureTool),
        typeof(DisconnectTool)
    };

    [HarmonyPatch]
    // ReSharper disable once UnusedType.Local
    private class OnDragToolPatch {

        // ReSharper disable once UnusedMember.Local
        private static IEnumerable<MethodBase> TargetMethods()
            => Assembly.GetAssembly(typeof(DragTool))
                .GetTypes()
                .Where(type => onDragToolClasses.Contains(type))
                .Select(
                    type => type.GetMethod(
                        nameof(DragTool.OnDragTool),
                        BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly
                    )
                );

        [HarmonyPostfix]
        // ReSharper disable once UnusedMember.Local
        private static void DragToolOnDragToolPostfix(DragTool __instance, int cell) =>
            AddDragCell(__instance, cell);

        private static void AddDragCell(DragTool __instance, int cell) =>
            PatchControl.RunIfEnabled(
                () => {
                    AssertSameInstance(__instance);
                    selection.Add(cell);
                    lastTool = __instance;
                }
            );
    }

    [HarmonyPatch]
    // ReSharper disable once UnusedType.Local
    private class OnDragCompletePatch {

        // ReSharper disable once UnusedMember.Local
        private static IEnumerable<MethodBase> TargetMethods()
            => Assembly.GetAssembly(typeof(DragTool))
                .GetTypes()
                .Where(type => onDragCompleteClasses.Contains(type))
                .Select(
                    type => type.GetMethod(
                        nameof(DragTool.OnDragComplete),
                        BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly
                    )
                );

        [HarmonyPostfix]
        // ReSharper disable once UnusedMember.Local
        private static void DragToolOnDragCompletePostfix(DragTool __instance, Vector3 __0, Vector3 __1) =>
            CompleteDrag(__instance, __0, __1);

        private static void CompleteDrag(DragTool instance, Vector3 cursorDown, Vector3 cursorUp) =>
            PatchControl.RunIfEnabled(
                () => {
                    AssertSameInstance(instance);

                    var args = new DragCompleteEventArgs(
                        selection,
                        cursorDown,
                        cursorUp,
                        ToolMenu.Instance.PriorityScreen.GetLastSelectedPriority(),
                        instance switch {
                            FilteredDragTool filtered => GetActiveParameters(filtered.currentFilterTargets),
                            HarvestTool harvest => GetActiveParameters(harvest.options),
                            _ => null
                        }
                    );

                    DragComplete?.Invoke(instance, args);

                    selection.Clear();
                    lastTool = null;
                }
            );

        private static string[] GetActiveParameters(Dictionary<string, ToolParameterMenu.ToggleState> parameters) {
            return parameters.Where(it => it.Value == ToolParameterMenu.ToggleState.On).Select(it => it.Key).ToArray();
        }
    }

    private static void AssertSameInstance(DragTool instance) {
        if (lastTool != null && lastTool != instance)
            throw new Exception("Concurrent drag events detected");
    }
}
