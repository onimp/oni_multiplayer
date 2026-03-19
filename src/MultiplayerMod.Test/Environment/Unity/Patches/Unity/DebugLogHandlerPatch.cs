using System;
using HarmonyLib;
using JetBrains.Annotations;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.Test.GameRuntime.Patches;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MultiplayerMod.Test.Environment.Unity.Patches.Unity;

[UsedImplicitly]
[HarmonyPatch(typeof(DebugLogHandler))]
public class DebugLogHandlerPatch {

    private static readonly MultiplayerMod.Core.Logging.Logger log =
        LoggerFactory.GetLogger<DebugLogHandlerPatch>();

    [UsedImplicitly]
    [HarmonyPrefix]
    [HarmonyPatch(
        nameof(DebugLogHandler.LogFormat),
        typeof(LogType),
        typeof(Object),
        typeof(string),
        typeof(object[])
    )]
    private static bool DebugLogHandler_LogFormat(LogType logType, string format, object[] args) {
        var message = string.Format(format, args);
        switch (logType) {
            case LogType.Error:
                log.Error(message);
                // During MinionIdentity.OnSpawn, suppress errors from missing Personality/resources
                // in the test environment where Personalities DB is empty.
                if (MinionIdentityPatch.SuppressErrors)
                    break;
                throw new Exception(message);
            case LogType.Warning:
                log.Warning(message);
                break;
            default:
                log.Info(message);
                break;
        }
        return false;
    }
}
