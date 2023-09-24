using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Logging;

namespace MultiplayerMod.Multiplayer.Commands.Registry;

[Dependency, UsedImplicitly]
public class MultiplayerCommandRegistry {

    private readonly Core.Logging.Logger log = LoggerFactory.GetLogger<MultiplayerCommandRegistry>();

    private readonly Dictionary<Type, MultiplayerCommandConfiguration> commands = new();

    public void Register<T>() where T : IMultiplayerCommand => Register(typeof(T));

    public void Register(Type type) {
        if (!typeof(IMultiplayerCommand).IsAssignableFrom(type))
            throw new MultiplayerCommandInvalidInterfaceException(type);

        if (commands.ContainsKey(type))
            throw new MultiplayerCommandAlreadyRegisteredException(type);

        var configuration = ExtractConfiguration(type);
        commands[type] = configuration;
        log.Debug(() => $"{configuration.Type} command {type} registered");
    }

    public MultiplayerCommandConfiguration GetCommandConfiguration(Type type) {
        if (!commands.TryGetValue(type, out var configuration))
            throw new MultiplayerCommandNotFoundException(type);

        return configuration;
    }

    private MultiplayerCommandConfiguration ExtractConfiguration(Type type) {
        var attribute = type.GetCustomAttribute<MultiplayerCommandAttribute>() ?? new MultiplayerCommandAttribute();
        return new MultiplayerCommandConfiguration(type, attribute.Type, attribute.ExecuteOnServer);
    }

}
