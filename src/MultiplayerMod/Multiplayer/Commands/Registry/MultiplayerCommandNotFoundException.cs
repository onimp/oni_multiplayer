using System;
using MultiplayerMod.Exceptions;

namespace MultiplayerMod.Multiplayer.Commands.Registry;

public class MultiplayerCommandNotFoundException : MultiplayerException {
    public MultiplayerCommandNotFoundException(Type type) : base(
        $"Command \"{type}\" not found"
    ) { }
}
