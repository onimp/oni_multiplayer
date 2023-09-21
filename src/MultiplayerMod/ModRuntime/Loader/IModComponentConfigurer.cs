using MultiplayerMod.Core.Dependency;

namespace MultiplayerMod.ModRuntime.Loader;

public interface IModComponentConfigurer {
    void Configure(DependencyContainerBuilder builder);
}
