using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using KMod;
using MultiplayerMod.Core.Extensions;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.Core.Patch;

namespace MultiplayerMod.ModRuntime.Loader;

public class DelayedModLoader {

    private readonly Core.Logging.Logger log = LoggerFactory.GetLogger<DelayedModLoader>();

    private readonly Harmony harmony;
    private readonly Assembly assembly;
    private readonly IReadOnlyList<Mod> mods;

    public DelayedModLoader(Harmony harmony, Assembly assembly, IReadOnlyList<Mod> mods) {
        this.harmony = harmony;
        this.assembly = assembly;
        this.mods = mods;
    }

    public void OnLoad() {
        var runtime = new Runtime();
        runtime.Dependencies.Register(harmony);
        PrioritizedPatch();
        assembly.GetTypes()
            .Where(type => typeof(IModComponentLoader).IsAssignableFrom(type) && type.IsClass)
            .OrderBy(type => type.GetCustomAttribute<ModComponentOrder>()?.Order ?? ModComponentOrder.Default)
            .ForEach(
                type => {
                    var instance = (IModComponentLoader) Activator.CreateInstance(type);
                    log.Debug($"Running mod component loader {type.FullName}");
                    instance.Load(runtime);
                }
            );
        log.Info("Mod runtime is ready");
    }

    private void PrioritizedPatch() => AccessTools.GetTypesFromAssembly(assembly)
        .Where(it => it != typeof(LaunchInitializerPatch))
        .Select(TryCreateClassProcessor)
        .NotNull()
        .Where(it => it.containerAttributes != null)
        .OrderByDescending(it => it.containerAttributes.priority)
        .ForEach(it => it.Patch());

    private PatchClassProcessor? TryCreateClassProcessor(Type type) {
        var optional = type.GetCustomAttribute<HarmonyOptionalAttribute>() != null;
        try {
            return harmony.CreateClassProcessor(type);
        } catch (Exception exception) {
            if (optional) {
                log.Trace(() => $"Unable to create class processor for patch {type.FullName}\n{exception}");
                log.Info($"Optional patch {type.FullName} is omitted");
            } else {
                throw;
            }
        }
        return null;
    }

}
