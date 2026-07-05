using System;
using MultiplayerMod.Multiplayer.Commands;
using MultiplayerMod.Multiplayer.Players;

namespace MultiplayerMod.Multiplayer.CoreOperations.PlayersManagement.Commands;

// Broadcast to everyone to update a player's cosmetic hard-sync progress phase. Mirrors
// ChangePlayerStateCommand, but drives only the loading-status UI (see HardSyncStatusView) and never
// touches the PlayerState readiness gate.
[Serializable]
[MultiplayerCommand(Type = MultiplayerCommandType.System)]
public class SetLoadPhaseCommand : MultiplayerCommand {

    private PlayerIdentity playerId;
    private PlayerLoadPhase phase;

    public SetLoadPhaseCommand(PlayerIdentity playerId, PlayerLoadPhase phase) {
        this.playerId = playerId;
        this.phase = phase;
    }

    public override void Execute(MultiplayerCommandContext context) {
        context.Multiplayer.Players[playerId].LoadPhase = phase;
    }

}
