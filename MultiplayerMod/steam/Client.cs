using System;
using System.Runtime.InteropServices;
using MultiplayerMod.multiplayer;
using Steamworks;
using UnityEngine;

namespace MultiplayerMod.steam
{

    public class Client : MonoBehaviour
    {
        private HSteamNetConnection _conn;
        private CallResult<LobbyEnter_t> lobbyJoined;
        private CSteamID lobbyId = CSteamID.Nil;

        private void Update()
        {
            if (!SteamManager.Initialized)
                return;

            // Run Steam client callbacks
            SteamAPI.RunCallbacks();
            SteamNetworkingSockets.RunCallbacks();

            if (MultiplayerState.IsConnected)
                ReceiveNetworkData();
        }

        private void OnEnable()
        {
            SteamAPI.Init();

            if (!SteamManager.Initialized)
                throw new Exception("Steam manager is not initialized");

            SteamNetworkingUtils.InitRelayNetworkAccess();

            Callback<GameLobbyJoinRequested_t>.Create(t => { ConnectToServer(t.m_steamIDLobby, false); });
            SteamClient.SetWarningMessageHook((severity, text) => { Debug.Log($"Steam warning. {severity} {text}."); });
            Callback<SteamNetConnectionStatusChangedCallback_t>.Create(t => {
                Debug.Log($"[Client] SteamNetConnectionStatusChangedCallback_t: {t.m_info.m_eState}");
            });

            var args = Environment.GetCommandLineArgs();
            if (args.Length <= 2 || args[1] != "+connect_lobby")
                return;

            var id = new CSteamID(ulong.Parse(args[2]));
            Debug.Log($"Connect via the command line to {id}");
            ConnectToServer(id, false);
        }

        private void OnDestroy()
        {
            Debug.Log("OnDestroy");
            if (MultiplayerState.IsConnected) {
                SteamMatchmaking.LeaveLobby(lobbyId);
                SteamNetworkingSockets.CloseConnection(_conn, 0, "", false);
            }
            SteamAPI.Shutdown();
        }

        public event Action<bool> OnConnectedToServer;
        public event Action<SerializedMessage.TypedMessage> OnCommandReceived;

        public void SendUserActionToServer(object payload = null)
        {
            SendCommandToServer(Command.UserAction, payload);
        }

        public void SendCommandToServer(Command command, object payload = null)
        {
            if (!MultiplayerState.IsConnected) return;

            using var message =
                new SerializedMessage(command, payload);
            var result = SteamNetworkingSockets.SendMessageToConnection(
                _conn,
                message.IntPtr,
                message.Size,
                Steamworks.Constants.k_nSteamNetworkingSend_Reliable,
                out var messageOut
            );
            if (result != EResult.k_EResultOK && messageOut == 0)
            {
                Debug.Log($"Failed ({result}) to send message. Message out is {messageOut}.");
                Debug.Log(
                    $"Args are {_conn} {message.IntPtr} {message.Size} {Steamworks.Constants.k_nSteamNetworkingSend_Reliable}"
                );
            }
        }

        public void ConnectToServer(CSteamID lobbyId, bool isLocal)
        {
            MultiplayerState.ConnectToServer();
            var lobbyJoinResult = SteamMatchmaking.JoinLobby(lobbyId);
            lobbyJoined = CallResult<LobbyEnter_t>.Create((_, _) => {
                this.lobbyId = lobbyId;
                Debug.Log($"[Client] Joined lobby {lobbyId}");
                var res = SteamMatchmaking.GetLobbyGameServer(lobbyId, out var ip, out var port, out var serverId);
                Debug.Log($"[Client] GetLobbyData['srvId']: {serverId}");
                if (!res)
                    return;
                Debug.Log($"[Client] Lobby game server id: {serverId}");

                var identity = new SteamNetworkingIdentity();
                identity.SetSteamID(serverId);
                _conn = SteamNetworkingSockets.ConnectP2P(ref identity, 0, 1, new[] { Server.BufferSizeConfig });

                Debug.Log($"[Client] ConnectP2P to {serverId}");
                SteamFriends.SetRichPresence("connect", $"+connect_lobby {lobbyId}");
                MultiplayerState.Connected();

                OnConnectedToServer?.Invoke(isLocal);
            });
            lobbyJoined.Set(lobbyJoinResult);
        }

        private void ReceiveNetworkData()
        {
            var messages = new IntPtr[128];
            var numMessages =
                SteamNetworkingSockets.ReceiveMessagesOnConnection(_conn, messages, 128);
            for (var idxMsg = 0; idxMsg < numMessages; idxMsg++)
            {
                var message =
                    (SteamNetworkingMessage_t)Marshal.PtrToStructure(
                        messages[idxMsg],
                        typeof(SteamNetworkingMessage_t)
                    );

                var msg = SerializedMessage.TypedMessage.DeserializeMessage(
                    message.m_pData,
                    message.m_cbSize
                );

                OnCommandReceived?.Invoke(msg);

                SteamNetworkingMessage_t.Release(messages[idxMsg]);
            }
        }
    }

}
