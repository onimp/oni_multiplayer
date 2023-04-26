using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using MultiplayerMod.Core.Patch;
using Object = UnityEngine.Object;

namespace MultiplayerMod.Game.Tools.Events;

[HarmonyPatch(typeof(BuildTool))]
public static class BuildEvents {

    public static event EventHandler<BuildEventArgs> Build;

    [HarmonyTranspiler]
    [HarmonyPatch(nameof(BuildTool.TryBuild))]
    private static IEnumerable<CodeInstruction> TryBuildTranspiler(
        IEnumerable<CodeInstruction> instructions,
        ILGenerator generator
    ) {
        using var source = instructions.GetEnumerator();
        var result = new List<CodeInstruction>();
        var lastDragCellField = AccessTools.Field(typeof(BuildTool), nameof(BuildTool.lastDragCell));
        var instantBuildReplaceMethod = AccessTools.Method(typeof(BuildTool), nameof(BuildTool.InstantBuildReplace));
        var postProcessBuildMethod = AccessTools.Method(typeof(BuildTool), nameof(BuildTool.PostProcessBuild));

        result.AddConditional(source, it => it.StoresField(lastDragCellField));

        // var replaced = false;
        var replaced = generator.DeclareLocal(typeof(bool));
        result.Add(new CodeInstruction(OpCodes.Ldc_I4_0));
        result.Add(new CodeInstruction(OpCodes.Stloc_S, replaced));

        // var asyncReplace = false;
        var asyncReplace = generator.DeclareLocal(typeof(bool));
        result.Add(new CodeInstruction(OpCodes.Ldc_I4_0));
        result.Add(new CodeInstruction(OpCodes.Stloc_S, asyncReplace));

        result.AddConditional(source, it => it.IsStoreToLocal(4));

        // replaced = true;
        result.Add(new CodeInstruction(OpCodes.Ldc_I4_1));
        result.Add(new CodeInstruction(OpCodes.Stloc_S, replaced));

        result.AddConditional(source, it => it.Calls(instantBuildReplaceMethod));
        result.AddRange(source, 1); // stloc.1

        // asyncReplace = true;
        result.Add(new CodeInstruction(OpCodes.Ldc_I4_1));
        result.Add(new CodeInstruction(OpCodes.Stloc_S, asyncReplace));

        result.AddConditional(source, it => it.Calls(postProcessBuildMethod));

        // if (builtItem != null || asyncReplace)
        var falseCondition = generator.DefineLabel();
        result.Add(new CodeInstruction(OpCodes.Ldloc_1));
        result.Add(new CodeInstruction(OpCodes.Ldnull));
        result.Add(CodeInstruction.Call(typeof(Object), "op_Inequality", new[] { typeof(Object), typeof(Object) }));
        result.Add(new CodeInstruction(OpCodes.Ldloc_S, asyncReplace));
        result.Add(new CodeInstruction(OpCodes.Or));
        result.Add(new CodeInstruction(OpCodes.Brfalse_S, falseCondition));

        // BuildEvents.BuildComplete(this, cell, instantBuild, replaced)
        result.Add(new CodeInstruction(OpCodes.Ldarg_0));
        result.Add(new CodeInstruction(OpCodes.Ldarg_1));
        result.Add(new CodeInstruction(OpCodes.Ldloc_2));
        result.Add(new CodeInstruction(OpCodes.Ldloc_S, replaced));
        result.Add(CodeInstruction.Call(typeof(BuildEvents), nameof(BuildComplete)));

        result.AddConditional(source, _ => false);

        result.Last().labels.Add(falseCondition);

        return result;
    }

    private static void BuildComplete(BuildTool tool, int cell, bool instantBuild, bool replaced) {
        Build?.Invoke(
            tool,
            new BuildEventArgs {
                Cell = cell,
                InstantBuild = instantBuild,
                Upgrade = replaced,
                FacadeId = tool.facadeID,
                Materials = tool.selectedElements.ToArray(),
                Orientation = tool.buildingOrientation,
                Priority = GameState.BuildToolPriority,
                PrefabId = tool.def.PrefabID
            }
        );
    }

}
