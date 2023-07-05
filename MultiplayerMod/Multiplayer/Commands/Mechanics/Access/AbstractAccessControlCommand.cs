using System;
using MultiplayerMod.Game.Mechanics;
using MultiplayerMod.Multiplayer.Tools;

namespace MultiplayerMod.Multiplayer.Commands.Mechanics.Access;

[Serializable]
public abstract class AbstractAccessControlCommand : IMultiplayerCommand {

    protected AccessControlEventArgs Arguments;

    protected AbstractAccessControlCommand(AccessControlEventArgs arguments) {
        Arguments = arguments;
    }

    public void Execute() {
        var control = Arguments.Target.GetComponent<AccessControl>();
        Apply(control);
        if (SideScreen<AccessControlSideScreen>.TargetSelected(control, out var screen))
            screen.Refresh(screen.identityList, false);
    }

    protected virtual void Apply(AccessControl control) { }

}
