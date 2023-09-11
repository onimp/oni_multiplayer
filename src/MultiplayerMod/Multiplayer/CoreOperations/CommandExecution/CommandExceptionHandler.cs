using System;
using System.Runtime.ExceptionServices;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.Multiplayer.Commands;
using MultiplayerMod.Multiplayer.Objects.Reference;

namespace MultiplayerMod.Multiplayer.CoreOperations.CommandExecution;

public class CommandExceptionHandler {

    private static readonly Core.Logging.Logger log = LoggerFactory.GetLogger<CommandExceptionHandler>();

    public void Handle(IMultiplayerCommand command, Exception exception) {
        switch (exception) {
            case ObjectNotFoundException e:
                log.Warning($"Multiplayer object {e.Reference} not found in command {command.GetType().FullName}");
                return;
            default:
                ExceptionDispatchInfo.Capture(exception).Throw();
                return;
        }
    }

}
