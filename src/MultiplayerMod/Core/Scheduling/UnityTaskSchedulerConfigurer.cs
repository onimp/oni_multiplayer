using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Unity;
using MultiplayerMod.ModRuntime.Loader;

namespace MultiplayerMod.Core.Scheduling;

[UsedImplicitly]
public class UnityTaskSchedulerConfigurer : IModComponentConfigurer {

    public void Configure(DependencyContainerBuilder builder) {
        UnityObject.CreateStaticWithComponent<UnityTaskExecutor>();
    }

}
