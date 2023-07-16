using System;
using MultiplayerMod.Game.Mechanics;

namespace MultiplayerMod.Multiplayer.Commands.Gameplay.Access;

[Serializable]
public class ChangeDefaultPermission : AbstractAccessControlCommand {

    public ChangeDefaultPermission(AccessControlEventArgs arguments) : base(arguments) { }

    protected override void Apply(AccessControl control) {
        if (Arguments.Permission != null)
            control.DefaultPermission = Arguments.Permission.Value;
    }

}
