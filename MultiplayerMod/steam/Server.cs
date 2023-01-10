using System;
using System.Runtime.InteropServices;
using System.Text;
using Steamworks;
using UnityEngine;

namespace MultiplayerMod.steam
{
    public class Server : MonoBehaviour
    {
        private HSteamNetPollGroup m_hNetPollGroup;

        public delegate void SimpleEvent();

        public event SimpleEvent ConnectedToSteam;

        public CSteamID SteamId => SteamGameServer.GetSteamID();

        private static bool _hostServerAfterLoad;

        private bool isServerStarted;

        void OnEnable()
        {
            Debug.Log("Multiplayer.RestartAppIfNecessary");

            SteamAPI.RestartAppIfNecessary(new AppId_t(457140));
            Debug.Log("Multiplayer.Server.Init");
            SteamAPI.Init();
            Debug.Log("Multiplayer.Server.Done");

            SteamNetworkingUtils.InitRelayNetworkAccess();

            if (!SteamManager.Initialized)
            {
                throw new Exception("Steam manager is not initialized");
            }

            SteamClient.SetWarningMessageHook(delegate(int severity, StringBuilder text)
            {
                Debug.Log($"Steam warning. {severity} {text}.");
            });
            Callback<SteamServersConnected_t>.CreateGameServer(delegate(SteamServersConnected_t t)
            {
                Debug.Log("Game server created");
                SteamGameServer.SetMaxPlayerCount(4);
                SteamGameServer.SetPasswordProtected(false);
                SteamGameServer.SetServerName($"{SteamFriends.GetPersonaName()}'s game");
                SteamGameServer.SetBotPlayerCount(0); // optional, defaults to zero
                SteamGameServer.SetMapName("MilkyWay");
                ConnectedToSteam?.Invoke();
            });
            Callback<SteamServerConnectFailure_t>.CreateGameServer(delegate(SteamServerConnectFailure_t t)
            {
                Debug.Log("SteamServerConnectFailure_t");
                Debug.Log(t);
            });
            Callback<SteamServersDisconnected_t>.CreateGameServer(delegate(SteamServersDisconnected_t t)
            {
                Debug.Log("SteamServersDisconnected_t");
                Debug.Log(t);
            });
            Callback<GSPolicyResponse_t>.CreateGameServer(delegate(GSPolicyResponse_t t)
            {
                Debug.Log(SteamGameServer.BSecure()
                    ? "ONI Multiplayer is VAC Secure!"
                    : "ONI Multiplayer is not VAC Secure!");
            });
            Callback<ValidateAuthTicketResponse_t>.CreateGameServer(delegate(ValidateAuthTicketResponse_t t)
            {
                Debug.Log("ValidateAuthTicketResponse_t");
                Debug.Log(t);
            });
            Callback<SteamNetConnectionStatusChangedCallback_t>.CreateGameServer(Steam_HandleIncomingConnection);
        }

        public static void HostServerAfterInit()
        {
            Debug.Log("Will host server after world is ready.");
            _hostServerAfterLoad = true;
        }

        public void HostServerIfNeeded()
        {
            if (!_hostServerAfterLoad) return;
            // Avoid multiplayer in follow up game loads.
            _hostServerAfterLoad = false;
            StartServer();
        }

        public void StartServer()
        {
            Debug.Log("Multiplayer server startup");
            Debug.Log(SteamFriends.GetPersonaName());

            if (!GameServer.Init(0, 27020, 27015, EServerMode.eServerModeNoAuthentication, "0.0.1.0"))
            {
                // SteamGameServer.Init() failed, log an error and return
                Debug.LogError("SteamGameServer.Init() failed.");
                return;
            }

            SteamGameServer.SetModDir("OxygenNotIncluded");
            SteamGameServer.SetProduct("OxygenNotIncluded Multiplayer");
            SteamGameServer.SetGameDescription("OxygenNotIncluded Multiplayer");


            SteamGameServer.LogOnAnonymous();
            SteamNetworkingUtils.InitRelayNetworkAccess();
            SteamGameServer.SetAdvertiseServerActive(true);

            SteamGameServerNetworkingSockets.CreateListenSocketP2P(0, 0, null);
            m_hNetPollGroup = SteamGameServerNetworkingSockets.CreatePollGroup();
            isServerStarted = true;
        }

        void Update()
        {
            if (!isServerStarted)
                return;
            // Run Steam client callbacks
            GameServer.RunCallbacks();
            SteamGameServerNetworkingSockets.RunCallbacks();
            ReceiveNetworkData();
        }

