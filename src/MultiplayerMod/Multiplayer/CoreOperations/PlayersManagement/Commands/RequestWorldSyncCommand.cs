using System;
using MultiplayerMod.Multiplayer.Commands;

namespace MultiplayerMod.Multiplayer.CoreOperations.PlayersManagement.Commands;

[Serializable]
public class RequestWorldSyncCommand : MultiplayerCommand {

    public override void Execute(MultiplayerCommandContext context) {
        context.EventDispatcher.Dispatch(new WorldSyncRequestedEvent());
    }

}
