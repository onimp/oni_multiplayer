﻿using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using KMod;
using MultiplayerMod.Core.Logging;

namespace MultiplayerMod.Core.Loader;

// ReSharper disable once UnusedType.Global
public class ModLoader : UserMod2 {

    private readonly Logging.Logger log = LoggerFactory.GetLogger<ModLoader>();

    public override void OnLoad(Harmony harmony) {
        var version = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
        log.Info($"Multiplayer mod version: {version}");
        harmony.CreateClassProcessor(typeof(LaunchInitializerPatch)).Patch();
    }

    public override void OnAllModsLoaded(Harmony harmony, IReadOnlyList<Mod> mods) {
        LaunchInitializerPatch.Loader = new DelayedModLoader(harmony, assembly, mods);
        log.Info("Delayed loader initialized");
    }

}
