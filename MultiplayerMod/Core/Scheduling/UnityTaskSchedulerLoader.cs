using HarmonyLib;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Loader;
using MultiplayerMod.Core.Unity;

namespace MultiplayerMod.Core.Scheduling;

public class UnityTaskSchedulerLoader : IModComponentLoader {

    private readonly Logging.Logger log = new(typeof(UnityTaskSchedulerLoader));

    public void OnLoad(Harmony harmony) {
        log.Debug("Creating task scheduler...");
        Container.Register<UnityTaskScheduler>();
        UnityObject.CreateStaticWithComponent<UnityTaskExecutor>();
    }

}
