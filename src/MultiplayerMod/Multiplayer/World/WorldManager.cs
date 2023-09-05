using System.IO;
using MultiplayerMod.Core.Extensions;
using MultiplayerMod.ModRuntime.Context;
using MultiplayerMod.ModRuntime.StaticCompatibility;
using MultiplayerMod.Multiplayer.Commands;
using MultiplayerMod.Multiplayer.Commands.Overlay;
using MultiplayerMod.Multiplayer.Commands.Speed;
using MultiplayerMod.Multiplayer.Players;
using MultiplayerMod.Multiplayer.Players.Commands;
using MultiplayerMod.Multiplayer.State;
using MultiplayerMod.Network;

namespace MultiplayerMod.Multiplayer.World;

public class WorldManager {

    private readonly IMultiplayerServer server;
    private readonly MultiplayerGame multiplayer;

    public WorldManager(IMultiplayerServer server, MultiplayerGame multiplayer) {
        this.server = server;
        this.multiplayer = multiplayer;
    }

    public void Sync() {
        server.Send(new PauseGame());
        multiplayer.Objects.SynchronizeWithTracker();
        multiplayer.Players.ForEach(it => server.Send(new ChangePlayerStateCommand(it.Id, PlayerState.Loading)));
        server.Send(new ChangePlayerStateCommand(multiplayer.Players.Current.Id, PlayerState.Ready));
        server.Send(new ShowLoadOverlay());
        server.Send(new LoadWorld(GetWorldSave()), MultiplayerCommandOptions.SkipHost);
    }

    public static void LoadWorldSave(byte[] data) {
        var path = Path.GetTempFileName();
        using (var writer = new BinaryWriter(File.OpenWrite(path))) {
            writer.Write(data);
        }
        LoadScreen.DoLoad(path);
    }

    private static byte[] GetWorldSave() {
        var path = SaveLoader.GetActiveSaveFilePath();
        Execution.RunUsingLevel(ExecutionLevel.Multiplayer, () => SaveLoader.Instance.Save(path));
        return File.ReadAllBytes(path);
    }

}
