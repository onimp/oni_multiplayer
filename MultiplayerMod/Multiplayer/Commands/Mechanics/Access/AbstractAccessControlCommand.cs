using System;
using MultiplayerMod.Game.Mechanics;

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
        RefreshSideScreen(control);
    }

    protected virtual void Apply(AccessControl control) { }

    private void RefreshSideScreen(AccessControl control) {
        if (DetailsScreen.Instance.currentSideScreen is not AccessControlSideScreen screen)
            return;

        if (screen.target == control)
            screen.Refresh(screen.identityList, false);
    }

}
