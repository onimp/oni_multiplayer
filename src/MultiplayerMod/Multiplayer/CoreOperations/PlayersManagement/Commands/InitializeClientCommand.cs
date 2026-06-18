using System;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.Multiplayer.Commands;
using MultiplayerMod.Multiplayer.Compatibility;
using MultiplayerMod.Multiplayer.Players;

namespace MultiplayerMod.Multiplayer.CoreOperations.PlayersManagement.Commands;

[Serializable]
[MultiplayerCommand(Type = MultiplayerCommandType.System, ExecuteOnServer = true)]
public class InitializeClientCommand : MultiplayerCommand {

    private static Core.Logging.Logger log = LoggerFactory.GetLogger<InitializeClientCommand>();

    private PlayerProfile profile;
    private CompatibilityFingerprint compatibility;

    public InitializeClientCommand(PlayerProfile profile, CompatibilityFingerprint compatibility) {
        this.profile = profile;
        this.compatibility = compatibility;
    }

    public override void Execute(MultiplayerCommandContext context) {
        if (context.ClientId == null) {
            log.Error("Missing client id. Unable to initialize a player.");
            return;
        }
        context.EventDispatcher.Dispatch(new ClientInitializationRequestEvent(context.ClientId, profile, compatibility));
    }

}
