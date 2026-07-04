using System;
using System.Runtime.ExceptionServices;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.Multiplayer.Commands;
using MultiplayerMod.Multiplayer.Objects;
using MultiplayerMod.Multiplayer.Objects.Reference;

namespace MultiplayerMod.Multiplayer.CoreOperations.CommandExecution;

public class CommandExceptionHandler {

    private static readonly Core.Logging.Logger log = LoggerFactory.GetLogger<CommandExceptionHandler>();

    public void Handle(IMultiplayerCommand command, Exception exception) {
        switch (exception) {
            case ObjectNotFoundException e:
                // Expected, already-handled condition: the command referenced an object that is not (yet)
                // replicated on this client - most often a chore whose target ghost has no shared identity
                // here (see ComponentReference.Resolve). This is the designed graceful-skip path, not an
                // error, so it must stay cheap: a host that keeps re-selecting one unresolvable chore would
                // otherwise storm the log with Warnings and dump the entire object table on every retry.
                // Log at Debug and skip the dump; keep both for genuinely unexpected failures below.
                log.Debug(() => $"Multiplayer object {e.Reference} not found in command {command.GetType().FullName}");
                return;
            default:
                MultiplayerObjectsDebugHelper.LogDump();
                ExceptionDispatchInfo.Capture(exception).Throw();
                return;
        }
    }

}
