using System.Collections;
using System.Linq;
using System.Reflection;

namespace MultiplayerMod.Game.World;

public static class WorldIdentity {

    public static object? ActiveWorld => ClusterManager.Instance?.activeWorld;

    public static int? ActiveWorldId => GetWorldId(ActiveWorld);

    public static object? GetWorld(int? worldId) {
        if (!worldId.HasValue)
            return ActiveWorld;

        var clusterManager = ClusterManager.Instance;
        var worlds = GetMemberValue(clusterManager, "WorldContainers") as IEnumerable
                     ?? GetMemberValue(clusterManager, "worldContainers") as IEnumerable;
        if (worlds == null)
            return ActiveWorld;

        foreach (var world in worlds) {
            if (GetWorldId(world) == worldId)
                return world;
        }

        return ActiveWorld;
    }

    public static int? GetWorldId(object? world) {
        if (world == null)
            return null;

        return GetIntMember(world, "id")
               ?? GetIntMember(world, "worldID")
               ?? GetIntMember(world, "WorldId")
               ?? GetIntMember(world, "worldId");
    }

    public static bool? IsRedAlertToggledOn(object? world) {
        var alertManager = GetMemberValue(world, "AlertManager");
        return GetMemberValue(alertManager, "IsRedAlertToggledOn") as bool?;
    }

    public static void ToggleRedAlert(object? world, bool enabled) {
        var alertManager = GetMemberValue(world, "AlertManager");
        Invoke(alertManager, "ToggleRedAlert", enabled);
    }

    private static int? GetIntMember(object instance, string name) {
        var value = GetMemberValue(instance, name);
        return value switch {
            int intValue => intValue,
            uint uintValue => checked((int) uintValue),
            _ => null
        };
    }

    private static object? GetMemberValue(object? instance, string name) {
        if (instance == null)
            return null;

        const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        try {
            var type = instance.GetType();
            var property = type.GetProperty(name, flags);
            if (property != null)
                return property.GetValue(instance);
            var field = type.GetField(name, flags);
            if (field != null)
                return field.GetValue(instance);
            var method = type.GetMethod(name, flags, null, System.Type.EmptyTypes, null);
            return method != null ? method.Invoke(instance, System.Array.Empty<object>()) : null;
        } catch {
            return null;
        }
    }

    private static void Invoke(object? instance, string name, params object[] args) {
        if (instance == null)
            return;

        const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        try {
            instance.GetType()
                .GetMethod(name, flags, null, args.Select(it => it.GetType()).ToArray(), null)
                ?.Invoke(instance, args);
        } catch { }
    }

}
