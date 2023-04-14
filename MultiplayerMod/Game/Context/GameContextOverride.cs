namespace MultiplayerMod.Game.Context;

// ReSharper disable Unity.IncorrectMonoBehaviourInstantiation
public class GameContextOverride : GameContext {

    public PriorityScreen PriorityScreen { get; set; }

    public GameContextOverride() {
        ToolMenu = new ToolMenu();
        PriorityScreen = new PriorityScreen();
        ToolMenu.priorityScreen = PriorityScreen;
    }

}
