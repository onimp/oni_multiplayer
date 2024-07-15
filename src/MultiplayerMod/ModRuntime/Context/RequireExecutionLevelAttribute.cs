using System;
using AttributeProcessor.Annotations;
using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;

namespace MultiplayerMod.ModRuntime.Context;

[DependenciesStaticTarget]
[AttributeUsage(AttributeTargets.Method)]
[ConditionalInvocation(typeof(RequireExecutionLevelAttribute), nameof(CheckExecutionLevel))]
public class RequireExecutionLevelAttribute(ExecutionLevel level) : Attribute {

    [InjectDependency]
    private static readonly ExecutionLevelManager manager = null!;

    [UsedImplicitly]
    public ExecutionLevel Level { get; } = level;

    private static bool CheckExecutionLevel(ExecutionLevel level) => manager.LevelIsActive(level);

}
