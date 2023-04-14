using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using KMod;
using MultiplayerMod.Core.Extensions;
using MultiplayerMod.Core.Logging;

namespace MultiplayerMod.Core.Loader;

public class ModLoader : UserMod2 {

    private readonly Logging.Logger log = LoggerFactory.GetLogger<ModLoader>();

    public override void OnLoad(Harmony harmony) {
        base.OnLoad(harmony);
        assembly.GetTypes()
            .Where(type => typeof(IModComponentLoader).IsAssignableFrom(type) && type.IsClass)
            .OrderBy(type => type.GetCustomAttribute<ModComponentOrder>()?.Order ?? ModComponentOrder.Default)
            .ForEach(type => {
                var instance = (IModComponentLoader) Activator.CreateInstance(type);
                log.Debug($"Running mod component loader {type.FullName}");
                instance.OnLoad(harmony);
            });
    }

}
