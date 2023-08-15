using System;

namespace MultiplayerMod.Core.Patch.Capture;

[AttributeUsage(AttributeTargets.Property)]
public class LocalVariableAttribute : Attribute {

    public int Index { get; }

    public LocalVariableAttribute(int index) {
        Index = index;
    }

}
