using System;

namespace MultiplayerMod.Core.Patch.Capture;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class LocalVariableCaptureAttribute : Attribute {

    public Type DeclaringType { get; }
    public string MethodName { get; }
    public Type[]? ArgumentTypes { get; }

    public LocalVariableCaptureAttribute(Type declaringType, string methodName) {
        DeclaringType = declaringType;
        MethodName = methodName;
    }

    public LocalVariableCaptureAttribute(Type declaringType, string methodName, Type[] argumentTypes) {
        DeclaringType = declaringType;
        MethodName = methodName;
        ArgumentTypes = argumentTypes;
    }

}
