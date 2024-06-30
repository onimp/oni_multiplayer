using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using KMod;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Events;
using MultiplayerMod.Core.Extensions;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.Core.Patch;

namespace MultiplayerMod.ModRuntime.Loader;

public class DelayedModLoader {

    private readonly Core.Logging.Logger log = LoggerFactory.GetLogger<DelayedModLoader>();

    private readonly Harmony harmony;
    private readonly Assembly modAssembly;
    private readonly IReadOnlyList<Mod> mods;

    public DelayedModLoader(Harmony harmony, Assembly modAssembly, IReadOnlyList<Mod> mods) {
        this.harmony = harmony;
        this.modAssembly = modAssembly;
        this.mods = mods;
    }

    public void OnLoad() {
        var builder = new DependencyContainerBuilder()
            .AddSingleton(harmony)
            .AddType<EventDispatcher>()
            .ScanAssembly(modAssembly);
        PrioritizedPatch();
        modAssembly.GetTypes()
            .Where(type => typeof(IModComponentConfigurer).IsAssignableFrom(type) && type.IsClass)
            .OrderBy(type => type.GetCustomAttribute<ModComponentOrder>()?.Order ?? ModComponentOrder.Default)
            .ForEach(
                type => {
                    var instance = (IModComponentConfigurer) Activator.CreateInstance(type);
                    log.Debug($"Configuring mod component with {type.FullName}");
                    instance.Configure(builder);
                }
            );
        var container = builder.Build();
        InjectStatic(modAssembly, container);
        OnRuntimeReady(container);
    }

    private void OnRuntimeReady(IDependencyContainer container) {
        container.Get<EventDispatcher>().Dispatch(new RuntimeReadyEvent(container.Get<Runtime>()));
        log.Info("Mod runtime is ready");
    }

    private void InjectStatic(Assembly assembly, IDependencyInjector container) => assembly.GetTypes()
        .Where(it => it.GetCustomAttribute<DependenciesStaticTargetAttribute>() != null)
        .ForEach(container.Inject);

    private void PrioritizedPatch() => AccessTools.GetTypesFromAssembly(modAssembly)
        .Where(it => it.GetCustomAttribute<HarmonyManualAttribute>() == null)
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
