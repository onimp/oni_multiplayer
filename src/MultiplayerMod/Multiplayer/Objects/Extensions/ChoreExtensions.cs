using MultiplayerMod.Core.Dependency;
using MultiplayerMod.ModRuntime;
using MultiplayerMod.Multiplayer.Objects.Reference;

namespace MultiplayerMod.Multiplayer.Objects.Extensions;

[DependenciesStaticTarget]
public static class ChoreExtensions {

    [InjectDependency]
    private static readonly MultiplayerGame multiplayer = null!;

    public static MultiplayerId Register(this Chore chore, MultiplayerId? multiplayerId = null) =>
        multiplayer.Objects.Register(chore, multiplayerId);

    public static MultiplayerId MultiplayerId(this Chore chore) => multiplayer.Objects[chore]!;

    public static ChoreReference GetReference(this Chore chore) => new(chore);

}
