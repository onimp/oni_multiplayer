using System;
using MultiplayerMod.Multiplayer.State;

namespace MultiplayerMod.Multiplayer.Commands.State;

[Serializable]
public class SyncMultiplayerState : IMultiplayerCommand {

    private MultiplayerState state;

    public SyncMultiplayerState(MultiplayerState state) {
        this.state = state;
    }

    public void Execute() {
        MultiplayerGame.State = state;
    }

}
