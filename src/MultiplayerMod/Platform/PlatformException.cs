using System;
using MultiplayerMod.Exceptions;

namespace MultiplayerMod.Platform;

public class PlatformException : MultiplayerException {
    public PlatformException(string message) : base(message) { }
    public PlatformException(string message, Exception cause) : base(message, cause) { }
}
