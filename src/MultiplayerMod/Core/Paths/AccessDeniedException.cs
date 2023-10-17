using System;

namespace MultiplayerMod.Core.Paths;

public class AccessDeniedException : Exception {
    public AccessDeniedException(string message) : base(message) { }
}
