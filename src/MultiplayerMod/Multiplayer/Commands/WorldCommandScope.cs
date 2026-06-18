using System;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.Game.World;
using MultiplayerMod.Multiplayer.Compatibility;

namespace MultiplayerMod.Multiplayer.Commands;

public static class WorldCommandScope {

    private static readonly Logger log = LoggerFactory.GetLogger(typeof(WorldCommandScope));

    public static void Execute(int? worldId, string actionName, Action action) {
        if (!Prepare(worldId, actionName))
            return;

        action();
    }

    public static bool Prepare(int? worldId, string actionName) {
        if (!worldId.HasValue)
            return true;

        var activeWorldId = WorldIdentity.ActiveWorldId;
        if (!activeWorldId.HasValue || activeWorldId == worldId)
            return true;

        if (WorldIdentity.TryActivateWorld(worldId.Value))
            return true;

        var message = $"Skipped cross-world multiplayer action '{actionName}' for world {worldId}; active world is {activeWorldId}.";
        log.Warning(message);
        DlcMultiplayerSafety.NotifyUnsupported(message);
        return false;
    }

}
