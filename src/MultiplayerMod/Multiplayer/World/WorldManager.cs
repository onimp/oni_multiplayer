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
using MultiplayerMod.Multiplayer.Commands.World;
using MultiplayerMod.Multiplayer.CoreOperations.PlayersManagement.Commands;
using MultiplayerMod.Multiplayer.Players;
using MultiplayerMod.Multiplayer.Players.Events;
using MultiplayerMod.Multiplayer.UI.Overlays;
using MultiplayerMod.Network;

namespace MultiplayerMod.Multiplayer.World;

[Dependency, UsedImplicitly]
public class WorldManager {

    private readonly IMultiplayerServer server;
    private readonly MultiplayerGame multiplayer;
    private readonly EventDispatcher events;
    private readonly UnityTaskScheduler scheduler;

    public WorldManager(
        IMultiplayerServer server,
        MultiplayerGame multiplayer,
        EventDispatcher events,
        UnityTaskScheduler scheduler
    ) {
        this.server = server;
        this.multiplayer = multiplayer;
        this.events = events;
        this.scheduler = scheduler;
    }

    public void Sync() {
        SetupStatusOverlay();

        var resume = !SpeedControlScreen.Instance.IsPaused;
        server.Send(new PauseGame());
        multiplayer.Objects.SynchronizeWithTracker();
        multiplayer.Players.ForEach(it => server.Send(new ChangePlayerStateCommand(it.Id, PlayerState.Loading)));
        server.Send(new ChangePlayerStateCommand(multiplayer.Players.Current.Id, PlayerState.Ready));
        server.Send(new LoadWorld(WorldName, GetWorldSave()), MultiplayerCommandOptions.SkipHost);

        if (resume)
            server.Send(new ResumeGame());
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

    public void RequestWorldLoad(string name, byte[] data) {
        MultiplayerStatusOverlay.Show($"Loading {name}...");
        events.Subscribe<PlayersReadyEvent>(
            (_, subscription) => {
                MultiplayerStatusOverlay.Close();
                subscription.Cancel();
            }
        );
        scheduler.Run(() => LoadWorldSave(name, data));
    }

    private void LoadWorldSave(string name, byte[] data) {
        var savePath = SaveLoader.GetCloudSavesDefault()
            ? SaveLoader.GetCloudSavePrefix()
            : SaveLoader.GetSavePrefixAndCreateFolder();

        var path = SecurePath.Combine(savePath, name, $"{name}.sav");
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        using (var writer = new BinaryWriter(File.OpenWrite(path)))
            writer.Write(data);

        LoadScreen.DoLoad(path);
    }

    private static string WorldName => Path.GetFileNameWithoutExtension(SaveLoader.GetActiveSaveFilePath());

    private static byte[] GetWorldSave() {
        var path = SaveLoader.GetActiveSaveFilePath();
        Execution.RunUsingLevel(ExecutionLevel.Multiplayer, () => SaveLoader.Instance.Save(path));
        return File.ReadAllBytes(path);
    }

}
