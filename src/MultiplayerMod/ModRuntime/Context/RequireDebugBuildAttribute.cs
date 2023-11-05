using System;
using AttributeProcessor.Annotations;

namespace MultiplayerMod.ModRuntime.Context;

[AttributeUsage(AttributeTargets.Method)]
[ConditionalInvocation(typeof(RequireDebugBuildAttribute), nameof(DebugEnabled))]
public class RequireDebugBuildAttribute : Attribute {

    private static bool DebugEnabled() {
#if DEBUG
        return true;
#else
        return false;
#endif
    }

}
