using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Events;
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

    private readonly IMultiplayerServer server;
    private readonly MultiplayerGame multiplayer;
    private readonly EventDispatcher events;
    private readonly UnityTaskScheduler scheduler;
    private readonly ExecutionLevelManager executionLevelManager;
    private readonly List<IWorldStateManager> worldStateManagers;

    public WorldManager(
        IMultiplayerServer server,
        MultiplayerGame multiplayer,
        EventDispatcher events,
        UnityTaskScheduler scheduler,
        ExecutionLevelManager executionLevelManager,
        List<IWorldStateManager> worldStateManagers
    ) {
        this.server = server;
        this.multiplayer = multiplayer;
        this.events = events;
        this.scheduler = scheduler;
        this.executionLevelManager = executionLevelManager;
        this.worldStateManagers = worldStateManagers;
    }

    public void Sync() {
        SetupStatusOverlay();

        var resume = !SpeedControlScreen.Instance.IsPaused;
        server.Send(new PauseGame());

        events.Dispatch(new WorldSyncEvent());

        multiplayer.Players.ForEach(it => server.Send(new ChangePlayerStateCommand(it.Id, PlayerState.Loading)));
        server.Send(new ChangePlayerStateCommand(multiplayer.Players.Current.Id, PlayerState.Ready));
        server.Send(new NotifyWorldSavePreparing(), MultiplayerCommandOptions.SkipHost);

        var world = new WorldSave(WorldName, GetWorldSave(), new WorldState());
        worldStateManagers.ForEach(it => it.SaveState(world.State));
        server.Send(new LoadWorld(world), MultiplayerCommandOptions.SkipHost);
        events.Subscribe<PlayersReadyEvent>(
            (_, subscription) => {
                if (resume)
                    server.Send(new ResumeGame());
                subscription.Cancel();
            }
        );
    }

    private void SetupStatusOverlay() {
        MultiplayerStatusOverlay.Show("Waiting for players...");
        events.Subscribe<PlayerStateChangedEvent>(
            (_, subscription) => {
                var players = multiplayer.Players;
                if (players.Ready) {
                    MultiplayerStatusOverlay.Close();
                    subscription.Cancel();
                }
                var readyPlayersCount = players.Count(it => it.State == PlayerState.Ready);
                var playerList = string.Join("\n", players.Select(it => $"{it.Profile.PlayerName}: {it.State}"));
                var statusText = $"Waiting for players ({readyPlayersCount}/{players.Count} ready)...\n{playerList}";
                MultiplayerStatusOverlay.Text = statusText;
            }
        );
    }

    public void RequestWorldLoad(WorldSave world) {
        MultiplayerStatusOverlay.Show($"Loading {world.Name}...");
        events.Subscribe<PlayersReadyEvent>(
            (_, subscription) => {
                MultiplayerStatusOverlay.Close();
                subscription.Cancel();
            }
        );
        events.Subscribe<WorldStateInitializingEvent>((_, subscription) => {
            worldStateManagers.ForEach(it => it.LoadState(world.State));
            subscription.Cancel();
        });
        scheduler.Run(() => LoadWorldSave(world.Name, world.Data));
    }

    private void LoadWorldSave(string name, byte[] data) {
        var savePath = SaveLoader.GetCloudSavesDefault()
            ? SaveLoader.GetCloudSavePrefix()
            : SaveLoader.GetSavePrefixAndCreateFolder();

        var path = SecurePath.Combine(savePath, name, $"{name}.sav");
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        using (var writer = new BinaryWriter(File.OpenWrite(path)))
            writer.Write(data);

        executionLevelManager.BaseLevel = ExecutionLevel.Multiplayer;
        LoadScreen.DoLoad(path);
    }

    private static string WorldName => Path.GetFileNameWithoutExtension(SaveLoader.GetActiveSaveFilePath());

    private static byte[] GetWorldSave() {
        var path = SaveLoader.GetActiveSaveFilePath();
        Execution.RunUsingLevel(ExecutionLevel.Multiplayer, () => SaveLoader.Instance.Save(path));
        return File.ReadAllBytes(path);
    }

}
