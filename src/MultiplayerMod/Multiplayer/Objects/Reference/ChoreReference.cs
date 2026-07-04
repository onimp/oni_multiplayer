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

    // Throw ObjectNotFoundException (handled + skipped by CommandExceptionHandler) rather than handing
    // back null for a chore that was already cleaned up on the client, which would NRE downstream.
    public override Chore Resolve() => objects.Get<Chore>(id) ?? throw new ObjectNotFoundException(this);

}
