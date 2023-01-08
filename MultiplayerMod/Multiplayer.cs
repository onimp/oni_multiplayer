using System;
using System.Collections.Generic;
using System.Text;
using Steamworks;

namespace MultiplayerMod
{
    public class Multiplayer
    {
        private List<String> clientIds = new List<string>();

        public static readonly Multiplayer Instance = new Multiplayer();

        private bool _hostServerAfterLoad;

        private Multiplayer()
        {
            Debug.Log("Multiplayer.RestartAppIfNecessary");
            // SteamAPI.RestartAppIfNecessary(new AppId_t(457140));
            Debug.Log("Multiplayer.Init");
            SteamAPI.Init();
            Debug.Log("Multiplayer.Done");

            if (!SteamManager.Initialized)
            {
                throw new Exception("Steam manager is not initialized");
            }

            SteamClient.SetWarningMessageHook(Steam_OnWarningMessage);
            Callback<LobbyEnter_t>.Create(Steam_LobbyEnter);
            Callback<SteamServersConnected_t>.CreateGameServer(delegate(SteamServersConnected_t t)
            {
                Debug.Log("SteamServersConnected_t");
                Debug.Log(t);
            
                // connecting to our own server
                var identity = new SteamNetworkingIdentity();
                Debug.Log($"Steam id is {SteamGameServer.GetSteamID()}");
                identity.SetSteamID(SteamGameServer.GetSteamID());
                var conn = SteamNetworkingSockets.ConnectP2P(ref identity, 0, 0, null);
                Debug.Log(conn);
                SteamFriends.ActivateGameOverlayToUser("friends", SteamUser.GetSteamID());
                Debug.Log("Lobby should have been created");
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
                Debug.Log("GSPolicyResponse_t");
                Debug.Log(t);
            });
            Callback<ValidateAuthTicketResponse_t>.CreateGameServer(delegate(ValidateAuthTicketResponse_t t)
            {
                Debug.Log("ValidateAuthTicketResponse_t");
                Debug.Log(t);
            });
            Callback<SteamNetConnectionStatusChangedCallback_t>.CreateGameServer(
                delegate(SteamNetConnectionStatusChangedCallback_t pCallback)
                {
                    Debug.Log("SteamNetConnectionStatusChangedCallback_t");
                    /// Connection handle
                    var hConn = pCallback.m_hConn;

                    /// Full connection info
                    var info = pCallback.m_info;

                    /// Previous state.  (Current state is in m_info.m_eState)
                    var eOldState = pCallback.m_eOldState;

                    // Parse information to know what was changed

                    // Check if a client has connected
                    Debug.Log(info.m_hListenSocket);
                    Debug.Log(eOldState);
                    Debug.Log(info.m_eState);
                    
                    if (info.m_hListenSocket != HSteamListenSocket.Invalid &&
                        eOldState == ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_None &&
                        info.m_eState == ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_Connecting)
                    {
                        // Connection from a new client
                        // Search for an available slot
                    //    for (var i = 0; i < 32; i++)
                    //    {
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
                            Debug.Log($"Connection accepted");

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
                });
        }


        public void HostServerAfterInit()
        {
            Debug.Log("Will host server after world is ready.");
            _hostServerAfterLoad = true;
        }

        public void HostServerIfNeeded()
        {
            if (!_hostServerAfterLoad) return;
            // Avoid multiplayer in follow up game loads.
            _hostServerAfterLoad = false;
            Debug.Log("Multiplayer server startup");
            Debug.Log(SteamFriends.GetPersonaName());
            /* CallResult<LobbyCreated_t>.Create(Steam_OnLobbyCreated)
                 .Set(SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, 10));*/
            //   _steamStuff.OnEnable();

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
            SteamNetworkingSockets.CreatePollGroup();
        }

        public void Update()
        {
            // Run Steam client callbacks
            SteamAPI.RunCallbacks();
            GameServer.RunCallbacks();
        }

        public void Shutdown()
        {
            GameServer.Shutdown();
            SteamAPI.Shutdown();
        }

        private void Steam_OnLobbyCreated(LobbyCreated_t lobbyCreatedT, bool failure)
        {
            Debug.Log("Lobby created {failure}. Showing invited dialog.");
            SteamFriends.ActivateGameOverlayInviteDialog(new CSteamID(lobbyCreatedT.m_ulSteamIDLobby));
        }

        private void Steam_OnWarningMessage(int severity, StringBuilder warning)
        {
            Debug.Log($"Steam warning. {severity} {warning}.");
        }

        private void Steam_LobbyEnter(LobbyEnter_t lobbyEnterT)
        {
            Debug.Log($"Lobby is entered by {lobbyEnterT}");
        }

        // Client has joined
        void onJoin()
        {
            HardSyncClients();
        }

        void onMorning()
        {
            HardSyncClients();
        }

        private void HardSyncClients()
        {
            PauseAll();
            var save = OniActions.SaveWorld();
            foreach (var clientId in clientIds)
            {
                HardSyncClient(clientId, save);
            }

            UnpauseAll();
        }

        private void PauseAll()
        {
            OniActions.PauseWorld();
            foreach (var clientId in clientIds)
            {
                SendCommand(clientId, Command.Pause);
            }
        }

        private void UnpauseAll()
        {
            OniActions.UnPauseWorld();
            foreach (var clientId in clientIds)
            {
                SendCommand(clientId, Command.Unpause);
            }
        }


        private void HardSyncClient(string clientId, object world)
        {
            SendCommand(clientId, Command.LoadWorld, world);
        }

        private void SendCommand(string clientId, Command command, object payload = null)
        {
            // TODO send command to the client
        }

        private void onPacketReceived()
        {
        }

        private enum Command
        {
            Pause = 1,
            Unpause = 2,
            LoadWorld = 3,
            CursorMove = 4
        }

        class OniActions
        {
            public static object SaveWorld()
            {
                throw new NotImplementedException();
            }

            public static void PauseWorld()
            {
                throw new NotImplementedException();
            }

            public static void UnPauseWorld()
            {
                throw new NotImplementedException();
            }
        }
    }
}