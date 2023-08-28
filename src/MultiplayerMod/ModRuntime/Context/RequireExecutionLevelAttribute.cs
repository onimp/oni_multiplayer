using System;
using MultiplayerMod.AttributeProcessor;
using MultiplayerMod.ModRuntime.StaticCompatibility;

namespace MultiplayerMod.ModRuntime.Context;

[AttributeUsage(AttributeTargets.Method)]
[ConditionalInvocation(typeof(RequireExecutionLevelAttribute), nameof(ExecutionLevelCheck))]
public class RequireExecutionLevelAttribute : Attribute {

    public ExecutionLevel Level { get; }

    public RequireExecutionLevelAttribute(ExecutionLevel level) {
        Level = level;
    }

    private static bool ExecutionLevelCheck(ExecutionLevel level) =>
        ExecutionLevelMatcher.Matches(Execution.Context.Level, level);

}
