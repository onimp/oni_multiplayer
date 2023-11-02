using HarmonyLib;
using JetBrains.Annotations;
using MultiplayerMod.ModRuntime.Context;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.Objects.Support;

[UsedImplicitly]
[HarmonyPatch(typeof(MinionStorage))]
public static class MinionStorageRedirectionSupport {

    [HarmonyPostfix, UsedImplicitly]
    [HarmonyPatch(nameof(MinionStorage.RedirectInstanceTracker))]
    [RequireExecutionLevel(ExecutionLevel.Multiplayer)]
    private static void RedirectInstanceTracker(GameObject src_minion, GameObject dest_minion) {
        src_minion.GetComponent<MultiplayerInstance>().Redirect(dest_minion.GetComponent<MultiplayerInstance>());
    }

}
