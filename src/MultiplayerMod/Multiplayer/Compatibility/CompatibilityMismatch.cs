using System;

namespace MultiplayerMod.Multiplayer.Compatibility;

[Serializable]
public record CompatibilityMismatch(string Field, string Message);