        void OnDestroy()
        {
            Debug.Log("Server.OnDestroy");
            GameServer.Shutdown();
            isServerStarted = false;
        }

        private void Steam_HandleIncomingConnection(SteamNetConnectionStatusChangedCallback_t pCallback)
        {
            // Connection handle
            var hConn = pCallback.m_hConn;

            // Full connection info
            var info = pCallback.m_info;

            // Previous state.  (Current state is in m_info.m_eState)
            var eOldState = pCallback.m_eOldState;

            // Parse information to know what was changed

            // Check if a client has connected
            if (info.m_hListenSocket != HSteamListenSocket.Invalid &&
                eOldState == ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_None &&
                info.m_eState == ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_Connecting)
            {
                // Connection from a new client
                // Search for an available slot
                //  for (var i = 0; i < 32; i++)
                //  {
                // if (!m_rgClientData[i].m_bActive && !m_rgPendingClientData[i].m_hConn)
                //   {
                // Found one.  "Accept" the connection.
                EResult res = SteamGameServerNetworkingSockets.AcceptConnection(hConn);
                if (res != EResult.k_EResultOK)
                {
                    Debug.Log($"AcceptConnection returned {res}");
                    SteamGameServerNetworkingSockets.CloseConnection(hConn,
                        (int)ESteamNetConnectionEnd.k_ESteamNetConnectionEnd_AppException_Generic,
                        "Failed to accept connection",
                        false);
                    return;
                }

                SteamGameServerNetworkingSockets.SetConnectionPollGroup(hConn, m_hNetPollGroup);

                Debug.Log($"Connection accepted from {pCallback.m_info.m_identityRemote.GetSteamID()}");

                // m_rgPendingClientData[i].m_hConn = hConn;

                // add the user to the poll group
                //  SteamGameServerNetworkingSockets.SetConnectionPollGroup(hConn, m_hNetPollGroup);

                // Send them the server info as a reliable message
                /*  MsgServerSendInfo_t msg;
                  msg.SetSteamIDServer(SteamGameServer.GetSteamID().ConvertToUint64());
                  msg.SetServerName(m_sServerName.c_str());
                  SteamGameServerNetworkingSockets.SendMessageToConnection(hConn, &msg,
                      sizeof(MsgServerSendInfo_t), k_nSteamNetworkingSend_Reliable, nullptr);*/

                return;
                //  }
                //  }

                // No empty slots.  Server full!
                // Debug.Log("Rejecting connection; server full");
                // SteamGameServerNetworkingSockets.CloseConnection(hConn,
                //     (int)ESteamNetConnectionEnd.k_ESteamNetConnectionEnd_AppException_Generic, "Server full!",
                //     false);
            }
            // Check if a client has disconnected
            else if ((eOldState ==
                      ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_Connecting ||
                      eOldState == ESteamNetworkingConnectionState
                          .k_ESteamNetworkingConnectionState_Connected) &&
                     info.m_eState == ESteamNetworkingConnectionState
                         .k_ESteamNetworkingConnectionState_ClosedByPeer)
            {
                // Handle disconnecting a client
                for (var i = 0; i < 32; ++i)
                {
                    // // If there is no ship, skip
                    // if (!m_rgClientData[i].m_bActive)
                    //     continue;
                    //
                    // if (m_rgClientData[i].m_SteamIDUser ==
                    //     info.m_identityRemote.GetSteamID()) //pCallback->m_steamIDRemote)
                    // {
                    //     OutputDebugString("Disconnected dropped user\n");
                    //     RemovePlayerFromServer(i, k_EDRClientDisconnect);
                    //     break;
                    // }
                }
            }
        }

        private void ReceiveNetworkData()
        {
            var msgs = new IntPtr[128];
            int numMessages =
                SteamGameServerNetworkingSockets.ReceiveMessagesOnPollGroup(m_hNetPollGroup, msgs, 128);
            for (int idxMsg = 0; idxMsg < numMessages; idxMsg++)
            {
                var message = (SteamNetworkingMessage_t)((GCHandle)msgs[idxMsg]).Target;
                var steamIDRemote = message.m_identityPeer.GetSteamID();
                var connection = message.m_conn;

                Debug.Log($"Received message from {steamIDRemote}");

                // TODO handle 
                // message.m_pData

                message.Release();
            }
        }
    }
}