using System;
using MultiplayerMod.AttributeProcessor;
using MultiplayerMod.ModRuntime.StaticCompatibility;

namespace MultiplayerMod.Multiplayer.CoreOperations;

[AttributeUsage(AttributeTargets.Method)]
[ConditionalInvocation(typeof(RequireHostModeAttribute), nameof(ExecutionLevelCheck))]
public class RequireHostModeAttribute : Attribute {

    private static bool ExecutionLevelCheck() => Dependencies.Get<MultiplayerGame>().Mode == MultiplayerMode.Host;

}
