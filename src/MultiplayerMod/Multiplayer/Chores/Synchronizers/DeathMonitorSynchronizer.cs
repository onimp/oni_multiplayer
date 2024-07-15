using HarmonyLib;
using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.ModRuntime;
using MultiplayerMod.ModRuntime.Context;
using MultiplayerMod.Multiplayer.Chores.Driver.Commands;
using MultiplayerMod.Multiplayer.StateMachines.Commands;
using MultiplayerMod.Network;

namespace MultiplayerMod.Multiplayer.Chores.Synchronizers;

[UsedImplicitly]
[DependenciesStaticTarget]
[HarmonyPatch(typeof(DeathMonitor.Instance))]
public class DeathMonitorSynchronizer {

    [InjectDependency]
    private static readonly IMultiplayerServer server = null!;

    [InjectDependency]
    private static readonly MultiplayerGame multiplayer = null!;

    [InjectDependency]
    private static readonly ExecutionLevelManager manager = null!;

    [HarmonyPrefix, UsedImplicitly]
    [HarmonyPatch(nameof(DeathMonitor.Instance.Kill))]
    private static bool KillPrefix(DeathMonitor.Instance __instance, Death death) {
        if (!manager.LevelIsActive(ExecutionLevel.Multiplayer))
            return true;

        if (multiplayer.Mode == MultiplayerMode.Client)
            return false;

        server.Send(new SetParameterValue(__instance, __instance.sm.death, death));
        server.Send(new ReleaseChoreDriver(__instance.controller.gameObject.GetComponent<ChoreDriver>()));
        return true;
    }

}
