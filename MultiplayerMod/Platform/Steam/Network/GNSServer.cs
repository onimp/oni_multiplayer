extern alias ValveSockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using MultiplayerMod.Core.Collections;
using MultiplayerMod.Core.Extensions;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.Multiplayer;
using MultiplayerMod.Network;
using MultiplayerMod.Network.Events;
using MultiplayerMod.Platform.Steam.Network.Messaging;
using ValveSockets::Valve.Sockets;

namespace MultiplayerMod.Platform.Steam.Network;

[HarmonyPatch(typeof(Global), "OnApplicationQuit")]
public class CleanupPatch {
    public static void Postfix() {
        Library.Deinitialize();
    }
};

public class GNSServer : BaseServer {
    private readonly Core.Logging.Logger log = LoggerFactory.GetLogger<GNSServer>();

    private readonly NetworkMessageProcessor messageProcessor = new();
    private readonly NetworkMessageFactory messageFactory = new();
    private readonly Dictionary<IPlayer, uint> players = new();
    protected override List<IPlayer> getPlayers() => new(players.Keys);
    protected override IMultiplayerEndpoint getEndpoint() => new DevServerEndpoint();

    private IPlayer currentPlayer;
    private NetworkingSockets devServer;
    private uint devPollGroup;
    private uint unidentifiedPollGroup;
    private uint devListenSocket;

    public void StatusCallback(ref StatusInfo info) {
        if (devServer == null)
            return;

        switch (info.connectionInfo.state) {
            case ConnectionState.None:
                break;
            case ConnectionState.Connecting:
                log.Info($"Server accepting connection {info.connection}");
                devServer.AcceptConnection(info.connection);
                devServer.SetConnectionPollGroup(unidentifiedPollGroup, info.connection);
                break;
            case ConnectionState.Connected:
                log.Info("Client connected - ID: " + info.connection + ", IP: " + info.connectionInfo.address.GetIP());
                break;
            case ConnectionState.ClosedByPeer:
            case ConnectionState.ProblemDetectedLocally:
                devServer.CloseConnection(info.connection);
                log.Info(
                    "Client disconnected - ID: " + info.connection + ", IP: " + info.connectionInfo.address.GetIP() +
                    " " + info.connectionInfo.state
                );
                break;
        }
    }

    public override void Tick() {
        switch (State) {
            case MultiplayerServerState.Starting:
            case MultiplayerServerState.Started:
                devServer.RunCallbacks();
                IdentifyClients();
                ReceiveDevMessages();
                break;
        }
    }

    public override void Send(IPlayer player, IMultiplayerCommand command) {
        var connections = new SingletonCollection<uint>(players[player]);
        SendCommand(command, MultiplayerCommandOptions.None, connections);
    }

    public override void Send(IMultiplayerCommand command, MultiplayerCommandOptions options) {
        IEnumerable<KeyValuePair<IPlayer, uint>> recipients = players;
        if (options.HasFlag(MultiplayerCommandOptions.SkipHost))
            recipients = recipients.Where(entry => !entry.Key.Equals(currentPlayer));

        SendCommand(command, options, recipients.Select(it => it.Value));
    }

    private void IdentifyClients() {
        const int maxMessages = 20;
        NetworkingMessage[] netMessages = new NetworkingMessage[maxMessages];
        int count = devServer.ReceiveMessagesOnPollGroup(unidentifiedPollGroup, netMessages, maxMessages);

        for (int i = 0; i < count; ++i) {
            ref NetworkingMessage netMessage = ref netMessages[i];
            log.Info(
                "IdentifyClients(): Message received from - ID: " + netMessage.connection
                                                                  + ", Channel ID: " + netMessage.channel
                                                                  + ", Data length: " + netMessage.length
            );
            var connection = netMessage.connection;
            try {
                byte[] buffer = new byte[64];
                netMessage.CopyTo(buffer);
                int pos = Array.IndexOf(buffer, (byte) 0);
                if (pos < 0)
                    pos = buffer.Length;
                var identity = Encoding.ASCII.GetString(buffer, 0, pos);
                log.Info($"Identified client '{identity}'");
                var connectedPlayer = new DevPlayer(identity);
                if (identity == "server")
                    currentPlayer = connectedPlayer;
                devServer.SetConnectionPollGroup(devPollGroup, connection);
                players[connectedPlayer] = connection;
                OnPlayerConnected(new PlayerConnectedEventArgs(connectedPlayer));
            } catch (Exception) {
                devServer.CloseConnection(connection);
                log.Info($"Failed to identify client - disconnecting {connection}");
            }
        }
    }

