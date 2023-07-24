using MultiplayerMod.Multiplayer.Objects;

namespace MultiplayerMod.Multiplayer.State;

public static class MultiplayerGame {

    public static IPlayer Player { get; set; } = null!;
    public static MultiplayerRole Role { get; set; }
    public static MultiplayerState State { get; set; } = new();
    public static MultiplayerObjects Objects { get; private set; } = new();

    public static PlayerState CurrentPlayerState {
        get {
            if (State.Players.TryGetValue(Player, out var state))
                return state;

            State.Players.Add(Player, new PlayerState(Player));
            return State.Players[Player];
        }
    }

    public static void Reset() {
        Player = null!;
        Role = MultiplayerRole.None;
        State = new MultiplayerState();
        Objects = new MultiplayerObjects();
    }

}
