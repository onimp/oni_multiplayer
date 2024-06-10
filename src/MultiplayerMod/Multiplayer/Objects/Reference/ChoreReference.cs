using System;
using MultiplayerMod.ModRuntime.StaticCompatibility;
using MultiplayerMod.Multiplayer.Objects.Extensions;

namespace MultiplayerMod.Multiplayer.Objects.Reference;

[Serializable]
public class ChoreReference(Chore chore) : TypedReference<Chore> {

    private MultiplayerId id = chore.MultiplayerId();

    public override Chore Resolve() => Dependencies.Get<MultiplayerGame>().Objects.Get<Chore>(id);

}
