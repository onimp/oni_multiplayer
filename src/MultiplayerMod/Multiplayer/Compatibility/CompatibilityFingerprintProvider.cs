using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;

namespace MultiplayerMod.Multiplayer.Compatibility;

[Dependency, UsedImplicitly]
public class CompatibilityFingerprintProvider {

    private static readonly string[] KnownContentIds = {
        "VANILLA_ID",
        "EXPANSION1_ID",
        "DLC2_ID",
        "DLC3_ID",
        "DLC4_ID",
        "DLC5_ID",
        "DLC6_ID",
        "DLC7_ID",
        "DLC8_ID",
        "DLC9_ID"
    };

    public CompatibilityFingerprint Create() => new(
        CompatibilityFingerprint.CurrentProtocolVersion,
        GetModVersion(),
        GetGameBuild(),
        GetGameBranch(),
        GetLoadedDlcIds(),
        GetActiveSaveDlcIds(),
        GetActiveMods()
    );

    private static string GetModVersion() =>
        typeof(CompatibilityFingerprintProvider).Assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion ?? "";

    private static string GetGameBuild() =>
        TryGetString("BuildWatermark", "GetBuildString")
        ?? TryGetString("BuildWatermark", "build")
        ?? TryGetString("KleiVersion", "ChangeList")
        ?? "";

    private static string GetGameBranch() =>
        TryGetString("BuildWatermark", "GetBranch")
        ?? TryGetString("KleiVersion", "BuildBranch")
        ?? "";

    private static string[] GetLoadedDlcIds() {
        var dlcManager = FindType("DlcManager");
        if (dlcManager == null)
            return Array.Empty<string>();

        var activeIds = ReadStaticStringCollectionMember(dlcManager, "GetActiveDlcIds")
            ?? ReadStaticStringCollectionMember(dlcManager, "GetActiveDLCIds")
            ?? ReadStaticStringCollectionMember(dlcManager, "GetSubscribedDlcIds")
            ?? ReadStaticStringCollectionMember(dlcManager, "GetSubscribedDLCIds")
            ?? ReadStaticStringCollectionMember(dlcManager, "ActiveDlcIds")
            ?? ReadStaticStringCollectionMember(dlcManager, "ActiveDLCIds")
            ?? ReadStaticStringCollectionMember(dlcManager, "activeDlcIds")
            ?? ReadStaticStringCollectionMember(dlcManager, "activeDLCIds");
        if (activeIds != null)
            return activeIds;

        return KnownContentIds
            .Where(id => TryIsContentActive(dlcManager, id))
            .ToArray();
    }

    private static string[] GetActiveSaveDlcIds() {
        var saveGameType = FindType("SaveGame");
        var instance = GetStaticMemberValue(saveGameType, "Instance");
        var values = ReadStringCollectionMember(instance, "DlcIds")
            ?? ReadStringCollectionMember(instance, "DLCIds")
            ?? ReadStringCollectionMember(instance, "dlcIds")
            ?? ReadStringCollectionMember(instance, "activeDlcIds");

        return values ?? Array.Empty<string>();
    }

    private static ModFingerprint[] GetActiveMods() {
        var manager = GetStaticMemberValue(FindType("Global"), "Instance");
        manager = GetMemberValue(manager, "modManager") ?? GetMemberValue(manager, "ModManager") ?? manager;
        var mods = GetMemberValue(manager, "mods") as IEnumerable
                   ?? GetMemberValue(manager, "Mods") as IEnumerable;
        if (mods == null)
            return Array.Empty<ModFingerprint>();

        return mods.Cast<object>()
            .Where(IsModEnabled)
            .Select(ToFingerprint)
            .Where(it => !string.IsNullOrWhiteSpace(it.StaticId))
            .OrderBy(it => it.StaticId, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static bool TryIsContentActive(Type dlcManager, string id) {
        const BindingFlags flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
        foreach (var methodName in new[] { "IsContentActive", "IsContentSubscribed" }) {
            var method = dlcManager.GetMethods(flags)
                .FirstOrDefault(it =>
                    it.Name == methodName
                    && it.GetParameters().Length == 1
                    && it.GetParameters()[0].ParameterType == typeof(string)
                );
            if (method != null) {
                try {
                    return (bool) method.Invoke(null, new object[] { id });
                } catch (Exception) { }
            }
        }
        return false;
    }

    private static bool IsModEnabled(object mod) {
        var enabled = GetMemberValue(mod, "enabled")
                      ?? GetMemberValue(mod, "Enabled")
                      ?? GetMemberValue(mod, "IsEnabled");
        return enabled is not bool value || value;
    }

    private static ModFingerprint ToFingerprint(object mod) {
        var label = GetMemberValue(mod, "label") ?? GetMemberValue(mod, "Label");
        var staticId = GetMemberValue(label, "id")?.ToString()
                       ?? GetMemberValue(label, "staticID")?.ToString()
                       ?? GetMemberValue(label, "StaticID")?.ToString()
                       ?? GetMemberValue(mod, "staticID")?.ToString()
                       ?? "";
        var title = GetMemberValue(label, "title")?.ToString()
                    ?? GetMemberValue(label, "Title")?.ToString()
                    ?? GetMemberValue(mod, "title")?.ToString()
                    ?? staticId;
        var version = GetMemberValue(label, "version")?.ToString()
                      ?? GetMemberValue(label, "Version")?.ToString()
                      ?? GetMemberValue(mod, "version")?.ToString()
                      ?? "";
        return new ModFingerprint(staticId, title, version);
    }

    private static string? TryGetString(string typeName, string memberName) {
        var type = FindType(typeName);
        var value = GetStaticMemberValue(type, memberName);
        return value?.ToString();
    }

    private static Type? FindType(string typeName) {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
            var type = assembly.GetType(typeName);
            if (type != null)
                return type;
        }
        return Type.GetType(typeName);
    }

    private static object? GetStaticMemberValue(Type? type, string name) {
        if (type == null)
            return null;
        return GetMemberValue(type, null, name);
    }

    private static object? GetMemberValue(object? instance, string name) {
        if (instance == null)
            return null;
        return GetMemberValue(instance.GetType(), instance, name);
    }

    private static object? GetMemberValue(Type type, object? instance, string name) {
        var flags = BindingFlags.Public | BindingFlags.NonPublic
                    | (instance == null ? BindingFlags.Static : BindingFlags.Instance);
        try {
            var property = type.GetProperty(name, flags);
            if (property != null)
                return property.GetValue(instance);
            var field = type.GetField(name, flags);
            if (field != null)
                return field.GetValue(instance);
            var method = type.GetMethod(name, flags, null, Type.EmptyTypes, null);
            return method != null ? method.Invoke(instance, Array.Empty<object>()) : null;
        } catch (Exception) {
            return null;
        }
    }

    private static string[]? ReadStringCollectionMember(object? instance, string name) {
        var value = GetMemberValue(instance, name);
        if (value is string single)
            return new[] { single };
        if (value is not IEnumerable enumerable)
            return null;
        return enumerable.Cast<object?>()
            .Select(it => it?.ToString())
            .Where(it => !string.IsNullOrWhiteSpace(it))
            .Select(it => it!)
            .ToArray();
    }

    private static string[]? ReadStaticStringCollectionMember(Type type, string name) {
        var value = GetStaticMemberValue(type, name);
        if (value is string single)
            return new[] { single };
        if (value is not IEnumerable enumerable)
            return null;
        return enumerable.Cast<object?>()
            .Select(it => it?.ToString())
            .Where(it => !string.IsNullOrWhiteSpace(it))
            .Select(it => it!)
            .ToArray();
    }

}
