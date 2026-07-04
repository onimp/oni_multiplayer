using System;
using MultiplayerMod.Multiplayer.Commands;
using MultiplayerMod.Multiplayer.World.Data;
using MultiplayerMod.Network;

namespace MultiplayerMod.Multiplayer.World.Commands;

// Multi-MB save transfer for the hard-sync — rides the lowest-priority Bulk lane so the reload payload
// no longer freezes gameplay on the client while it streams.
[Serializable]
[MultiplayerCommand(Type = MultiplayerCommandType.System, Lane = NetworkLane.Bulk)]
public class LoadWorld(WorldSave world) : MultiplayerCommand {

    public override void Execute(MultiplayerCommandContext context) {
        context.Runtime.Dependencies.Get<WorldManager>().RequestWorldLoad(world);
    }

}
