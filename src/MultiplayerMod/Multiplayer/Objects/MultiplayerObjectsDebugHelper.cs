using System.Linq;
using System.Text.RegularExpressions;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Extensions;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.ModRuntime;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.Objects;

[DependenciesStaticTarget]
public class MultiplayerObjectsDebugHelper {

    private static readonly Core.Logging.Logger log = LoggerFactory.GetLogger(typeof(MultiplayerObjectsDebugHelper));

    [InjectDependency]
    private static readonly MultiplayerObjects objects = null!;

    public static void LogDump() {
        log.Debug(() => {
            var snapshot = objects.GetEnumerable().ToList();
            var lines = snapshot.Select(it => $"  {it.Value} [{it.Value.Generation}] -> {GetObjectName(it.Key)}");
            return $"Multiplayer objects dump:\n{string.Join("\n", lines)}";
        });
    }

    public static string GetObjectName(object? instance) => instance switch {
        GameObject gameObject => GetProperName(gameObject, gameObject),
        Component component => GetProperName(component, component.gameObject),
        Chore chore => GetChoreName(chore),
        _ => "N/A"
    };

    private static string GetChoreName(Chore chore) {
        var signature = chore.GetType().GetSignature();
        var targetName = "N/A";
        if (chore.target is Object target)
            targetName = GetProperName(target, chore.gameObject);
        return $"{signature} of {targetName}";
    }

    private static string GetProperName(Object instance, GameObject gameObject) {
        var name = gameObject.GetProperName();
        name = name != "" ? CleanupProperName(name) : gameObject.name;
        var signature = instance.GetType().GetSignature();
        var instanceId = gameObject.GetInstanceID();
        return $"{signature} (Name = {name}, Instance Id = 0x{instanceId:x8})";
    }

    private static string CleanupProperName(string value) =>
        Regex.Replace(value, "<.*?>", "", RegexOptions.Multiline | RegexOptions.IgnoreCase);

}
