using System.Linq;
using MultiplayerMod.steam;
using Steamworks;
using UnityEngine;

namespace MultiplayerMod.multiplayer
{
    public class ServerActions : MonoBehaviour
    {
        private ClientActions _clientActions;
        private Server _server;

        void OnEnable()
        {
            _server = FindObjectsOfType<Server>().FirstOrDefault();
            _clientActions = FindObjectsOfType<ClientActions>().FirstOrDefault();
            if (_server == null)
            {
                Debug.Log("Server is null");
                return;
            }

            _server.ServerCreated += ServerCreated;
            _server.ClientJoined += HardSyncClients;
        }

        private void ServerCreated()
        {
            _clientActions.ConnectToServer(_server.SteamId);
            SteamFriends.ActivateGameOverlayToUser("friends", SteamUser.GetSteamID());
        }

        private void HardSyncClients(CSteamID steamID)
        {
            // if it is resulted from ourself - skip
            if (steamID == SteamUser.GetSteamID()) return;

            _server.BroadcastCommand(Command.Pause);
            _server.BroadcastCommand(Command.LoadWorld, _clientActions.SaveWorld());
            _server.BroadcastCommand(Command.Unpause);
        }
    }
}