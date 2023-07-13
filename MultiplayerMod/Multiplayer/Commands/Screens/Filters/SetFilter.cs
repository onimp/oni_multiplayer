using System;
using MultiplayerMod.Multiplayer.Objects;

namespace MultiplayerMod.Multiplayer.Commands.Screens.Filters;

[Serializable]
public class SetFilter : IMultiplayerCommand {
    private readonly MultiplayerReference reference;
    private readonly Tag tag;

    public SetFilter(MultiplayerReference reference, Tag tag) {
        this.reference = reference;
        this.tag = tag;
    }

    public void Execute() {
        var filterable = reference.GetComponent<Filterable>();
        if (filterable != null) {
            filterable.SelectedTag = tag;
        }
    }
}
