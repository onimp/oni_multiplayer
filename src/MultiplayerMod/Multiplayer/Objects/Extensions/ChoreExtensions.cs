using MultiplayerMod.Core.Dependency;
using MultiplayerMod.ModRuntime;
using MultiplayerMod.Multiplayer.Objects.Reference;

namespace MultiplayerMod.Multiplayer.Objects.Extensions;

[DependenciesStaticTarget]
public static class ChoreExtensions {

    [InjectDependency]
    private static readonly MultiplayerObjects objects = null!;

    public static MultiplayerId Register(
        this Chore chore,
        MultiplayerId? multiplayerId = null,
        bool persistent = false
    ) => objects.Register(chore, multiplayerId, persistent).Id;

    public static MultiplayerId MultiplayerId(this Chore chore) => objects.Get(chore)!.Id;

    public static ChoreReference GetReference(this Chore chore) => new(chore);

}
