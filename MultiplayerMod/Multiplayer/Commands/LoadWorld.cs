using System;
using MultiplayerMod.Multiplayer.World;

namespace MultiplayerMod.Multiplayer.Commands;

[Serializable]
public class LoadWorld : IMultiplayerCommand {

    private byte[] data;

    public LoadWorld(byte[] data) {
        this.data = data;
    }

    public void Execute() => WorldManager.LoadWorldSave(data);

}
