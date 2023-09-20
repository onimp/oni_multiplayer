using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.ModRuntime;
using MultiplayerMod.ModRuntime.Context;
using MultiplayerMod.Multiplayer.Commands;
using MultiplayerMod.Network;

namespace MultiplayerMod.Multiplayer.CoreOperations.CommandExecution;

[Dependency, UsedImplicitly]
public class MultiplayerCommandExecutor {

    private readonly Core.Logging.Logger log = LoggerFactory.GetLogger<MultiplayerCommandExecutor>();

    private readonly Runtime runtime;
    private readonly ExecutionLevelManager executionLevelManager;
    private readonly CommandExceptionHandler exceptionHandler = new();

    private readonly Dictionary<Type, Configuration> configurationCache = new();

    public MultiplayerCommandExecutor(Runtime runtime, ExecutionLevelManager executionLevelManager) {
        this.runtime = runtime;
        this.executionLevelManager = executionLevelManager;
    }

    public void Execute(IMultiplayerClientId? clientId, IMultiplayerCommand command) {
        var configuration = GetCommandConfiguration(command);
        switch (configuration.Type) {
            case MultiplayerCommandType.System:
                RunCatching(clientId, command);
                break;
            case MultiplayerCommandType.Game:
               executionLevelManager.RunIfLevelIsActive(
                    ExecutionLevel.Game,
                    ExecutionLevel.Command,
                    () => RunCatching(clientId, command)
                );
                break;
            default:
                throw new CommandConfigurationException($"{command} has invalid type \"{configuration.Type}\"");
        }
    }

    private void RunCatching(IMultiplayerClientId? clientId, IMultiplayerCommand command) {
        try {
            log.Trace(() => $"Executing {command}");
            command.Execute(new MultiplayerCommandContext(clientId, runtime));
        } catch (Exception exception) {
            exceptionHandler.Handle(command, exception);
        }
    }

    private Configuration GetCommandConfiguration(IMultiplayerCommand command) {
        var type = command.GetType();
        if (configurationCache.TryGetValue(type, out var configuration))
            return configuration;

        configuration = new Configuration(type.GetCustomAttribute<MultiplayerCommandAttribute>());
        configurationCache[type] = configuration;
        return configuration;
    }

    private class Configuration {

        public MultiplayerCommandType Type { get; set; }

        public Configuration(MultiplayerCommandAttribute? attribute) {
            Type = attribute?.Type ?? MultiplayerCommandType.Game;
        }

    }

}
