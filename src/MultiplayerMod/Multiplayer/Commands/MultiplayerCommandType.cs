namespace MultiplayerMod.Multiplayer.Commands;

public enum MultiplayerCommandType {

    /// <summary>
    /// A command will be always executed.
    /// </summary>
    System,

    /// <summary>
    /// A command is related to the game and will be executed if possible.
    /// </summary>
    Game

}
