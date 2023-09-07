using System;
using JetBrains.Annotations;
using MultiplayerMod.AttributeProcessor;

namespace MultiplayerMod.ModRuntime.Context;

[AttributeUsage(AttributeTargets.Method)]
[ConditionalInvocation(typeof(RequireExecutionLevelAttribute), nameof(ExecutionLevelCheck))]
public class RequireExecutionLevelAttribute : Attribute {

    [UsedImplicitly]
    public ExecutionLevel Level { get; }

    public RequireExecutionLevelAttribute(ExecutionLevel level) {
        Level = level;
    }

    private static bool ExecutionLevelCheck(ExecutionLevel level) =>
        Runtime.Instance.Dependencies.Get<ExecutionLevelManager>().LevelIsActive(level);

}
