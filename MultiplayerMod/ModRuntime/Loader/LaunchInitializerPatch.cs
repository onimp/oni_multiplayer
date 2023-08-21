using HarmonyLib;

namespace MultiplayerMod.ModRuntime.Loader;

[HarmonyPatch(typeof(LaunchInitializer))]
public static class LaunchInitializerPatch {

    public static DelayedModLoader Loader { get; set; } = null!;

    // ReSharper disable once UnusedMember.Local
    [HarmonyPrefix]
    [HarmonyPatch(nameof(LaunchInitializer.DeleteLingeringFiles))]
    private static void LaunchInitializerBeforeDeleteLingeringFiles() {
        Loader.OnLoad();
    }

}
