using System;

namespace MultiplayerMod.Multiplayer.Compatibility;

[Serializable]
public record ModFingerprint(string StaticId, string Title, string Version);
