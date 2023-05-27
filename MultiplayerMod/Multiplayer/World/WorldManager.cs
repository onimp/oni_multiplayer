using System.IO;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Patch;
using MultiplayerMod.Multiplayer.Commands;
using MultiplayerMod.Multiplayer.Commands.Speed;
using MultiplayerMod.Multiplayer.State;
using MultiplayerMod.Network;

namespace MultiplayerMod.Multiplayer.World;

public static class WorldManager {

    private static readonly IMultiplayerServer server = Container.Get<IMultiplayerServer>();

    public static byte[] GetWorldSave() {
        var path = SaveLoader.GetActiveSaveFilePath();
        PatchControl.RunWithDisabledPatches(() => SaveLoader.Instance.Save(path));
        return File.ReadAllBytes(path);
    }

    public static void Sync() {
        server.Send(new PauseGame());
        // TODO: Improvement: overlay should be shown on server as well. But it must be hidden as soon as all
        // connected clients are ready.
        server.Send(new ShowOverlay(), MultiplayerCommandOptions.SkipHost);
        server.Send(new LoadWorld(GetWorldSave()), MultiplayerCommandOptions.SkipHost);
    }

    public static void LoadWorldSave(byte[] data) {
        MultiplayerState.WorldSpawned = false;
        var path = Path.GetTempFileName();
        using (var writer = new BinaryWriter(File.OpenWrite(path)))
            writer.Write(data);
        LoadScreen.DoLoad(path);
    }

}
