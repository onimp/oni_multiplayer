using System;
using MultiplayerMod.Multiplayer.Debug;

namespace MultiplayerMod.Multiplayer.Commands.Debug;

[Serializable]
public class SyncWorldDebugSnapshot : IMultiplayerCommand {

    private WorldDebugSnapshot snapshot;

    public SyncWorldDebugSnapshot(WorldDebugSnapshot snapshot) {
        this.snapshot = snapshot;
    }

    public void Execute() {
        WorldDebugSnapshotRunner.LastServerInfo = snapshot;
    }

}
