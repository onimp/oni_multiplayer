using System.Collections.Generic;
using HarmonyLib;

namespace MultiplayerMod.Core.Patch;

public class CodeInstructionsOffsetAccessor {

    private readonly List<CodeInstruction> source;
    private readonly Dictionary<int, int> offsetIndex = new();

    public CodeInstructionsOffsetAccessor(List<CodeInstruction> source) {
        this.source = source;
        var offset = 0;
        var index = 0;
        foreach (var instruction in source) {
            offsetIndex[offset] = index++;
            offset += instruction.GetSize();
        }
    }

    public CodeInstruction GetInstruction(int offset) {
        return source[offsetIndex[offset]];
    }

    public List<CodeInstruction> GetInstructions(int from, int to) {
        var index = offsetIndex[from];
        var count = offsetIndex[to] - index + 1;
        return source.GetRange(index, count);
    }

}
