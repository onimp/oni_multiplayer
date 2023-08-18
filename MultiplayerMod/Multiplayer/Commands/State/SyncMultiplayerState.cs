using System;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Multiplayer.State;

namespace MultiplayerMod.Multiplayer.Commands.State;

[Serializable]
public class SyncMultiplayerState : MultiplayerCommand {

    private MultiplayerState state;

    public SyncMultiplayerState(MultiplayerState state) {
        this.state = state;
    }

    public override void Execute() {
        Dependencies.Get<MultiplayerGame>().State = Dependencies.Inject(state);
    }

}
