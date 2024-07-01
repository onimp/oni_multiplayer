using System;
using MultiplayerMod.Multiplayer.Commands;
using MultiplayerMod.Multiplayer.World.Data;

namespace MultiplayerMod.Multiplayer.World.Commands;

[Serializable]
[MultiplayerCommand(Type = MultiplayerCommandType.System)]
public class LoadWorld(WorldSave world) : MultiplayerCommand {

    public override void Execute(MultiplayerCommandContext context) {
        context.Runtime.Dependencies.Get<WorldManager>().RequestWorldLoad(world);
    }

}
