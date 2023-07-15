﻿using System;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.Multiplayer.Objects.Reference;

namespace MultiplayerMod.Multiplayer.Configuration;

public class CommandExceptionHandler {

    private static readonly Core.Logging.Logger log = LoggerFactory.GetLogger<CommandExceptionHandler>();

    public void Handle(IMultiplayerCommand command, Exception exception) {
        switch (exception) {
            case ObjectNotFoundException e:
                log.Warning($"Multiplayer object {e.Reference} not found in command {command.GetType().FullName}");
                return;
            default:
                throw exception;
        }
    }

}
