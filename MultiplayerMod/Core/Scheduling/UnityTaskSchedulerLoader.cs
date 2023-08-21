using JetBrains.Annotations;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.Core.Unity;
using MultiplayerMod.ModRuntime;
using MultiplayerMod.ModRuntime.Loader;

namespace MultiplayerMod.Core.Scheduling;

[UsedImplicitly]
public class UnityTaskSchedulerLoader : IModComponentLoader {

    private readonly Logging.Logger log = LoggerFactory.GetLogger<UnityTaskSchedulerLoader>();

    public void Load(Runtime runtime) {
        log.Debug("Creating task scheduler...");
        runtime.Dependencies.Register<UnityTaskScheduler>();
        UnityObject.CreateStaticWithComponent<UnityTaskExecutor>();
    }

}
