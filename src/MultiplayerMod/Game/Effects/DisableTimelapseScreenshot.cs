using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.ModRuntime.Context;
using MultiplayerMod.ModRuntime.StaticCompatibility;

namespace MultiplayerMod.Game.Effects;

// ONI's new-day timelapse renders every world across many frames, freezing the main thread — and with it
// the Steam network tick that rides on that thread — for tens of seconds. In multiplayer that stall lands
// right on top of the hard-sync, delaying the save transfer and reload (observed ~45s of dead air before
// a client even received the save). The screenshots are purely cosmetic, so skip them while a multiplayer
// session is active. Single-player is unaffected: the prefix self-gates on the execution level and only
// suppresses when the Multiplayer level is active.
[HarmonyPatch]
public static class DisableTimelapseScreenshot {

    private static readonly Core.Logging.Logger log = LoggerFactory.GetLogger(typeof(DisableTimelapseScreenshot));

    // Timelapser.SaveScreenshot is the NewDay handler that queues worldsToScreenshot and starts the render
    // coroutine (it prints the "Timelapse.OnNewDay but worldsToScreenshot is not empty" line). Resolve it by
    // name so a signature/overload change can't break mod load: if it can't be found we patch nothing and
    // the timelapse simply keeps running, rather than throwing at PatchAll time.
    [UsedImplicitly]
    private static IEnumerable<MethodBase> TargetMethods() {
        var method = AccessTools.Method(typeof(Timelapser), "SaveScreenshot");
        if (method == null) {
            log.Warning("Timelapser.SaveScreenshot not found; timelapse will not be suppressed during multiplayer");
            yield break;
        }
        yield return method;
    }

    // A bool prefix returns true to run the original (take the screenshot) and false to skip it. Suppress
    // only while a multiplayer session is live.
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool Prefix() => !MultiplayerSessionActive();

    private static bool MultiplayerSessionActive() {
        try {
            return Dependencies.Get<ExecutionLevelManager>().LevelIsActive(ExecutionLevel.Multiplayer);
        } catch (Exception) {
            // Container not built yet / no session -> default to letting the (cosmetic) timelapse run.
            return false;
        }
    }

}
