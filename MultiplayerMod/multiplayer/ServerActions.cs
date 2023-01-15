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
            var saveWorld = _clientActions.SaveWorld();
            SendWorldByChunks(saveWorld);

            _server.BroadcastCommand(Command.Unpause);
        }

        private void SendWorldByChunks(byte[] saveWorld)
        {
            const int maxMsgSize = 100 * 1024; // 100 kb
            var chunksCount = (saveWorld.Length - 1) / maxMsgSize + 1;
            for (var i = 0; i < chunksCount; i++)
            {
                _server.BroadcastCommand(SteamUser.GetSteamID(), Command.LoadWorld,
                    new WorldSaveChunk
                    {
                        chunkIndex = i,
                        totalChunks = chunksCount,
                        chunkData = saveWorld.Skip(i * maxMsgSize).Take(maxMsgSize).ToArray()
                    });
            }
        }
    }
}