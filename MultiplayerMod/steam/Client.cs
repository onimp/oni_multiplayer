using System;
using System.Runtime.InteropServices;
using Steamworks;
using UnityEngine;

namespace MultiplayerMod.steam
{
    public class Client : MonoBehaviour
    {
        private HSteamNetConnection? _conn;

        public Action<ServerToClientEnvelope.ServerToClientMessage> OnCommandReceived;

        void OnEnable()
        {
            SteamAPI.Init();

            if (!SteamManager.Initialized)
                throw new Exception("Steam manager is not initialized");

            SteamNetworkingUtils.InitRelayNetworkAccess();

            Callback<LobbyGameCreated_t>.Create(LogObject);
            Callback<GameRichPresenceJoinRequested_t>.Create(delegate(GameRichPresenceJoinRequested_t t)
            {
                JoinByCommandLine(t.m_rgchConnect);
            });
            Callback<NewUrlLaunchParameters_t>.Create(delegate
            {
                SteamApps.GetLaunchCommandLine(out var cmd, 1024);
                JoinByCommandLine(cmd);
            });
            Callback<SteamServersConnected_t>.Create(LogObject);
            Callback<SteamServersDisconnected_t>.Create(LogObject);
            Callback<SteamServerConnectFailure_t>.Create(LogObject);
            Callback<ItemInstalled_t>.Create(LogObject);
            Callback<DurationControl_t>.Create(LogObject);
            Callback<GameOverlayActivated_t>.Create(LogObject);
        }

        private void LogObject<T>(T t)
        {
            Debug.Log(t);
        }

        void Update()
        {
            if (!SteamManager.Initialized)
                return;
            // Run Steam client callbacks
            SteamAPI.RunCallbacks();
            SteamNetworkingSockets.RunCallbacks();

            if (_conn != null)
                ReceiveNetworkData();
        }

        void OnDestroy()
        {
            Debug.Log("OnDestroy");
            SteamAPI.Shutdown();
        }

        public static void ShowJoinToFriend()
        {
            SteamFriends.ActivateGameOverlay("friends");
        }

        private void JoinByCommandLine(String cmd)
        {
            Debug.Log($"We have been asked to run and join {cmd}");
            if (cmd.Contains("+connect"))
            {
                var steamIdString = cmd.Split(new[] { ' ' })[1];
                Debug.Log($"Trying to connect to {steamIdString}");
                ConnectToServer(new CSteamID(ulong.Parse(steamIdString)));
                return;
            }

            Debug.Log("Unknown command line.");
        }

        public void ConnectToServer(CSteamID serverId)
        {
            var identity = new SteamNetworkingIdentity();
            identity.SetSteamID(serverId);

            _conn = SteamNetworkingSockets.ConnectP2P(ref identity, 0, 0, null);

            Debug.Log($"+connect {serverId}");
            // TODO make sure that other client can handle it and connect
            // TODO Make sure that there is no game startup warning
            SteamFriends.SetRichPresence("connect", $"+connect {serverId}");
        }

        private void ReceiveNetworkData()
        {
            var messages = new IntPtr[128];
            int numMessages =
                SteamNetworkingSockets.ReceiveMessagesOnConnection(_conn!.Value, messages, 128);
            for (int idxMsg = 0; idxMsg < numMessages; idxMsg++)
            {
                var message = (SteamNetworkingMessage_t)((GCHandle)messages[idxMsg]).Target;
                var steamIDRemote = message.m_identityPeer.GetSteamID();
                var connection = message.m_conn;

                Debug.Log($"Received message from {steamIDRemote}");

                var msg = ServerToClientEnvelope.ServerToClientMessage.ToServerToClientMessage(message.m_pData); 

                OnCommandReceived?.Invoke(msg);

                message.Release();
            }
        }
    }
}