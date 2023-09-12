using JetBrains.Annotations;
using MultiplayerMod.Multiplayer.Commands;
using MultiplayerMod.Multiplayer.CoreOperations.CommandExecution;
using MultiplayerMod.Network;

namespace MultiplayerMod.Multiplayer.CoreOperations;

[UsedImplicitly]
public class MultiplayerCommandController {

    private readonly MultiplayerCommandExecutor executor;

    public MultiplayerCommandController(
        IMultiplayerServer server,
        IMultiplayerClient client,
        MultiplayerCommandExecutor executor
    ) {
        this.executor = executor;

        server.CommandReceived += OnServerReceivedCommand;
        client.CommandReceived += OnClientReceivedCommand;
    }

    private void OnClientReceivedCommand(IMultiplayerCommand command) {
        executor.Execute(null, command);
    }

    private void OnServerReceivedCommand(IMultiplayerClientId clientId, IMultiplayerCommand command) {
        executor.Execute(clientId, command);
    }

}
