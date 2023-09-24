using System;
using MultiplayerMod.Exceptions;

namespace MultiplayerMod.Multiplayer.Commands.Registry;

public class MultiplayerCommandAlreadyRegisteredException : MultiplayerException {
    public MultiplayerCommandAlreadyRegisteredException(Type type) : base(
        $"Command \"{type}\" already registered"
    ) { }
}
