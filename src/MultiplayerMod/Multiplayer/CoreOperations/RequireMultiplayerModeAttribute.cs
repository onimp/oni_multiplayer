using System;
using JetBrains.Annotations;
using MultiplayerMod.AttributeProcessor;
using MultiplayerMod.ModRuntime.StaticCompatibility;

namespace MultiplayerMod.Multiplayer.CoreOperations;

[AttributeUsage(AttributeTargets.Method)]
[ConditionalInvocation(typeof(RequireMultiplayerModeAttribute), nameof(CheckMultiplayerGameMode))]
public class RequireMultiplayerModeAttribute : Attribute {

    [UsedImplicitly]
    public MultiplayerMode Mode { get; }

    public RequireMultiplayerModeAttribute(MultiplayerMode mode) {
        Mode = mode;
    }

    private static bool CheckMultiplayerGameMode(MultiplayerMode mode) =>
        Dependencies.Get<MultiplayerGame>().Mode == mode;

}
