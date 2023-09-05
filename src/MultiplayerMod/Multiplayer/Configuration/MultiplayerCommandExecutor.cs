using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
using MultiplayerMod.Exceptions;
using MultiplayerMod.ModRuntime;
using MultiplayerMod.ModRuntime.Context;
using MultiplayerMod.Network;

namespace MultiplayerMod.Multiplayer.Configuration;

[UsedImplicitly]
public class MultiplayerCommandExecutor {

    private readonly Runtime runtime;
    private readonly ExecutionLevelManager executionLevelManager;
    private readonly CommandExceptionHandler exceptionHandler = new();

    private readonly Dictionary<Type, Configuration> commandMetadata = new();

    public MultiplayerCommandExecutor(Runtime runtime, ExecutionLevelManager executionLevelManager) {
        this.runtime = runtime;
        this.executionLevelManager = executionLevelManager;
    }

    public void Execute(IMultiplayerClientId? clientId, IMultiplayerCommand command) {
        var configuration = GetCommandConfiguration(command);
        switch (configuration.Type) {
            case MultiplayerCommandType.System:
                command.Execute(new MultiplayerCommandContext(clientId, runtime));
                break;
            case MultiplayerCommandType.Gameplay:
                executionLevelManager.RunIfPossible(
                    ExecutionLevel.Gameplay,
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
            command.Execute(new MultiplayerCommandContext(clientId, runtime));
        } catch (Exception exception) {
            exceptionHandler.Handle(command, exception);
        }
    }

    private Configuration GetCommandConfiguration(IMultiplayerCommand command) {
        var type = command.GetType();
        if (commandMetadata.TryGetValue(type, out var metadata))
            return metadata;

        metadata = new Configuration(type.GetCustomAttribute<MultiplayerCommandAttribute>());
        commandMetadata[type] = metadata;
        return metadata;
    }

    private class Configuration {

        public MultiplayerCommandType Type { get; set; }

        public Configuration(MultiplayerCommandAttribute? attribute) {
            Type = attribute?.Type ?? MultiplayerCommandType.Gameplay;
        }

    }

}
