using MultiplayerMod.ModRuntime.StaticCompatibility;

namespace MultiplayerMod.Multiplayer.Objects.Extensions;

public static class ChoreExtensions {

    public static MultiplayerId Register(this Chore chore, MultiplayerId? multiplayerId = null) =>
        Dependencies.Get<MultiplayerGame>().Objects.Register(chore, multiplayerId);

    public static MultiplayerId MultiplayerId(this Chore chore) => Dependencies.Get<MultiplayerGame>().Objects[chore]!;

}
