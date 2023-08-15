using System;
using System.Collections.Generic;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.Core.Unity;
using MultiplayerMod.Multiplayer;
using MultiplayerMod.Network;
using MultiplayerMod.Network.Events;
using MultiplayerMod.Platform.Base.Network.Components;
using UnityEngine;

namespace MultiplayerMod.Platform.Base.Network;

public abstract class BaseServer : IMultiplayerServer {

    public MultiplayerServerState State { protected set; get; } = MultiplayerServerState.Stopped;

    public IMultiplayerEndpoint Endpoint {
        get {
            if (State != MultiplayerServerState.Started)
                throw new NetworkPlatformException("Server isn't started");

            return getEndpoint();
        }
    }

    protected abstract IMultiplayerEndpoint getEndpoint();
    protected abstract List<IPlayer> getPlayers();

    public List<IPlayer> Players => getPlayers();

    public event Action<ServerStateChangedEventArgs>? StateChanged;
    public event Action<IPlayer>? PlayerConnected;
    public event Action<IPlayer>? PlayerDisconnected;
    public event Action<CommandReceivedEventArgs>? CommandReceived;

    private readonly Core.Logging.Logger log = LoggerFactory.GetLogger<BaseServer>();

    protected void OnPlayerConnected(IPlayer player) {
        PlayerConnected?.Invoke(player);
    }

    protected void OnPlayerDisconnected(IPlayer player) {
        PlayerDisconnected?.Invoke(player);
    }

    protected void OnCommandReceived(CommandReceivedEventArgs args) {
        CommandReceived?.Invoke(args);
    }

    private GameObject? gameObject;

    protected abstract void doStart();

    public void Start() {
        if (!SteamManager.Initialized)
            throw new NetworkPlatformException("Steam API is not initialized");

        log.Debug("Starting...");
        SetState(MultiplayerServerState.Preparing);
        try {
            doStart();
        } catch (Exception) {
            Reset();
            SetState(MultiplayerServerState.Error);
            throw;
        }
        gameObject = UnityObject.CreateStaticWithComponent<ServerComponent>();
    }

    public void Stop() {
        if (State <= MultiplayerServerState.Stopped)
            throw new NetworkPlatformException("Server isn't started");

        log.Debug("Stopping...");
        if (gameObject != null)
            UnityObject.Destroy(gameObject);
        Reset();
        SetState(MultiplayerServerState.Stopped);
    }

    public abstract void Tick();

    public abstract void Send(IPlayer player, IMultiplayerCommand command);

    public abstract void Send(IMultiplayerCommand command, MultiplayerCommandOptions options);

    protected void SetState(MultiplayerServerState state) {
        State = state;
        StateChanged?.Invoke(new ServerStateChangedEventArgs(state));
    }

    protected abstract void doReset();

    protected void Reset() {
        doReset();
    }

}
