using System;
using MultiplayerMod.Game.Mechanics;

namespace MultiplayerMod.Multiplayer.Commands.Mechanics.Access;

[Serializable]
public class ChangeDefaultPermission : AbstractAccessControlCommand {

    public ChangeDefaultPermission(AccessControlEventArgs arguments) : base(arguments) { }

    protected override void Apply(AccessControl control) {
        // ReSharper disable once PossibleInvalidOperationException
        control.DefaultPermission = Arguments.Permission.Value;
    }

}
