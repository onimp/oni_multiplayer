using System;
using MultiplayerMod.Multiplayer.State;

namespace MultiplayerMod.Multiplayer.Commands.State;

[Serializable]
public class SyncSharedState : IMultiplayerCommand {

    private MultiplayerSharedState state;

    public SyncSharedState(MultiplayerSharedState state) {
        this.state = state;
    }

    public void Execute() {
        MultiplayerState.Shared = state;
    }

}
