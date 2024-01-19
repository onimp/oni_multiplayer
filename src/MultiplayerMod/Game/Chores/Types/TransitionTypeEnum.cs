namespace MultiplayerMod.Game.Chores.Types;

public enum TransitionTypeEnum {
    // names are aligned with methods in GameStateMachine`4.cs
    Enter,
    Exit,
    MoveTo,
    Update,
    EventHandler
}
