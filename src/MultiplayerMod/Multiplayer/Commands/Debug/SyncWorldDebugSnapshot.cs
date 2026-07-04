using System;
using MultiplayerMod.Multiplayer.World.Debug;
using MultiplayerMod.Network;

namespace MultiplayerMod.Multiplayer.Commands.Debug;

// Large, periodic diagnostic payload — rides the lowest-priority Bulk lane so it never delays gameplay.
[Serializable]
[MultiplayerCommand(Lane = NetworkLane.Bulk)]
public class SyncWorldDebugSnapshot : MultiplayerCommand {

    private WorldDebugSnapshot snapshot;

    public SyncWorldDebugSnapshot(WorldDebugSnapshot snapshot) {
        this.snapshot = snapshot;
    }

    public override void Execute(MultiplayerCommandContext context) {
        WorldDebugSnapshotRunner.LastServerInfo = snapshot;
    }

}
