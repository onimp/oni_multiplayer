using HarmonyLib;
using MultiplayerMod.Core.Patch;
using MultiplayerMod.Core.Patch.Context;

namespace MultiplayerMod.Game.Mechanics.Minions;

[HarmonyPatch(typeof(MinionIdentity))]
// ReSharper disable once UnusedType.Global
public static class MinionIdentitySpawnPatch {

    // ReSharper disable once UnusedMember.Local
    [HarmonyPrefix]
    [HarmonyPatch(nameof(MinionIdentity.OnSpawn))]
    private static void BeforeSpawn() => PatchContext.Enter(PatchControl.DisablePatches);

    // ReSharper disable once UnusedMember.Local
    [HarmonyPostfix]
    [HarmonyPatch(nameof(MinionIdentity.OnSpawn))]
    private static void AfterSpawn() => PatchContext.Leave();

}
