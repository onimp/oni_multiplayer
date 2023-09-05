using MultiplayerMod.Core.Logging;
using MultiplayerMod.Game.Chores;
using MultiplayerMod.Game.UI.Screens.Events;
using MultiplayerMod.Game.World;
using MultiplayerMod.Multiplayer.Commands.Chores;
using MultiplayerMod.Multiplayer.Commands.Debug;
using MultiplayerMod.Multiplayer.State;
using MultiplayerMod.Multiplayer.World;
using MultiplayerMod.Multiplayer.World.Debug;
using MultiplayerMod.Network;

namespace MultiplayerMod.Multiplayer.Configuration;

// ReSharper disable once ClassNeverInstantiated.Global
public class ServerEventBindings {

    private readonly Core.Logging.Logger log = LoggerFactory.GetLogger<ServerEventBindings>();
    private readonly IMultiplayerServer server;
    private readonly MultiplayerGame multiplayer;
    private readonly WorldManager worldManager;
    private bool bound;

    public ServerEventBindings(IMultiplayerServer server, MultiplayerGame multiplayer, WorldManager worldManager) {
        this.server = server;
        this.multiplayer = multiplayer;
        this.worldManager = worldManager;
    }

    public void Bind() {
        if (bound)
            return;

        log.Debug("Binding server events");

        PauseScreenEvents.QuitGame += server.Stop;
        WorldDebugSnapshotRunner.SnapshotAvailable += snapshot => server.Send(new SyncWorldDebugSnapshot(snapshot));
        SaveLoaderEvents.WorldSaved += worldManager.Sync;

        ChoreConsumerEvents.FindNextChore += p => server.Send(new FindNextChore(p), MultiplayerCommandOptions.SkipHost);

        bound = true;
    }
}
