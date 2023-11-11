using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using MultiplayerMod.Core.Patch;
using MultiplayerMod.ModRuntime.Context;
using MultiplayerMod.ModRuntime.StaticCompatibility;

namespace MultiplayerMod.Game.Configuration;

[HarmonyPatch]
// ReSharper disable once UnusedType.Global
public static class MethodPatchesDisabler {

    private static readonly PatchTargetResolver targets = new PatchTargetResolver.Builder()
        .AddMethods(typeof(MinionIdentity), nameof(MinionIdentity.OnSpawn))
        .AddMethods(typeof(MinionStartingStats), nameof(MinionStartingStats.Deliver))
        .AddMethods(typeof(SaveLoader), nameof(SaveLoader.InitialSave))
        .AddMethods(typeof(MinionStorage), nameof(MinionStorage.CopyMinion))
        .Build();

    // ReSharper disable once UnusedMember.Local
    [HarmonyTargetMethods]
    private static IEnumerable<MethodBase> TargetMethods() => targets.Resolve();

    // ReSharper disable once UnusedMember.Local
    [HarmonyPrefix]
    [RequireExecutionLevel(ExecutionLevel.Multiplayer)]
    private static void BeforeMethod() => Execution.EnterLevelSection(ExecutionLevel.Component);

    // ReSharper disable once UnusedMember.Local
    [HarmonyPostfix]
    [RequireExecutionLevel(ExecutionLevel.Component)]
    private static void AfterMethod() => Execution.LeaveLevelSection();

}
