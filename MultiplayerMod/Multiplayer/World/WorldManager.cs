using System.IO;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Extensions;
using MultiplayerMod.Core.Patch;
using MultiplayerMod.Multiplayer.Commands;
using MultiplayerMod.Multiplayer.Commands.Overlay;
using MultiplayerMod.Multiplayer.Commands.Speed;
using MultiplayerMod.Multiplayer.Commands.State;
using MultiplayerMod.Multiplayer.State;
using MultiplayerMod.Network;

namespace MultiplayerMod.Multiplayer.World;

public static class WorldManager {

    private static readonly IMultiplayerServer server = Dependencies.Get<IMultiplayerServer>();
    private static readonly MultiplayerGame multiplayer = Dependencies.Get<MultiplayerGame>();

    public static void Sync() {
        server.Send(new PauseGame());
        multiplayer.Objects.SynchronizeWithTracker();
        multiplayer.State.Players.Values.ForEach(playerState => playerState.WorldSpawned = false);
        multiplayer.State.Current.WorldSpawned = true;
        server.Send(new SyncMultiplayerState(multiplayer.State));
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
        PatchControl.RunWithDisabledPatches(() => SaveLoader.Instance.Save(path));
        return File.ReadAllBytes(path);
    }

}
