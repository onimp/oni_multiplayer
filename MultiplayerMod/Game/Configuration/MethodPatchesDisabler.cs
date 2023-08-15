using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using MultiplayerMod.Core.Patch;
using MultiplayerMod.Core.Patch.Context;

namespace MultiplayerMod.Game.Configuration;

[HarmonyPatch]
// ReSharper disable once UnusedType.Global
public static class MethodPatchesDisabler {

    private static readonly PatchTargetResolver targets = new PatchTargetResolver.Builder()
        .AddMethods(typeof(MinionIdentity), nameof(MinionIdentity.OnSpawn))
        .AddMethods(typeof(MinionStartingStats), nameof(MinionStartingStats.Deliver))
        .AddMethods(typeof(SaveLoader), nameof(SaveLoader.InitialSave))
        .Build();

    // ReSharper disable once UnusedMember.Local
    [HarmonyTargetMethods]
    private static IEnumerable<MethodBase> TargetMethods() => targets.Resolve();

    // ReSharper disable once UnusedMember.Local
    [HarmonyPrefix]
    private static void BeforeMethod() => PatchContext.Enter(PatchControl.DisablePatches);

    // ReSharper disable once UnusedMember.Local
    [HarmonyPostfix]
    private static void AfterMethod() => PatchContext.Leave();

}
