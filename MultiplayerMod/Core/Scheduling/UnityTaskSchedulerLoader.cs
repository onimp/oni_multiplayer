using HarmonyLib;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Loader;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.Core.Unity;

namespace MultiplayerMod.Core.Scheduling;

// ReSharper disable once UnusedType.Global
public class UnityTaskSchedulerLoader : IModComponentLoader {

    private readonly Logging.Logger log = LoggerFactory.GetLogger<UnityTaskSchedulerLoader>();

    public void OnLoad(Harmony harmony) {
        log.Debug("Creating task scheduler...");
        Dependencies.Register<UnityTaskScheduler>();
        UnityObject.CreateStaticWithComponent<UnityTaskExecutor>();
    }

}
