using System;
using System.Collections.Generic;
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

    public event EventHandler<ServerStateChangedEventArgs> StateChanged;
    public event EventHandler<PlayerConnectedEventArgs> PlayerConnected;
    public event EventHandler<CommandReceivedEventArgs> CommandReceived;

    protected void OnPlayerConnected(PlayerConnectedEventArgs args) {
        PlayerConnected?.Invoke(this, args);
    }

    protected void OnCommandReceived(CommandReceivedEventArgs args) {
        CommandReceived?.Invoke(this, args);
    }

    private GameObject gameObject;

    protected abstract void doStart();

    public void Start() {
        SetState(MultiplayerServerState.Preparing);
        doStart();
        gameObject = UnityObject.CreateStaticWithComponent<ServerComponent>();
    }

    public void Stop() {
        if (State <= MultiplayerServerState.Stopped)
            throw new NetworkPlatformException("Server isn't started");

        UnityObject.Destroy(gameObject);
        Reset();
        SetState(MultiplayerServerState.Stopped);
    }

    public abstract void Tick();

    public abstract void Send(IPlayer player, IMultiplayerCommand command);

    public abstract void Send(IMultiplayerCommand command, MultiplayerCommandOptions options);

    protected void SetState(MultiplayerServerState state) {
        State = state;
        StateChanged?.Invoke(this, new ServerStateChangedEventArgs(state));
    }

    protected abstract void doReset();

    protected void Reset() {
        doReset();
    }

}
