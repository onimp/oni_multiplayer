using System;
using MultiplayerMod.ModRuntime;
using MultiplayerMod.Multiplayer.Objects.Reference;

namespace MultiplayerMod.Multiplayer.Commands.Gameplay;

[Serializable]
public class ChangePriority : MultiplayerCommand {

    private ComponentReference<Prioritizable> target;
    private PrioritySetting priority;

    public ChangePriority(ComponentReference<Prioritizable> target, PrioritySetting priority) {
        this.target = target;
        this.priority = priority;
    }

    public override void Execute(Runtime runtime) => target.GetComponent().SetMasterPriority(priority);

}
