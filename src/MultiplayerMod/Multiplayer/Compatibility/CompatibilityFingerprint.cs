using System;

namespace MultiplayerMod.Multiplayer.Compatibility;

[Serializable]
public record CompatibilityFingerprint(
    int ProtocolVersion,
    string ModVersion,
    string GameBuild,
    string GameBranch,
    string[] LoadedDlcIds,
    string[] ActiveDlcIds,
    ModFingerprint[] ActiveMods
) {
    public const int CurrentProtocolVersion = 2;

    public static CompatibilityFingerprint Empty => new(
        CurrentProtocolVersion,
        "",
        "",
        "",
        Array.Empty<string>(),
        Array.Empty<string>(),
        Array.Empty<ModFingerprint>()
    );
}
