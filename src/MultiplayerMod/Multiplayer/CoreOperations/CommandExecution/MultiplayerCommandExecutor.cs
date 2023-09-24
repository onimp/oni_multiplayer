using System;
using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.ModRuntime;
using MultiplayerMod.ModRuntime.Context;
using MultiplayerMod.Multiplayer.Commands;
using MultiplayerMod.Multiplayer.Commands.Registry;
using MultiplayerMod.Network;

namespace MultiplayerMod.Multiplayer.CoreOperations.CommandExecution;

[Dependency, UsedImplicitly]
public class MultiplayerCommandExecutor {

    private readonly Core.Logging.Logger log = LoggerFactory.GetLogger<MultiplayerCommandExecutor>();

    private readonly ExecutionLevelManager executionLevelManager;
    private readonly MultiplayerCommandRegistry registry;
    private readonly CommandExceptionHandler exceptionHandler = new();

    private readonly MultiplayerCommandRuntimeAccessor runtimeAccessor;

    public MultiplayerCommandExecutor(
        Runtime runtime,
        ExecutionLevelManager executionLevelManager,
        MultiplayerCommandRegistry registry
    ) {
        this.executionLevelManager = executionLevelManager;
        this.registry = registry;
        runtimeAccessor = new MultiplayerCommandRuntimeAccessor(runtime);
    }

    public void Execute(IMultiplayerClientId? clientId, IMultiplayerCommand command) {
        var configuration = registry.GetCommandConfiguration(command.GetType());
        switch (configuration.CommandType) {
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
                throw new CommandConfigurationException(
                    $"{command.GetType()} has unsupported type \"{configuration.Type}\""
                );
        }
    }

    private void RunCatching(IMultiplayerClientId? clientId, IMultiplayerCommand command) {
        try {
            log.Trace(() => $"Executing {command}");
            command.Execute(new MultiplayerCommandContext(clientId, runtimeAccessor));
        } catch (Exception exception) {
            exceptionHandler.Handle(command, exception);
        }
    }

}
