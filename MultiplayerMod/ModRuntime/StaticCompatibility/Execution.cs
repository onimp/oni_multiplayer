using System.Runtime.CompilerServices;
using MultiplayerMod.ModRuntime.Context;

namespace MultiplayerMod.ModRuntime.StaticCompatibility;

public static class Execution {

    public static ExecutionContext Context => Runtime.Instance.ExecutionContext;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void EnterLevelSection(ExecutionLevel level) =>
        Dependencies.Get<ExecutionLevelManager>().EnterSection(level);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void LeaveLevelSection() =>
        Dependencies.Get<ExecutionLevelManager>().LeaveSection();

    public static void RunUsingLevel(ExecutionLevel level, System.Action action) =>
        Dependencies.Get<ExecutionLevelManager>().RunUsingLevel(level, action);

    public static void RunIfPossible(ExecutionLevel requiredLevel, System.Action action) {
        if (ExecutionLevelMatcher.Matches(Context.Level, requiredLevel))
            action();
    }

}
