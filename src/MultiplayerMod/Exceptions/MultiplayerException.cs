using System;

namespace MultiplayerMod.Exceptions;

public class MultiplayerException : Exception {
    protected MultiplayerException(string message) : base(message) { }
    protected MultiplayerException(string message, Exception cause) : base(message, cause) { }
}
