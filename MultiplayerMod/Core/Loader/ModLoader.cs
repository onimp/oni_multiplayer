using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using KMod;
using MultiplayerMod.Core.Extensions;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.Core.Patch;

namespace MultiplayerMod.Core.Loader;

// ReSharper disable once UnusedType.Global
public class ModLoader : UserMod2 {

    private readonly Logging.Logger log = LoggerFactory.GetLogger<ModLoader>();

    public override void OnLoad(Harmony harmony) {
        PrioritizedPatch(harmony);
        assembly.GetTypes()
            .Where(type => typeof(IModComponentLoader).IsAssignableFrom(type) && type.IsClass)
            .OrderBy(type => type.GetCustomAttribute<ModComponentOrder>()?.Order ?? ModComponentOrder.Default)
            .ForEach(
                type => {
                    var instance = (IModComponentLoader) Activator.CreateInstance(type);
                    log.Debug($"Running mod component loader {type.FullName}");
                    instance.OnLoad(harmony);
                }
            );
    }

    private void PrioritizedPatch(Harmony harmony) => AccessTools.GetTypesFromAssembly(assembly)
        .Select(it => TryCreateClassProcessor(harmony, it))
        .NotNull()
        .Where(it => it.containerAttributes != null)
        .OrderByDescending(it => it.containerAttributes.priority)
        .ForEach(it => it.Patch());

    private PatchClassProcessor? TryCreateClassProcessor(Harmony harmony, Type type) {
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
