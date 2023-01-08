using System;
using Steamworks;
using UnityEngine;

namespace MultiplayerMod.steam
{
    public class Client : MonoBehaviour
    {
        void Start()
        {
            SteamAPI.Init();

            if (!SteamManager.Initialized)
                throw new Exception("Steam manager is not initialized");


            var server = GetComponent<Server>();
            server.ConnectedToSteam += delegate
            {
                ConnectToServer(server.SteamId);
                SteamFriends.ActivateGameOverlayToUser("friends", SteamUser.GetSteamID());
            };

            Callback<LobbyGameCreated_t>.Create(delegate(LobbyGameCreated_t t)
            {
                Debug.Log("Client.LobbyGameCreated_t");
                Debug.Log(t);
            });
            Callback<GameRichPresenceJoinRequested_t>.Create(delegate(GameRichPresenceJoinRequested_t t)
            {
                Debug.Log($"We have been asked to join {t.m_rgchConnect}");
                Debug.Log("Client.GameRichPresenceJoinRequested_t");
                Debug.Log(t);
            });
            Callback<NewUrlLaunchParameters_t>.Create(delegate(NewUrlLaunchParameters_t t)
            {
                string s;
                SteamApps.GetLaunchCommandLine(out s, 1024);
                Debug.Log($"We have been asked to run and join {s}");
                if (s.Contains("+connect"))
                {
                    var steamIdString = s.Split(new[] { ' ' })[1];
                    Debug.Log($"Trying to connect to {steamIdString}");
                    ConnectToServer(new CSteamID(ulong.Parse(steamIdString)));
                    return;
                }

                Debug.Log("Unknown command line.");
            });
            Callback<SteamServersConnected_t>.Create(delegate(SteamServersConnected_t t)
            {
                Debug.Log("!!!!!!!!!!!!!");
                Debug.Log("Client.SteamServersConnected_t");
                Debug.Log(t);
            });
            Callback<SteamServersDisconnected_t>.Create(delegate(SteamServersDisconnected_t t)
            {
                Debug.Log("!!!!!!!!!!!!!");
                Debug.Log("Client.SteamServersDisconnected_t");
                Debug.Log(t);
            });
            Callback<SteamServerConnectFailure_t>.Create(delegate(SteamServerConnectFailure_t t)
            {
                Debug.Log("!!!!!!!!!!!!!");
                Debug.Log("Client.SteamServerConnectFailure_t");
                Debug.Log(t);
            });
            Callback<ItemInstalled_t>.Create(delegate(ItemInstalled_t t)
            {
                Debug.Log("Client.ItemInstalled_t");
                Debug.Log(t);
            });
            Callback<DurationControl_t>.Create(delegate(DurationControl_t t)
            {
                // China stuff
                Debug.Log("Client.DurationControl_t");
                Debug.Log(t);
            });
        }

        void Update()
        {
            if (!SteamManager.Initialized)
                return;
            // Run Steam client callbacks
            SteamAPI.RunCallbacks();
            SteamNetworkingSockets.RunCallbacks();
        }

        void OnDestroy()
        {
            SteamAPI.Shutdown();
        }

        private void ConnectToServer(CSteamID serverId)
        {
            SteamNetworkingIdentity identity = new SteamNetworkingIdentity();
            identity.SetSteamID(serverId);

            SteamNetworkingSockets.ConnectP2P(ref identity, 0, 0, null);

            Debug.Log($"+connect {serverId}");
            SteamFriends.SetRichPresence("connect", $"+connect {serverId}");
        }

        public void JoinToFriend()
        {
            SteamFriends.ActivateGameOverlay("friends");
        }
    }
}