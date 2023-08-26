using System.Runtime.CompilerServices;

namespace MultiplayerMod.ModRuntime.Context;

public static class ExecutionLevelMatcher {

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Matches(ExecutionLevel currentLevel, ExecutionLevel requiredLevel) =>
        currentLevel >= requiredLevel;

}
