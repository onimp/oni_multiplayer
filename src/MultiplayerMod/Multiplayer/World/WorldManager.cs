using System.IO;
using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Extensions;
using MultiplayerMod.ModRuntime.Context;
using MultiplayerMod.ModRuntime.StaticCompatibility;
using MultiplayerMod.Multiplayer.Commands.Overlay;
using MultiplayerMod.Multiplayer.Commands.Speed;
using MultiplayerMod.Multiplayer.Commands.World;
using MultiplayerMod.Multiplayer.CoreOperations.PlayersManagement.Commands;
using MultiplayerMod.Multiplayer.Players;
using MultiplayerMod.Network;

namespace MultiplayerMod.Multiplayer.World;

[Dependency, UsedImplicitly]
public class WorldManager {

    private readonly IMultiplayerServer server;
    private readonly MultiplayerGame multiplayer;

    public WorldManager(IMultiplayerServer server, MultiplayerGame multiplayer) {
        this.server = server;
        this.multiplayer = multiplayer;
    }

    public void Sync() {
        var resume = !SpeedControlScreen.Instance.IsPaused;
        server.Send(new PauseGame());
        multiplayer.Objects.SynchronizeWithTracker();
        multiplayer.Players.ForEach(it => server.Send(new ChangePlayerStateCommand(it.Id, PlayerState.Loading)));
        server.Send(new ChangePlayerStateCommand(multiplayer.Players.Current.Id, PlayerState.Ready));
        server.Send(new ShowLoadOverlay());
        server.Send(new LoadWorld(WorldName, GetWorldSave()), MultiplayerCommandOptions.SkipHost);
        if (resume)
            server.Send(new ResumeGame());
    }

    public static void LoadWorldSave(string worldName, byte[] data) {
        var savePath = SaveLoader.GetCloudSavesDefault()
            ? SaveLoader.GetCloudSavePrefix()
            : SaveLoader.GetSavePrefixAndCreateFolder();

        var path = Path.Combine(savePath, worldName, $"{worldName}.sav");
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        using (var writer = new BinaryWriter(File.OpenWrite(path))) {
            writer.Write(data);
        }
        LoadScreen.DoLoad(path);
    }

    private static string WorldName => Path.GetFileNameWithoutExtension(SaveLoader.GetActiveSaveFilePath());

    private static byte[] GetWorldSave() {
        var path = SaveLoader.GetActiveSaveFilePath();
        Execution.RunUsingLevel(ExecutionLevel.Multiplayer, () => SaveLoader.Instance.Save(path));
        return File.ReadAllBytes(path);
    }

}
