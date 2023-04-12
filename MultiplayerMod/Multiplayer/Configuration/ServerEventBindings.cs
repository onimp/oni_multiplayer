﻿using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Game;
using MultiplayerMod.Game.Chores;
using MultiplayerMod.Multiplayer.Commands.Chore;
using MultiplayerMod.Multiplayer.Commands.Debug;
using MultiplayerMod.Multiplayer.Commands.State;
using MultiplayerMod.Multiplayer.Debug;
using MultiplayerMod.Multiplayer.State;
using MultiplayerMod.Multiplayer.World;
using MultiplayerMod.Network;

namespace MultiplayerMod.Multiplayer.Configuration;

public class ServerEventBindings
{

    private readonly Core.Logging.Logger log = new(typeof(ServerEventBindings));
    private readonly IMultiplayerServer server = Container.Get<IMultiplayerServer>();
    private bool bound;

    public void Bind()
    {
        if (bound)
            return;

        log.Debug("Binding server events");

        MultiplayerEvents.PlayerWorldSpawned += player => server.Send(
            player,
            new SyncSharedState(MultiplayerState.Shared)
        );
        WorldDebugSnapshotRunner.SnapshotAvailable += snapshot => server.Send(new SyncWorldDebugSnapshot(snapshot));
        SaveLoaderEvents.WorldSaved += WorldManager.Sync;

        ChoreConsumerEvents.FindNextChore += p => server.Send(new FindNextChore(p), MultiplayerCommandOptions.SkipHost);

        bound = true;
    }
}