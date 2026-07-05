using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Events;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.Core.Extensions;
using MultiplayerMod.Core.Paths;
using MultiplayerMod.Core.Scheduling;
using MultiplayerMod.ModRuntime.Context;
using MultiplayerMod.ModRuntime.StaticCompatibility;
using MultiplayerMod.Multiplayer.Commands.Speed;
using MultiplayerMod.Multiplayer.CoreOperations.Events;
using MultiplayerMod.Multiplayer.CoreOperations.PlayersManagement.Commands;
using MultiplayerMod.Multiplayer.Players;
using MultiplayerMod.Multiplayer.Players.Events;
using MultiplayerMod.Multiplayer.UI.Overlays;
using MultiplayerMod.Multiplayer.World.Commands;
using MultiplayerMod.Multiplayer.World.Data;
using MultiplayerMod.Network;

namespace MultiplayerMod.Multiplayer.World;

[Dependency, UsedImplicitly]
public class WorldManager {

    private readonly Core.Logging.Logger log = LoggerFactory.GetLogger<WorldManager>();

    private readonly IMultiplayerServer server;
    private readonly IMultiplayerClient client;
    private readonly MultiplayerGame multiplayer;
    private readonly EventDispatcher events;
    private readonly UnityTaskScheduler scheduler;
    private readonly ExecutionLevelManager executionLevelManager;
    private readonly List<IWorldStateManager> worldStateManagers;

    public WorldManager(
        IMultiplayerServer server,
        IMultiplayerClient client,
        MultiplayerGame multiplayer,
        EventDispatcher events,
        UnityTaskScheduler scheduler,
        ExecutionLevelManager executionLevelManager,
        List<IWorldStateManager> worldStateManagers
    ) {
        this.server = server;
        this.client = client;
        this.multiplayer = multiplayer;
        this.events = events;
        this.scheduler = scheduler;
        this.executionLevelManager = executionLevelManager;
        this.worldStateManagers = worldStateManagers;
    }

    public void Sync() {
        MultiplayerStatusOverlay.Show(
            new HardSyncStatusView(
                multiplayer.Players, "Hard-syncing world", () => server.GetConnectionStats(), "Uplink"
            ).Render
        );

        var resume = !SpeedControlScreen.Instance.IsPaused;
        server.SendAll(new PauseGame());

        events.Dispatch(new WorldSyncEvent());

        var hostId = multiplayer.Players.Current.Id;
        multiplayer.Players.ForEach(it => server.SendAll(new ChangePlayerStateCommand(it.Id, PlayerState.Loading)));
        server.SendAll(new ChangePlayerStateCommand(hostId, PlayerState.Ready));

        // Seed the clients' progress so their lines show up immediately (they'll refine it as they load).
        // The host itself is already Ready, so the status view renders it as "Done" regardless of phase.
        multiplayer.Players
            .Where(it => !it.Id.Equals(hostId))
            .ForEach(it => server.SendAll(new SetLoadPhaseCommand(it.Id, PlayerLoadPhase.Transferring)));

        server.Send(new NotifyWorldSavePreparing());

        var saveWatch = Stopwatch.StartNew();
        var saveData = GetWorldSave();
        saveWatch.Stop();

        var stateWatch = Stopwatch.StartNew();
        var world = new WorldSave(WorldName, saveData, new WorldState());
        worldStateManagers.ForEach(it => it.SaveState(world.State));
        stateWatch.Stop();

        log.Info(
            $"Hard-sync host prepare: save={saveWatch.ElapsedMilliseconds}ms " +
            $"({saveData.Length / 1024}KiB), state={stateWatch.ElapsedMilliseconds}ms"
        );
        server.Send(new LoadWorld(world));
        // Sync() runs synchronously inside the new-day autosave (WorldSavedEvent); ONI then freezes the
        // main thread for ONI's timelapse screenshot. Flush now so the save reaches clients before that
        // freeze, instead of sitting buffered until the host's network tick resumes seconds later.
        server.Flush();
        events.Subscribe<PlayersReadyEvent>(
            (_, subscription) => {
                if (resume)
                    server.SendAll(new ResumeGame());
                MultiplayerStatusOverlay.Close();
                subscription.Cancel();
            }
        );
    }

    public void RequestWorldLoad(WorldSave world) {
        MultiplayerStatusOverlay.Show(
            new HardSyncStatusView(
                multiplayer.Players, $"Loading {world.Name}", () => client.GetConnectionStats(), "Host link"
            ).Render
        );
        var loadWatch = Stopwatch.StartNew();
        events.Subscribe<PlayersReadyEvent>(
            (_, subscription) => {
                loadWatch.Stop();
                log.Info($"Hard-sync client load complete: {loadWatch.ElapsedMilliseconds}ms (request -> ready)");
                MultiplayerStatusOverlay.Close();
                subscription.Cancel();
            }
        );
        events.Subscribe<WorldStateInitializingEvent>((_, subscription) => {
            ReportLocalPhase(PlayerLoadPhase.Reconciling);
            worldStateManagers.ForEach(it => it.LoadState(world.State));
            subscription.Cancel();
        });
        ReportLocalPhase(PlayerLoadPhase.Transferring);
        scheduler.Run(() => LoadWorldSave(world.Name, world.Data));
    }

    // A client reports its own hard-sync progress; the server rebroadcasts it so every screen updates.
    private void ReportLocalPhase(PlayerLoadPhase phase) =>
        client.Send(new RequestLoadPhaseCommand(multiplayer.Players.Current.Id, phase));

    private void LoadWorldSave(string name, byte[] data) {
        var savePath = SaveLoader.GetCloudSavesDefault()
            ? SaveLoader.GetCloudSavePrefix()
            : SaveLoader.GetSavePrefixAndCreateFolder();

        var path = SecurePath.Combine(savePath, name, $"{name}.sav");
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);

        var writeWatch = Stopwatch.StartNew();
        using (var writer = new BinaryWriter(File.OpenWrite(path)))
            writer.Write(data);
        writeWatch.Stop();

        log.Info($"Hard-sync client save write: {writeWatch.ElapsedMilliseconds}ms ({data.Length / 1024}KiB)");

        // Last phase report before the reload stall — DoLoad monopolises the main thread and the network
        // tick (SteamClient.Tick -> RunCallbacks) stops running until the reload completes. Flush right
        // after reporting so this "Loading" phase actually reaches the host before the freeze; otherwise
        // it sits un-flushed and every remote screen shows the previous phase for the whole ~30s reload.
        ReportLocalPhase(PlayerLoadPhase.Loading);
        client.Flush();

        executionLevelManager.BaseLevel = ExecutionLevel.Multiplayer;
        // DoLoad kicks off an async scene reload and returns before the game is ready; the total client
        // reload time is measured in RequestWorldLoad (request -> PlayersReadyEvent), not around this call.
        LoadScreen.DoLoad(path);
    }

    private static string WorldName => Path.GetFileNameWithoutExtension(SaveLoader.GetActiveSaveFilePath());

    private static byte[] GetWorldSave() {
        var path = SaveLoader.GetActiveSaveFilePath();
        Execution.RunUsingLevel(ExecutionLevel.Multiplayer, () => SaveLoader.Instance.Save(path));
        return File.ReadAllBytes(path);
    }

}
