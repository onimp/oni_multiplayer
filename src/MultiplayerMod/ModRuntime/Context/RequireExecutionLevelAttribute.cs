using System;
using AttributeProcessor.Annotations;
using JetBrains.Annotations;

namespace MultiplayerMod.ModRuntime.Context;

[AttributeUsage(AttributeTargets.Method)]
[ConditionalInvocation(typeof(RequireExecutionLevelAttribute), nameof(CheckExecutionLevel))]
public class RequireExecutionLevelAttribute : Attribute {

    [UsedImplicitly]
    public ExecutionLevel Level { get; }

    public RequireExecutionLevelAttribute(ExecutionLevel level) {
        Level = level;
    }

    private static bool CheckExecutionLevel(ExecutionLevel level) =>
        Runtime.Instance.Dependencies.Get<ExecutionLevelManager>().LevelIsActive(level);

}
