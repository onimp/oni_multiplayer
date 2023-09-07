using System.Runtime.CompilerServices;
using MultiplayerMod.ModRuntime.Context;

namespace MultiplayerMod.ModRuntime.StaticCompatibility;

public static class Execution {

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void EnterLevelSection(ExecutionLevel level) =>
        Dependencies.Get<ExecutionLevelManager>().EnterOverrideSection(level);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void LeaveLevelSection() =>
        Dependencies.Get<ExecutionLevelManager>().LeaveOverrideSection();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RunUsingLevel(ExecutionLevel level, System.Action action) =>
        Dependencies.Get<ExecutionLevelManager>().RunUsingLevel(level, action);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RunIfLevelIsActive(ExecutionLevel requiredLevel, System.Action action) =>
        Dependencies.Get<ExecutionLevelManager>().RunIfLevelIsActive(requiredLevel, action);

}
