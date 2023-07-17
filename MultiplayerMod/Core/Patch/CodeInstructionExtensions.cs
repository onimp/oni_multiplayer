using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using static System.Reflection.Emit.OperandType;

namespace MultiplayerMod.Core.Patch;

public static class CodeInstructionExtensions {

    public static bool IsStoreToLocal(this CodeInstruction instruction, int variableIndex) {
        if (instruction.opcode == OpCodes.Stloc_0)
            return variableIndex == 0;
        if (instruction.opcode == OpCodes.Stloc_1)
            return variableIndex == 1;
        if (instruction.opcode == OpCodes.Stloc_2)
            return variableIndex == 2;
        if (instruction.opcode == OpCodes.Stloc_3)
            return variableIndex == 3;

        if (instruction.opcode == OpCodes.Stloc || instruction.opcode == OpCodes.Stloc_S) {
            var index = instruction.operand switch {
                int i => i,
                LocalBuilder b => b.LocalIndex,
                _ => -1
            };
            return variableIndex == index;
        }
        return false;
    }

    public static void AddConditional(
        this IList<CodeInstruction> instructions,
        IEnumerator<CodeInstruction> enumerator,
        Func<CodeInstruction, bool> stopWhen,
        bool includeMatch = true
    ) {
        while (enumerator.MoveNext()) {
            var instruction = enumerator.Current;
            if (stopWhen(instruction)) {
                if (includeMatch)
                    instructions.Add(instruction);
                return;
            }
            instructions.Add(instruction);
        }
    }

    public static void AddRange(
        this IList<CodeInstruction> instructions,
        IEnumerator<CodeInstruction> enumerator,
        int count
    ) {
        var added = 0;
        while (added++ < count && enumerator.MoveNext())
            instructions.Add(enumerator.Current);
    }

    public static int GetSize(this CodeInstruction instruction) =>
        instruction.opcode.Size + instruction.opcode.OperandType switch {
            InlineBrTarget or
                InlineField or
                InlineI or
                InlineMethod or
                InlineString or
                InlineTok or
                InlineType or
                ShortInlineR => 4,
            InlineI8 or
                InlineR => 8,
            InlineSwitch => (((Label[]) instruction.operand).Length + 1) * 4,
            InlineVar => 2,
            ShortInlineBrTarget or
                ShortInlineI or
                ShortInlineVar => 1,
            _ => 0
        };

}
