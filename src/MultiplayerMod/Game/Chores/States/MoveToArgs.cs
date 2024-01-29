namespace MultiplayerMod.Game.Chores.States;

public record MoveToArgs(
    Chore Chore,
    string TargetState,
    int Cell,
    CellOffset[] Offsets
);
