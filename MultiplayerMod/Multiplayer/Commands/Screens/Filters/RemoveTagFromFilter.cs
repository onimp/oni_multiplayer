using System;
using MultiplayerMod.Multiplayer.Objects;

namespace MultiplayerMod.Multiplayer.Commands.Screens.Filters;

[Serializable]
public class RemoveTagFromFilter : IMultiplayerCommand {
    private readonly MultiplayerReference reference;
    private readonly Tag tag;

    public RemoveTagFromFilter(MultiplayerReference reference, Tag tag) {
        this.reference = reference;
        this.tag = tag;
    }

    public void Execute() {
        reference.GetComponent<TreeFilterable>()?.RemoveTagFromFilter(tag);
    }

}