    private void ReceiveDevMessages() {
        const int maxMessages = 20;

        NetworkingMessage[] netMessages = new NetworkingMessage[maxMessages];

        int netMessagesCount = devServer.ReceiveMessagesOnPollGroup(devPollGroup, netMessages, maxMessages);
        for (int i = 0; i < netMessagesCount; i++) {
            ref NetworkingMessage netMessage = ref netMessages[i];

            log.Info(
                "Message received from - ID: " + netMessage.connection + ", Channel ID: " + netMessage.channel +
                ", Data length: " + netMessage.length
            );
            var message = messageProcessor.Process(
                netMessage.connection,
                new NetworkMessageHandle(netMessage.data, (uint) netMessage.length)
            );
            if (message != null) {
                log.Info($"Received message {message}");

                var connection = netMessage.connection;
                IPlayer player = players.Where(it => it.Value.Equals(connection)).Select(it => it.Key).First();

                if (message.Options.HasFlag(MultiplayerCommandOptions.ExecuteOnServer)) {
                    OnCommandReceived(new CommandReceivedEventArgs(player, message.Command));
                } else {
                    var connections = players.Where(it => !it.Key.Equals(player)).Select(it => it.Value);
                    SendCommand(message.Command, message.Options, connections);
                }
            }
            netMessage.Destroy();
        }
    }

    private void SendCommand(
        IMultiplayerCommand command,
        MultiplayerCommandOptions options,
        IEnumerable<uint> connections
    ) {
        var sequence = messageFactory.Create(command, options);
        log.Info($"Sending command to clients: {command} in {sequence.Count()} parts");
        sequence.ForEach(handle => connections.ForEach(it => Send(handle, it)));
    }

    private void Send(INetworkMessageHandle handle, uint connection) {
        var result = devServer.SendMessageToConnection(
            connection,
            handle.Pointer,
            handle.Size,
            SendFlags.Reliable
        );
        if (result != ValveSockets::Valve.Sockets.Result.OK)
            log.Error($"Failed to send message, result: {result}");
    }

    protected override void doStart() {
        log.Debug("Starting...");
        SetState(MultiplayerServerState.Preparing);
        SetState(MultiplayerServerState.Starting);
        devServer = new NetworkingSockets();
        devPollGroup = devServer.CreatePollGroup();
        unidentifiedPollGroup = devServer.CreatePollGroup();
        Address serverAddress = new Address();
        serverAddress.SetAddress("127.0.0.1", 8081);
        var configuration = new ValveSockets::Valve.Sockets.Configuration {
            data = new ValveSockets::Valve.Sockets.Configuration.ConfigurationData { Int32 = 10485760 }, // 10 MiB
            value = ConfigurationValue.SendBufferSize,
            dataType = ConfigurationDataType.Int32
        };
        var timeoutInitial = new ValveSockets::Valve.Sockets.Configuration {
            data = new ValveSockets::Valve.Sockets.Configuration.ConfigurationData { Int32 = 10000000 },
            value = ConfigurationValue.TimeoutInitial,
            dataType = ConfigurationDataType.Int32,
        };
        var timeoutConnect = new ValveSockets::Valve.Sockets.Configuration {
            data = new ValveSockets::Valve.Sockets.Configuration.ConfigurationData { Int32 = 10000000 },
            value = ConfigurationValue.TimeoutConnected,
            dataType = ConfigurationDataType.Int32,
        };
        devListenSocket = devServer.CreateListenSocket(
            ref serverAddress,
            new[] {
                configuration, timeoutInitial, timeoutConnect
            }
        );
        SetState(MultiplayerServerState.Started);
    }

    protected override void doReset() {
        devServer.DestroyPollGroup(devPollGroup);
        devServer.DestroyPollGroup(unidentifiedPollGroup);
        log.Info("Destroyed poll groups");
    }

}
