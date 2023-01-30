using System;
using System.Runtime.InteropServices;
using Steamworks;
using UnityEngine;

namespace MultiplayerMod.steam
{
    public class Client : MonoBehaviour
    {
        private HSteamNetConnection _conn;
        private bool _connected;

        public event Action<bool> OnConnectedToServer;
        public event Action<SerializedMessage.TypedMessage> OnCommandReceived;

        void OnEnable()
        {
            SteamAPI.Init();

            if (!SteamManager.Initialized)
                throw new Exception("Steam manager is not initialized");

            SteamNetworkingUtils.InitRelayNetworkAccess();

            Callback<GameRichPresenceJoinRequested_t>.Create(delegate(GameRichPresenceJoinRequested_t t)
            {
                JoinByCommandLine(t.m_rgchConnect);
            });
            Callback<NewUrlLaunchParameters_t>.Create(delegate
            {
                SteamApps.GetLaunchCommandLine(out var cmd, 1024);
                JoinByCommandLine(cmd);
            });
        }

        void Update()
        {
            if (!SteamManager.Initialized)
                return;
            // Run Steam client callbacks
            SteamAPI.RunCallbacks();
            SteamNetworkingSockets.RunCallbacks();

            if (_connected)
                ReceiveNetworkData();
        }

        void OnDestroy()
        {
            Debug.Log("OnDestroy");
            SteamAPI.Shutdown();
        }

        public void SendUserActionToServer(object payload = null)
        {
            SendCommandToServer(Command.UserAction, payload);
        }

        public void SendCommandToServer(Command command, object payload = null)
        {
            if (!_connected) return;
            using var message =
                new SerializedMessage(command, payload);
            var result = SteamNetworkingSockets.SendMessageToConnection(_conn,
                message.IntPtr, message.Size,
                Steamworks.Constants.k_nSteamNetworkingSend_Reliable, out var messageOut);
            if (result != EResult.k_EResultOK && messageOut == 0)
            {
                Debug.Log($"Failed ({result}) to send message. Message out is {messageOut}.");
            }
        }

        private void JoinByCommandLine(String cmd)
        {
            Debug.Log($"We have been asked to run and join {cmd}");
            if (cmd.Contains("+connect"))
            {
                var steamIdString = cmd.Split(new[] { ' ' })[1];
                Debug.Log($"Trying to connect to {steamIdString}");
                ConnectToServer(new CSteamID(ulong.Parse(steamIdString)), false);
                return;
            }

            Debug.Log("Unknown command line.");
        }

        public void ConnectToServer(CSteamID serverId, bool isLocal)
        {
            var identity = new SteamNetworkingIdentity();
            identity.SetSteamID(serverId);

            _conn = SteamNetworkingSockets.ConnectP2P(ref identity, 0, 1,
                new[] { Server.BufferSizeConfig });

            Debug.Log($"+connect {serverId}");
            // TODO make sure that other client can handle it and connect
            // TODO Make sure that there is no game startup warning
            SteamFriends.SetRichPresence("connect", $"+connect {serverId}");
            _connected = true;
            OnConnectedToServer?.Invoke(isLocal);
        }

        private void ReceiveNetworkData()
        {
            var messages = new IntPtr[128];
            var numMessages =
                SteamNetworkingSockets.ReceiveMessagesOnConnection(_conn, messages, 128);
            for (var idxMsg = 0; idxMsg < numMessages; idxMsg++)
            {
                var message =
                    (SteamNetworkingMessage_t)Marshal.PtrToStructure(messages[idxMsg],
                        typeof(SteamNetworkingMessage_t));

                var msg = SerializedMessage.TypedMessage.DeserializeMessage(message.m_pData,
                    message.m_cbSize);

                OnCommandReceived?.Invoke(msg);

                SteamNetworkingMessage_t.Release(messages[idxMsg]);
            }
        }
    }
}