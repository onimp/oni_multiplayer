using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace MultiplayerMod.Game.World;

public static class WorldIdentity {

    public static object? ActiveWorld => ClusterManager.Instance?.activeWorld;

    public static int? ActiveWorldId => GetWorldId(ActiveWorld);

    public static object? GetWorld(int? worldId) {
        if (!worldId.HasValue)
            return ActiveWorld;

        return FindWorld(worldId.Value) ?? ActiveWorld;
    }

    public static object? FindWorld(int worldId) {
        var clusterManager = ClusterManager.Instance;
        var worlds = GetMemberValue(clusterManager, "WorldContainers") as IEnumerable
                     ?? GetMemberValue(clusterManager, "worldContainers") as IEnumerable;
        if (worlds == null)
            return null;

        foreach (var world in worlds) {
            if (GetWorldId(world) == worldId)
                return world;
        }

        return null;
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

    public static int? GetCellWorldId(int cell) {
        const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
        var gridType = typeof(Grid);
        var value = gridType.GetField("WorldIdx", flags)?.GetValue(null)
                    ?? gridType.GetProperty("WorldIdx", flags)?.GetValue(null);
        return value switch {
            int[] worldIdx when cell >= 0 && cell < worldIdx.Length => worldIdx[cell],
            _ => null
        };
    }

    public static int? GetObjectWorldId(GameObject? gameObject) {
        if (gameObject == null)
            return null;

        try {
            return GetCellWorldId(Grid.PosToCell(gameObject));
        } catch {
            return null;
        }
    }

    public static int? GetCellsWorldId(IEnumerable<int> cells) {
        int? result = null;
        foreach (var worldId in cells.Select(GetCellWorldId).Where(it => it.HasValue).Distinct()) {
            if (result.HasValue && result != worldId)
                return null;
            result = worldId;
        }
        return result ?? ActiveWorldId;
    }

    public static int? GetCursorWorldId(Vector3 cursorPos) {
        try {
            return GetCellWorldId(Grid.PosToCell(cursorPos)) ?? ActiveWorldId;
        } catch {
            return ActiveWorldId;
        }
    }

    public static bool TryActivateWorld(int worldId) {
        if (ActiveWorldId == worldId)
            return true;

        var world = FindWorld(worldId);
        if (world == null)
            return false;

        var clusterManager = ClusterManager.Instance;
        if (TryInvoke(clusterManager, "SetActiveWorld", world) ||
            TryInvoke(clusterManager, "SetActiveWorld", worldId) ||
            TryInvoke(clusterManager, "SetActiveWorld", world, false) ||
            TryInvoke(clusterManager, "SetActiveWorld", worldId, false) ||
            TryInvoke(clusterManager, "SwitchActiveWorld", world) ||
            TryInvoke(clusterManager, "SwitchActiveWorld", worldId) ||
            TryInvoke(clusterManager, "ActivateWorld", world) ||
            TryInvoke(clusterManager, "ActivateWorld", worldId) ||
            TrySetMember(clusterManager, "activeWorld", world))
            return ActiveWorldId == worldId;

        return false;
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

    private static bool TryInvoke(object? instance, string name, params object[] args) {
        if (instance == null)
            return false;

        const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        try {
            foreach (var method in instance.GetType().GetMethods(flags).Where(it => it.Name == name)) {
                var parameters = method.GetParameters();
                if (parameters.Length != args.Length)
                    continue;
                if (!parameters.Zip(args, (parameter, arg) => arg == null || parameter.ParameterType.IsInstanceOfType(arg)).All(it => it))
                    continue;

                method.Invoke(instance, args);
                return true;
            }
        } catch { }
        return false;
    }

    private static bool TrySetMember(object? instance, string name, object value) {
        if (instance == null)
            return false;

        const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        try {
            var type = instance.GetType();
            var property = type.GetProperty(name, flags);
            if (property is { CanWrite: true }) {
                property.SetValue(instance, value);
                return true;
            }
            var field = type.GetField(name, flags);
            if (field == null)
                return false;

            field.SetValue(instance, value);
            return true;
        } catch {
            return false;
        }
    }

}
