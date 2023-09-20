using JetBrains.Annotations;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.Core.Unity;
using MultiplayerMod.ModRuntime;
using MultiplayerMod.ModRuntime.Loader;

namespace MultiplayerMod.Core.Scheduling;

[UsedImplicitly]
public class UnityTaskSchedulerInitializer : IModComponentInitializer {

    private readonly Logging.Logger log = LoggerFactory.GetLogger<UnityTaskSchedulerInitializer>();

    public void Initialize(Runtime runtime) {
        log.Debug("Creating task scheduler...");
        UnityObject.CreateStaticWithComponent<UnityTaskExecutor>();
    }

}
