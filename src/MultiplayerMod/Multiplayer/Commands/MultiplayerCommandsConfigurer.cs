using System.Linq;
using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Extensions;
using MultiplayerMod.ModRuntime.Loader;
using MultiplayerMod.Multiplayer.Commands.Registry;

namespace MultiplayerMod.Multiplayer.Commands;

[UsedImplicitly]
public class MultiplayerCommandsConfigurer : IModComponentConfigurer {

    public void Configure(DependencyContainerBuilder builder) {
        builder.ContainerCreated += RegisterCommands;
    }

    private void RegisterCommands(DependencyContainer container) {
        var registry = container.Get<MultiplayerCommandRegistry>();
        GetType().Assembly.DefinedTypes
            .Where(it => typeof(IMultiplayerCommand).IsAssignableFrom(it))
            .Where(it => !it.IsAbstract)
            .ForEach(it => registry.Register(it));
    }

}
