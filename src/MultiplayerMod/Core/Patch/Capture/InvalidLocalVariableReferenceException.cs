using System;

namespace MultiplayerMod.Core.Patch.Capture;

public class InvalidLocalVariableReferenceException : Exception {
    public InvalidLocalVariableReferenceException(string message) : base(message) { }
}
