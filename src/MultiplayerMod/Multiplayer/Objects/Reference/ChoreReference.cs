using System;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.ModRuntime;
using MultiplayerMod.Multiplayer.Objects.Extensions;

namespace MultiplayerMod.Multiplayer.Objects.Reference;

[Serializable]
[DependenciesStaticTarget]
public class ChoreReference(Chore chore) : TypedReference<Chore> {

    [InjectDependency]
    private static MultiplayerObjects objects = null!;

    private MultiplayerId id = chore.MultiplayerId();

    public override Chore Resolve() => objects.Get<Chore>(id)!;

}
