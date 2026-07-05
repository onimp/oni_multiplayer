using System;
using MultiplayerMod.Multiplayer.Commands;
using MultiplayerMod.Multiplayer.Players;
using MultiplayerMod.Network;

namespace MultiplayerMod.Multiplayer.CoreOperations.PlayersManagement.Commands;

// A client reports its own hard-sync progress; the server rebroadcasts it to everyone so all screens
// show a live per-player list. Mirrors RequestPlayerStateChangeCommand.
[Serializable]
[MultiplayerCommand(Type = MultiplayerCommandType.System, ExecuteOnServer = true)]
public class RequestLoadPhaseCommand : MultiplayerCommand {

    private PlayerIdentity playerId;
    private PlayerLoadPhase phase;

    public RequestLoadPhaseCommand(PlayerIdentity playerId, PlayerLoadPhase phase) {
        this.playerId = playerId;
        this.phase = phase;
    }

    public override void Execute(MultiplayerCommandContext context) {
        var server = context.Runtime.Dependencies.Get<IMultiplayerServer>();
        server.SendAll(new SetLoadPhaseCommand(playerId, phase));
    }

}
