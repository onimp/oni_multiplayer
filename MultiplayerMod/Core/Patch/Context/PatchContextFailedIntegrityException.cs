using System;

namespace MultiplayerMod.Core.Patch.Context;

public class PatchContextIntegrityFailureException : Exception {
    public PatchContextIntegrityFailureException(string message) : base(message) { }
}
