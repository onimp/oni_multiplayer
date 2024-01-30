namespace MultiplayerMod.Game.Chores.States;

public record MoveToArgs(
    Chore Chore,
    int Cell,
    CellOffset[] Offsets
);
