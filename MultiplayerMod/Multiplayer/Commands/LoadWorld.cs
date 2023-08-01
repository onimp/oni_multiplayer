using System;
using MultiplayerMod.Multiplayer.World;

namespace MultiplayerMod.Multiplayer.Commands;

[Serializable]
public class LoadWorld : MultiplayerCommand {

    private byte[] data;

    public LoadWorld(byte[] data) {
        this.data = data;
    }

    public override void Execute() => WorldManager.LoadWorldSave(data);

}
