using System;
using MultiplayerMod.Multiplayer.World;

namespace MultiplayerMod.Multiplayer.Commands.World;

[Serializable]
[MultiplayerCommand(Type = MultiplayerCommandType.System)]
public class LoadWorld : MultiplayerCommand {

    private string worldName;
    private byte[] data;

    public LoadWorld(string worldName, byte[] data) {
        this.worldName = worldName;
        this.data = data;
    }

    public override void Execute(MultiplayerCommandContext context) => WorldManager.LoadWorldSave(worldName, data);

}
