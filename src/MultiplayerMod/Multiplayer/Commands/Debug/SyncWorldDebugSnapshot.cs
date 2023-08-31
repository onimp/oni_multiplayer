using System;
using MultiplayerMod.ModRuntime;
using MultiplayerMod.Multiplayer.World.Debug;

namespace MultiplayerMod.Multiplayer.Commands.Debug;

[Serializable]
public class SyncWorldDebugSnapshot : MultiplayerCommand {

    private WorldDebugSnapshot snapshot;

    public SyncWorldDebugSnapshot(WorldDebugSnapshot snapshot) {
        this.snapshot = snapshot;
    }

    public override void Execute(Runtime runtime) {
        WorldDebugSnapshotRunner.LastServerInfo = snapshot;
    }

}
