using System;
using MultiplayerMod.Game.Mechanics;

namespace MultiplayerMod.Multiplayer.Commands.Gameplay.Access;

[Serializable]
public class ChangePermission : AbstractAccessControlCommand {

    public ChangePermission(AccessControlEventArgs arguments) : base(arguments) { }

    protected override void Apply(AccessControl control) {
        if (Arguments.MinionProxy == null)
            return;
        var proxy = Arguments.MinionProxy.GetComponent<MinionAssignablesProxy>();
        if (Arguments.Permission != null)
            control.SetPermission(proxy, Arguments.Permission.Value);
        else
            control.ClearPermission(proxy);
    }

}
