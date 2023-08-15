extern alias ValveSockets;
using System;
using System.Text;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Extensions;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.Core.Unity;
using MultiplayerMod.Multiplayer;
using MultiplayerMod.Network;
using MultiplayerMod.Network.Events;
using MultiplayerMod.Platform.Base.Network;
using MultiplayerMod.Platform.Base.Network.Components;
using MultiplayerMod.Platform.Base.Network.Messaging;
using ValveSockets::Valve.Sockets;

namespace MultiplayerMod.Platform.Gns.Network;

public class GNSClient : BaseClient {

    private readonly Core.Logging.Logger log = LoggerFactory.GetLogger<GNSClient>();

    public static string Identity = "server";
    private readonly Lazy<IPlayer> playerContainer = new(() => new DevPlayer(Identity));
    protected override Lazy<IPlayer> getPlayer() => playerContainer;

    private readonly NetworkMessageProcessor messageProcessor = new();
    private readonly NetworkMessageFactory messageFactory = new();

    private NetworkingSockets devClient = null!;
    private uint devConnection;

    private NetworkingUtils utils = null!;

    private int reconnectAttempts = 0;

    private GnsServer getServer() {
        return (GnsServer) Container.Get<IMultiplayerServer>();
    }

    private void forwardStatusCallbackToServer(ref StatusInfo info) {
        var server = getServer();
        if (server != null)
            server.StatusCallback(ref info);
    }

    private void connectToServer() {
        log.Info($"Client connecting... with identity '{Identity}'");
        Address address = new Address();
        address.SetAddress("127.0.0.1", 8081);
        devConnection = devClient.Connect(ref address);
        log.Info($"New devConnection {devConnection}");
    }

    private StatusCallback status = null!;

    private void statusCallback(ref StatusInfo info) {
        if (info.connection != devConnection) {
            forwardStatusCallbackToServer(ref info);
            return;
        }

        switch (info.connectionInfo.state) {
            case ConnectionState.None:
                break;

            case ConnectionState.Connected:
                log.Info("Client connected to server - ID: " + info.connection);
                log.Info($"Sending identity '{Identity}' to server");
                devClient.SendMessageToConnection(
                    info.connection,
                    Encoding.ASCII.GetBytes(Identity),
                    SendFlags.Reliable
                );
                SetState(MultiplayerClientState.Connected);
                break;

            case ConnectionState.ClosedByPeer:
            case ConnectionState.ProblemDetectedLocally:
                devClient.CloseConnection(info.connection);
                log.Info($"Client disconnected from server {info.connectionInfo.state}");
                if (reconnectAttempts < 20) {
                    reconnectAttempts++;
                    log.Info($"Attempting to reconnect {reconnectAttempts} times.");
                    connectToServer();
                } else {
                    log.Warning($"Maximum reconnect attempts reached. Won't try to reconnect.");
                }
                break;
        }
    }

    private DebugCallback debug = null!;

    private void debugCallback(DebugType type, string message) {
        if (type > DebugType.Message)
            return;

        log.Info($"GNS Debug: {type} - {message}");
    }

    public override void Connect(IMultiplayerEndpoint endpoint) {
        if (endpoint == null)
            Identity = "client";

        devClient = new NetworkingSockets();

        utils = new NetworkingUtils();
        status = (ref StatusInfo info) => this.statusCallback(ref info);
        utils.SetStatusCallback(status);
        debug = (DebugType type, string message) => this.debugCallback(type, message);
        // utils.SetDebugCallback(DebugType.Message, debug);

        SetState(MultiplayerClientState.Connecting);
        connectToServer();
        gameObject = UnityObject.CreateStaticWithComponent<ClientComponent>();
        // Run callbacks immediately so that the client on the server reacts to the
        // connection in a timely manner. Otherwise the connection would fail with
        // problem detected locally:
        devClient.RunCallbacks();
    }

    public override void Tick() {
        devClient.RunCallbacks();

        if (State != MultiplayerClientState.Connected)
            return;

        ReceiveDevCommands();
    }

    public override void Send(
        IMultiplayerCommand command,
        MultiplayerCommandOptions options = MultiplayerCommandOptions.None
    ) {
        if (State != MultiplayerClientState.Connected)
            throw new NetworkPlatformException("Client not connected");

        log.Info($"Sending {command} to server.");
        messageFactory.Create(command, options).ForEach(
            handle => {
                var result = devClient.SendMessageToConnection(
                    devConnection,
                    handle.Pointer,
                    handle.Size,
                    SendFlags.Reliable
                );
                if (result != ValveSockets::Valve.Sockets.Result.OK)
                    log.Error($"Failed to send {command}: {result}");
            }
        );
    }

    private void ReceiveDevCommands() {
        const int maxMessages = 20;

        NetworkingMessage[] netMessages = new NetworkingMessage[maxMessages];
        int netMessagesCount = devClient.ReceiveMessagesOnConnection(devConnection, netMessages, maxMessages);
        log.Info($"netMessagesCount = {netMessagesCount}");
        for (int i = 0; i < netMessagesCount; i++) {
            ref NetworkingMessage netMessage = ref netMessages[i];

            log.Info(
                "Message received from server - Channel ID: " + netMessage.channel + ", Data length: " +
                netMessage.length
            );
            var message = messageProcessor.Process(
                devConnection,
                new NetworkMessageHandle(netMessage.data, (uint) netMessage.length)
            );
            if (message != null) {
                log.Info($"Received message: {message}");
                OnCommandReceived(new CommandReceivedEventArgs(null, message.Command));
            }
            netMessage.Destroy();
        }
    }
}
