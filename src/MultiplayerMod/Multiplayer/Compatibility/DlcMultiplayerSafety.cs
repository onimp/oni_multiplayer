using System;
using System.Linq;
using System.Reflection;

namespace MultiplayerMod.Multiplayer.Compatibility;

public static class DlcMultiplayerSafety {

    private static readonly string[] UnsafeObjectSyncTypes = {
        "PassengerRocketModule",
        "RocketControlStation",
        "CraftModuleInterface",
        "LaunchConditionManager",
        "StarmapScreen",
        "ClusterMapScreen"
    };

    public static bool ShouldBlockObjectSync(MethodBase method) {
        var declaringTypeName = method.DeclaringType?.Name ?? "";
        return UnsafeObjectSyncTypes.Any(typeName =>
            declaringTypeName.IndexOf(typeName, StringComparison.OrdinalIgnoreCase) >= 0
        );
    }

}
