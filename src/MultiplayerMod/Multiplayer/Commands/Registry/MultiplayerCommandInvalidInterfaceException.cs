using System;
using MultiplayerMod.Exceptions;

namespace MultiplayerMod.Multiplayer.Commands.Registry;

public class MultiplayerCommandInvalidInterfaceException : MultiplayerException {
    public MultiplayerCommandInvalidInterfaceException(Type type) : base(
        $"Type \"{type}\" doesn't implement \"{typeof(IMultiplayerCommand)}\" interface"
    ) { }
}
