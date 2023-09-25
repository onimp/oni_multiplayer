using System;
using System.Collections.Generic;
using System.Linq;
using MultiplayerMod.Core.Extensions;
using MultiplayerMod.ModRuntime;
using MultiplayerMod.Multiplayer.Commands;
using MultiplayerMod.Multiplayer.Commands.Registry;
using MultiplayerMod.Network;

namespace MultiplayerMod.Test.Environment.Network;

public class TestMultiplayerServer : IMultiplayerServer {

    public event Action<MultiplayerServerState>? StateChanged;
    public event Action<IMultiplayerClientId>? ClientConnected;
    public event Action<IMultiplayerClientId>? ClientDisconnected;
    public event Action<IMultiplayerClientId, IMultiplayerCommand>? CommandReceived;

    public MultiplayerServerState State { get; private set; } = MultiplayerServerState.Stopped;
    public IMultiplayerEndpoint Endpoint { get; }
    public List<IMultiplayerClientId> Clients { get; } = new();

    private readonly TestRuntime runtime;
    private readonly MultiplayerCommandRegistry registry;
    private readonly TestMultiplayerClientId currentPlayer;
    private readonly Queue<System.Action> pendingActions = new();

    public bool EnablePendingActions { get; set; }

    public TestMultiplayerServer(
        TestMultiplayerClientId identity,
        TestRuntime runtime,
        MultiplayerCommandRegistry registry
    ) {
        Endpoint = new TestMultiplayerEndpoint(this);
        currentPlayer = identity;
        this.runtime = runtime;
        this.registry = registry;
    }

    public void Start() {
        EnqueuePendingAction(() => SetState(MultiplayerServerState.Preparing));
        EnqueuePendingAction(() => SetState(MultiplayerServerState.Starting));
        EnqueuePendingAction(() => SetState(MultiplayerServerState.Started));
    }

    public void Stop() {
        EnqueuePendingAction(() => SetState(MultiplayerServerState.Stopped));
    }

    public bool RunPendingAction() {
        if (pendingActions.Count <= 0)
            return false;

        pendingActions.Dequeue().Invoke();
        return true;
    }

    private void EnqueuePendingAction(System.Action action) {
        if (EnablePendingActions) {
            pendingActions.Enqueue(action);
            return;
        }
        action();
    }

    public void Send(IMultiplayerClientId player, IMultiplayerCommand command) {
        (player as TestMultiplayerClientId)!.Client.Receive(CommandTools.Copy(command));
    }

    public void Send(IMultiplayerCommand command, MultiplayerCommandOptions options) {
        var oldRuntime = (TestRuntime) Runtime.Instance;
        try {
            runtime.Activate();
            var players = new List<IMultiplayerClientId>(Clients).Cast<TestMultiplayerClientId>();
            if (options.HasFlag(MultiplayerCommandOptions.SkipHost))
                players = players.Where(it => !it.Equals(currentPlayer));
            players.ForEach(it => it.Client.Receive(CommandTools.Copy(command)));
        } finally {
            oldRuntime.Activate();
        }
    }

    private void SetState(MultiplayerServerState state) {
        State = state;
        StateChanged?.Invoke(state);
    }

    public void Accept(IMultiplayerClientId client) {
        Clients.Add(client);
        ClientConnected?.Invoke(client);
    }

    public void Drop(IMultiplayerClientId client) {
        Clients.Remove(client);
        ClientDisconnected?.Invoke(client);
    }

    public void Receive(TestMultiplayerClient source, IMultiplayerCommand command, MultiplayerCommandOptions options) {
        var oldRuntime = (TestRuntime) Runtime.Instance;
        try {
            runtime.Activate();
            var configuration = registry.GetCommandConfiguration(command.GetType());
            if (configuration.ExecuteOnServer) {
                CommandReceived?.Invoke(source.Id, CommandTools.Copy(command));
                return;
            }
            new List<IMultiplayerClientId>(Clients).Cast<TestMultiplayerClientId>()
                .Where(it => !ReferenceEquals(it.Client, source))
                .ForEach(it => it.Client.Receive(CommandTools.Copy(command)));
        } finally {
            oldRuntime.Activate();
        }
    }

}
