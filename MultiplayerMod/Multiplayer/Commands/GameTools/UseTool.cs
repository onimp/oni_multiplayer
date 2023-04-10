using System;

namespace MultiplayerMod.Multiplayer.Commands.GameTools;

[Serializable]
public class UseTool : IMultiplayerCommand {

    private GameToolType type;
    private object payload;

    public UseTool(GameToolType type, object payload) {
        this.type = type;
        this.payload = payload;
    }

    public void Execute() {
        GameToolFactory.CreateToolAction(type)(payload);
    }

}
