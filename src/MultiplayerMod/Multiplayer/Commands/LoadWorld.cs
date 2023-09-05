using System;
using MultiplayerMod.Multiplayer.World;

namespace MultiplayerMod.Multiplayer.Commands;

[Serializable]
[MultiplayerCommand(Type = MultiplayerCommandType.System)]
public class LoadWorld : MultiplayerCommand {

    private byte[] data;

    public LoadWorld(byte[] data) {
        this.data = data;
    }

    public override void Execute(MultiplayerCommandContext context) => WorldManager.LoadWorldSave(data);

}
