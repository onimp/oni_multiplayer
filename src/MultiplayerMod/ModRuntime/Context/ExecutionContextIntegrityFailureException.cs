using System;

namespace MultiplayerMod.ModRuntime.Context;

public class ExecutionContextIntegrityFailureException : Exception {
    public ExecutionContextIntegrityFailureException(string message) : base(message) { }
}
