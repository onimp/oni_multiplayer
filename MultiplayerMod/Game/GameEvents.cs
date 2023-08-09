using HarmonyLib;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Scheduling;

namespace MultiplayerMod.Game;

[HarmonyPatch(typeof(global::Game))]
public static class GameEvents {

    public static System.Action? GameStarted;

    // ReSharper disable once UnusedMember.Local
    [HarmonyPostfix]
    [HarmonyPatch(nameof(global::Game.SetGameStarted))]
    private static void SetGameStarted() => Container.Get<UnityTaskScheduler>().Run(() => {
        GameStarted?.Invoke();
    });

}
