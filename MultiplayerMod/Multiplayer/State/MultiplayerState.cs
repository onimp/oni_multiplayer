using UnityEngine;

namespace MultiplayerMod.Multiplayer.State;

public static class MultiplayerState {

    private static readonly Core.Logging.Logger log = new(typeof(MultiplayerState));

    private static MultiplayerRole role = MultiplayerRole.None;

    public static MultiplayerRole Role {
        get => role;
        set {
            role = value;
            log.Trace(() => $"Setting role to {role}");
        }
    }

    public static GameObject GameObject { get; set; }

    public static bool WorldSpawned { get; set; }

    public static MultiplayerSharedState Shared { get; set; } = new();

    public static void Reset() {
        Role = MultiplayerRole.None;
        WorldSpawned = false;
    }

}
