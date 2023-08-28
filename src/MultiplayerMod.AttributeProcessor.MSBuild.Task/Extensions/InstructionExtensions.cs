using dnlib.DotNet.Emit;
using dnlib.DotNet.Pdb;

namespace MultiplayerMod.AttributeProcessor.MSBuild.Task.Extensions;

public static class InstructionExtensions {

    public static Instruction WithSequencePoint(this Instruction instruction, SequencePoint sequencePoint) {
        instruction.SequencePoint = sequencePoint;
        return instruction;
    }

}
