using System;
using MultiplayerMod.Core.Unity;
using MultiplayerMod.Multiplayer;
using MultiplayerMod.Network;
using MultiplayerMod.Network.Events;
using UnityEngine;

namespace MultiplayerMod.Platform.Base.Network;

public abstract class BaseClient : IMultiplayerClient {

    public IPlayer Player => getPlayer().Value;
    protected abstract Lazy<IPlayer> getPlayer();
    public MultiplayerClientState State { get; private set; } = MultiplayerClientState.Disconnected;
    public event EventHandler<ClientStateChangedEventArgs> StateChanged;
    public event EventHandler<CommandReceivedEventArgs> CommandReceived;

    protected void OnCommandReceived(CommandReceivedEventArgs args)
    {
      CommandReceived?.Invoke(this, args);
    }

    protected GameObject gameObject;

    public abstract void Connect(IMultiplayerEndpoint endpoint);

    public void Disconnect() {
        if (State <= MultiplayerClientState.Disconnected)
            throw new NetworkPlatformException("Client not connected");

      if (gameObject)
        UnityObject.Destroy(gameObject);
      doDisconnect();
    }

    protected virtual void doDisconnect() {}

    public abstract void Tick();

    public abstract void Send(IMultiplayerCommand command, MultiplayerCommandOptions options = MultiplayerCommandOptions.None);

    protected void SetState(MultiplayerClientState status) {
        State = status;
        StateChanged?.Invoke(this, new ClientStateChangedEventArgs(status));
    }

}
