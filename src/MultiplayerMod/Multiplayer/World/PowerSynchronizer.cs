using System.Collections.Generic;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Unity;
using MultiplayerMod.ModRuntime.Context;
using MultiplayerMod.Multiplayer.Commands.Gameplay;
using MultiplayerMod.Multiplayer.Objects.Extensions;
using MultiplayerMod.Network;

namespace MultiplayerMod.Multiplayer.World;

// Host-authoritative power replication. See SyncBatteryCharge for the model. Mirrors
// DuplicantStateSynchronizer: on a ~1 s cadence the host reads every live battery's stored joules and
// sends them in a single command on the reliable Gameplay lane. Colonies rarely have more than a few dozen
// batteries, so one small payload per tick is fine; gated Host-only + level-active + at least one client.
public class PowerSynchronizer : MultiplayerKMonoBehaviour, IRenderEveryTick {

    private const float sendPeriod = 1.0f;

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
            server.Send(new SyncBatteryCharge(entries));
    }

    private static SyncBatteryCharge.BatteryEntry[] BuildEntries() {
        var batteries = global::Components.Batteries.Items;
        var list = new List<SyncBatteryCharge.BatteryEntry>(batteries.Count);
        foreach (var battery in batteries) {
            if (battery == null)
                continue;
            list.Add(new SyncBatteryCharge.BatteryEntry {
                Reference = battery.GetReference<Battery>(),
                Joules = battery.joulesAvailable
            });
        }
        return list.ToArray();
    }

}
