﻿using HarmonyLib;
using MultiplayerMod.Core.Patch;

namespace MultiplayerMod.ModRuntime.Loader;

[HarmonyManual]
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
