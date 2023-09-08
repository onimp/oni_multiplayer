using System;
using System.Collections.Generic;
using MultiplayerMod.ModRuntime;
using MultiplayerMod.Multiplayer.Commands;
using MultiplayerMod.Network;

namespace MultiplayerMod.Test.Environment.Network;

public class TestMultiplayerClient : IMultiplayerClient {

    public event Action<MultiplayerClientState>? StateChanged;
    public event Action<IMultiplayerCommand>? CommandReceived;

    private readonly TestRuntime runtime;
    private TestMultiplayerServer server = null!;
    private readonly Queue<System.Action> pendingActions = new();

    public IMultiplayerClientId Id { get; }
    public MultiplayerClientState State { get; private set; } = MultiplayerClientState.Disconnected;

    public bool EnablePendingActions { get; set; }

    public TestMultiplayerClient(TestMultiplayerClientId id) {
        Id = id;
        runtime = (TestRuntime) Runtime.Instance;
        id.Client = this;
    }

    public void Connect(IMultiplayerEndpoint endpoint) {
        server = ((TestMultiplayerEndpoint) endpoint).Server;
        EnqueuePendingAction(() => SetState(MultiplayerClientState.Connecting));
        EnqueuePendingAction(
            () => {
                // State must show the client is connected, but the state change event must be postponed
                State = MultiplayerClientState.Connected;
                server.Accept(Id);
            }
        );
        EnqueuePendingAction(() => StateChanged?.Invoke(State));
    }

    public void Disconnect() {
        EnqueuePendingAction(
            () => {
                server.Drop(Id);
                SetState(MultiplayerClientState.Disconnected);
            }
        );
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

    public void Send(IMultiplayerCommand command, MultiplayerCommandOptions options = MultiplayerCommandOptions.None) {
        server.Receive(this, CommandTools.Copy(command), options);
    }

    public void SetState(MultiplayerClientState state) {
        State = state;
        StateChanged?.Invoke(state);
    }

    public void Receive(IMultiplayerCommand command) {
        var oldRuntime = (TestRuntime) Runtime.Instance;
        try {
            runtime.Activate();
            CommandReceived?.Invoke(CommandTools.Copy(command));
        } finally {
            oldRuntime.Activate();
        }
    }

}
