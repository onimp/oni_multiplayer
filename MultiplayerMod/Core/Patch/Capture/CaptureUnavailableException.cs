using System;

namespace MultiplayerMod.Core.Patch.Capture;

public class CaptureUnavailableException : Exception {
    public CaptureUnavailableException(string message) : base(message) { }
}
