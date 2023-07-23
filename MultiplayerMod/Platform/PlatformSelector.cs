namespace MultiplayerMod.Platform;

public static class PlatformSelector {
    public static bool IsSteamPlatform() {
        // Dev net overrides DistributionPlatform.
#if USE_DEV_NET
        return false;
#endif
        return DistributionPlatform.Inst.Platform == "Steam";
    }
}
