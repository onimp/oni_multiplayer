using System;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.Multiplayer.Commands;
using MultiplayerMod.Multiplayer.Players;

namespace MultiplayerMod.Multiplayer.CoreOperations.PlayersManagement.Commands;

[Serializable]
[MultiplayerCommand(Type = MultiplayerCommandType.System)]
public class InitializeClientCommand : MultiplayerCommand {

    private static Core.Logging.Logger log = LoggerFactory.GetLogger<InitializeClientCommand>();

    private PlayerProfile profile;

    public InitializeClientCommand(PlayerProfile profile) {
        this.profile = profile;
    }

    public override void Execute(MultiplayerCommandContext context) {
        if (context.ClientId == null) {
            log.Error("Missing client id. Unable to initialize a player.");
            return;
        }
        context.EventDispatcher.Dispatch(new ClientInitializationRequestEvent(context.ClientId, profile));
    }

}
