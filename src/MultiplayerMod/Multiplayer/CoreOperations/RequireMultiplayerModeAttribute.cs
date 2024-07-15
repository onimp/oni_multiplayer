using System;
using AttributeProcessor.Annotations;
using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.ModRuntime;

namespace MultiplayerMod.Multiplayer.CoreOperations;

[DependenciesStaticTarget]
[AttributeUsage(AttributeTargets.Method)]
[ConditionalInvocation(typeof(RequireMultiplayerModeAttribute), nameof(CheckMultiplayerGameMode))]
public class RequireMultiplayerModeAttribute(MultiplayerMode mode) : Attribute {

    [InjectDependency]
    private static readonly MultiplayerGame multiplayer = null!;

    [UsedImplicitly]
    public MultiplayerMode Mode { get; } = mode;

    private static bool CheckMultiplayerGameMode(MultiplayerMode mode) => multiplayer.Mode == mode;

}
