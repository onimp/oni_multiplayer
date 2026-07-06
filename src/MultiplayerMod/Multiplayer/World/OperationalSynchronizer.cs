using System.Collections.Generic;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Unity;
using MultiplayerMod.ModRuntime.Context;
using MultiplayerMod.Multiplayer.Commands.Gameplay;
using MultiplayerMod.Multiplayer.Objects.Extensions;
using MultiplayerMod.Network;

namespace MultiplayerMod.Multiplayer.World;

// Convergence safety net for host-authoritative building active state (the immediate path is the
// Operational.SetActive postfix in OperationalActiveSynchronizer). On a slow cadence the host re-streams
// every complete building's IsActive and the client stomps it. This covers the two cases the event-driven
// path can't: buildings that were already active before this client joined (whose state never toggles, so
// no SetActive fires) and the rare dropped command. One small bool-per-building payload every few seconds,
// gated Host-only + level-active + at least one client - the same model as PowerSynchronizer.
public class OperationalSynchronizer : MultiplayerKMonoBehaviour, IRenderEveryTick {

    private const float sendPeriod = 3.0f;

    [InjectDependency]
    private readonly IMultiplayerServer server = null!;

    [InjectDependency]
    private readonly MultiplayerGame multiplayer = null!;

    [InjectDependency]
    private readonly ExecutionLevelManager manager = null!;

    private float lastTime;

    public void RenderEveryTick(float dt) {
        if (!manager.LevelIsActive(ExecutionLevel.Multiplayer) || multiplayer.Mode != MultiplayerMode.Host)
            return;
        if (server.Clients.Count == 0)
            return;
        if (GameClock.Instance.GetTime() - lastTime < sendPeriod)
            return;

        lastTime = GameClock.Instance.GetTime();

        var entries = BuildEntries();
        if (entries.Length > 0)
            server.Send(new SyncBuildingActive(entries));
    }

    private static SyncBuildingActive.Entry[] BuildEntries() {
        var buildings = global::Components.BuildingCompletes.Items;
        var list = new List<SyncBuildingActive.Entry>(buildings.Count);
        foreach (var building in buildings) {
            if (building == null)
                continue;
            var operational = building.GetComponent<Operational>();
            if (operational == null)
                continue;
            list.Add(new SyncBuildingActive.Entry {
                Reference = operational.GetReference<Operational>(),
                Active = operational.IsActive
            });
        }
        return list.ToArray();
    }

}
