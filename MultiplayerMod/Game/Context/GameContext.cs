namespace MultiplayerMod.Game.Context;

public class GameContext {

    public ToolMenu ToolMenu { get; set; }

    public static GameContext GetCurrent() => new() {
        ToolMenu = ToolMenu.Instance
    };

    public void Restore() {
        ToolMenu.Instance = ToolMenu;
    }

}
