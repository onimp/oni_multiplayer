using Steamworks;

namespace MultiplayerMod
{
    public class SteamStuff
    {
        // Current game server version
        const string SPACEWAR_SERVER_VERSION = "1.0.0.0";

        // UDP port for the spacewar server to do authentication on (ie, talk to Steam on)
        const ushort SPACEWAR_AUTHENTICATION_PORT = 8766;

        // UDP port for the spacewar server to listen on
        const ushort SPACEWAR_SERVER_PORT = 27015;

        // Tells us when we have successfully connected to Steam
        protected Callback<SteamServersConnected_t> m_CallbackSteamServersConnected;
        // Tells us when there was a failure to connect to Steam
        protected Callback<SteamServerConnectFailure_t> m_CallbackSteamServersConnectFailure;
        // Tells us when we have been logged out of Steam
        protected Callback<SteamServersDisconnected_t> m_CallbackSteamServersDisconnected;
        // Tells us that Steam has set our security policy (VAC on or off)
        protected Callback<GSPolicyResponse_t> m_CallbackPolicyResponse;
        //
        // Various callback functions that Steam will call to let us know about whether we should
        // allow clients to play or we should kick/deny them.
        //
        // Tells us a client has been authenticated and approved to play by Steam (passes auth, license check, VAC status, etc...)
        protected Callback<ValidateAuthTicketResponse_t> m_CallbackGSAuthTicketResponse;
        // client connection state
        protected Callback<P2PSessionRequest_t> m_CallbackP2PSessionRequest;
        protected Callback<P2PSessionConnectFail_t> m_CallbackP2PSessionConnectFail;

        public string m_strServerName = "Test Server";
        public string m_strMapName = "Milky Way";
        public int m_nMaxPlayers = 4;

        bool m_bInitialized;
        bool m_bConnectedToSteam;

        public void OnEnable()
        {
            m_CallbackSteamServersConnected =
                Callback<SteamServersConnected_t>.CreateGameServer(OnSteamServersConnected);
            m_CallbackSteamServersConnectFailure =
                Callback<SteamServerConnectFailure_t>.CreateGameServer(OnSteamServersConnectFailure);
            m_CallbackSteamServersDisconnected =
                Callback<SteamServersDisconnected_t>.CreateGameServer(OnSteamServersDisconnected);
            m_CallbackPolicyResponse = Callback<GSPolicyResponse_t>.CreateGameServer(OnPolicyResponse);

            m_CallbackGSAuthTicketResponse =
                Callback<ValidateAuthTicketResponse_t>.CreateGameServer(OnValidateAuthTicketResponse);
            m_CallbackP2PSessionRequest = Callback<P2PSessionRequest_t>.CreateGameServer(OnP2PSessionRequest);
            m_CallbackP2PSessionConnectFail =
                Callback<P2PSessionConnectFail_t>.CreateGameServer(OnP2PSessionConnectFail);


            m_bInitialized = false;
            m_bConnectedToSteam = false;

            EServerMode eMode = EServerMode.eServerModeAuthenticationAndSecure;

            // Initialize the SteamGameServer interface, we tell it some info about us, and we request support
            // for both Authentication (making sure users own games) and secure mode, VAC running in our game
            // and kicking users who are VAC banned
            m_bInitialized = GameServer.Init(0u, SPACEWAR_AUTHENTICATION_PORT, SPACEWAR_SERVER_PORT, eMode,
                SPACEWAR_SERVER_VERSION);
            if (!m_bInitialized)
            {
                Debug.Log("SteamGameServer_Init call failed");
                return;
            }

            Debug.Log("Initialized");

            SteamGameServer.SetModDir("spacewar");
            SteamGameServer.SetProduct("SteamworksExample");
            SteamGameServer.SetGameDescription("Steamworks Example");
            SteamGameServer.LogOnAnonymous();
            


            Debug.Log("Started.");
        }

        private void OnDisable()
        {
            if (!m_bInitialized)
            {
                return;
            }

            m_CallbackSteamServersConnected.Dispose();
            // Disconnect from the steam servers
            SteamGameServer.LogOff();


            // release our reference to the steam client library
            GameServer.Shutdown();
            m_bInitialized = false;

            Debug.Log("Shutdown.");
        }

        public void Update()
        {
            if (!m_bInitialized)
            {
                return;
            }

            GameServer.RunCallbacks();

            if (m_bConnectedToSteam)
            {
                SendUpdatedServerDetailsToSteam();
            }
        }

        //-----------------------------------------------------------------------------
        // Purpose: Take any action we need to on Steam notifying us we are now logged in
        //-----------------------------------------------------------------------------
        void OnSteamServersConnected(SteamServersConnected_t pLogonSuccess)
        {
            Debug.Log("SpaceWarServer connected to Steam successfully");
            m_bConnectedToSteam = true;

            // log on is not finished until OnPolicyResponse() is called

            // Tell Steam about our server details
            SendUpdatedServerDetailsToSteam();
        }


	void OnSteamServersConnectFailure(SteamServerConnectFailure_t pConnectFailure) {
		m_bConnectedToSteam = false;
		Debug.Log("SpaceWarServer failed to connect to Steam");
	}

	void OnSteamServersDisconnected(SteamServersDisconnected_t pLoggedOff) {
		m_bConnectedToSteam = false;
		Debug.Log("SpaceWarServer got logged out of Steam");
	}

	void OnPolicyResponse(GSPolicyResponse_t pPolicyResponse) {
		// Check if we were able to go VAC secure or not
		if (SteamGameServer.BSecure()) {
			Debug.Log("SpaceWarServer is VAC Secure!");
		}
		else {
			Debug.Log("SpaceWarServer is not VAC Secure!");
		}

		Debug.Log("Game server SteamID:" + SteamGameServer.GetSteamID().ToString());
	}

	void OnValidateAuthTicketResponse(ValidateAuthTicketResponse_t pResponse) {
		Debug.Log("OnValidateAuthTicketResponse Called steamID: " + pResponse.m_SteamID); // Riley

		if (pResponse.m_eAuthSessionResponse == EAuthSessionResponse.k_EAuthSessionResponseOK) {
			// This is the final approval, and means we should let the client play (find the pending auth by steamid)

		}
		else {
			// Looks like we shouldn't let this user play, kick them

		}
	}

	//-----------------------------------------------------------------------------
	// Purpose: Handle clients connecting
	//-----------------------------------------------------------------------------
	void OnP2PSessionRequest(P2PSessionRequest_t pCallback) {
		Debug.Log("OnP2PSesssionRequest Called steamIDRemote: " + pCallback.m_steamIDRemote); // Riley

		// we'll accept a connection from anyone
		SteamGameServerNetworking.AcceptP2PSessionWithUser(pCallback.m_steamIDRemote);
	}

	//-----------------------------------------------------------------------------
	// Purpose: Handle clients disconnecting
	//-----------------------------------------------------------------------------
	void OnP2PSessionConnectFail(P2PSessionConnectFail_t pCallback) {
		Debug.Log("OnP2PSessionConnectFail Called steamIDRemote: " + pCallback.m_steamIDRemote); // Riley

		// socket has closed, kick the user associated with it
	}



        void SendUpdatedServerDetailsToSteam()
        {
            SteamGameServer.SetMaxPlayerCount(m_nMaxPlayers);
            SteamGameServer.SetPasswordProtected(false);
            SteamGameServer.SetServerName(m_strServerName);
            SteamGameServer.SetBotPlayerCount(0); // optional, defaults to zero
            SteamGameServer.SetMapName(m_strMapName);
        }
    }
}