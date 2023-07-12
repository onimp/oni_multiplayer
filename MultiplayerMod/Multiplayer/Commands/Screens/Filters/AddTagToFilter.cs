using System;
using MultiplayerMod.Multiplayer.Objects;

namespace MultiplayerMod.Multiplayer.Commands.Screens.Filters;

[Serializable]
public class AddTagToFilter : IMultiplayerCommand {
    private readonly MultiplayerReference reference;
    private readonly Tag tag;

    public AddTagToFilter(MultiplayerReference reference, Tag tag) {
        this.reference = reference;
        this.tag = tag;
    }

    public void Execute() {
        reference.GetComponent<TreeFilterable>()?.AddTagToFilter(tag);
    }
}
