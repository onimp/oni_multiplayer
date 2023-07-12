using System;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.Multiplayer.Objects;

namespace MultiplayerMod.Multiplayer.Configuration;

public class CommandExceptionHandler {

    private static readonly Core.Logging.Logger log = LoggerFactory.GetLogger<CommandExceptionHandler>();

    public void Handle(IMultiplayerCommand command, Exception exception) {
        switch (exception) {
            case MultiplayerObjectNotFoundException e:
                log.Warning($"Multiplayer object {e.Id} not found in command {command.GetType().FullName}");
                return;
            default:
                throw exception;
        }
    }

}
