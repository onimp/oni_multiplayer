using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Multiplayer.Objects;

namespace MultiplayerMod.Multiplayer.State;

public static class MultiplayerGame {

    public static MultiplayerRole Role { get; set; } = MultiplayerRole.None;
    public static MultiplayerState State { get; set; } = null!;
    public static MultiplayerObjects Objects { get; private set; } = null!;

    public static void Reset() {
        Role = MultiplayerRole.None;
        State = new MultiplayerState();
        Objects = Dependencies.Resolve<MultiplayerObjects>();
    }

}
