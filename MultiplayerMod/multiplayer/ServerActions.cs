using System.ComponentModel;
using System.Linq;
using MultiplayerMod.multiplayer.effect;
using MultiplayerMod.multiplayer.message;
using MultiplayerMod.steam;
using Steamworks;
using UnityEngine;

namespace MultiplayerMod.multiplayer
{
    /// <summary>
    /// Handles server side events.
    ///
    /// Server here is a Multiplayer itself. Game is not considered as a part of the server (it is one of clients).
    /// Based on the event either broadcast a message or change server state accordingly .
    /// </summary>
    public class ServerActions : MonoBehaviour
    {
        private ClientActions _clientActions;
        private Server _server;
        private PlayersState _playersState;

        private System.DateTime _lastUpdateTime;

        // 33 ms is 30 hz
        private const int RefreshDelayMS = 33;

        void OnEnable()
        {
            _server = FindObjectsOfType<Server>().FirstOrDefault();
            _clientActions = FindObjectsOfType<ClientActions>().FirstOrDefault();
            if (_server == null)
            {
                Debug.Log("Server is null");
                return;
            }

            _server.ServerCreated += OnServerCreated;
            _server.ClientJoined += HardSyncClients;
            _server.OnCommandReceived += OnCommandReceived;
        }

        private void Update()
        {
            if ((System.DateTime.Now - _lastUpdateTime).TotalMilliseconds < RefreshDelayMS)
                return;
            _lastUpdateTime = System.DateTime.Now;
            _server.BroadcastCommand(Command.PlayersState, _playersState);
        }

        private void OnServerCreated()
        {
            _clientActions.ConnectToServer(_server.SteamId);
            _playersState = new PlayersState();
            SteamFriends.ActivateGameOverlayToUser("friends", SteamUser.GetSteamID());
        }

        private void HardSyncClients(CSteamID steamID)
        {
            // if it is resulted from ourself - skip
            if (steamID == SteamUser.GetSteamID()) return;

            _server.BroadcastCommand(Command.Pause);
            var saveWorld = WorldSaver.SaveWorld();
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

        private void OnCommandReceived(CSteamID userId, SerializedMessage.TypedMessage typedMessage)
        {
            switch (typedMessage.Command)
            {
                case Command.UserAction:
                    _server.BroadcastCommand(userId, Command.UserAction, typedMessage.Payload);
                    break;
                case Command.MouseMove:
                    _playersState.UpdateMousePos(userId, (Pair<float, float>)typedMessage.Payload);
                    break;
                default:
                    throw new InvalidEnumArgumentException($"Unknown command received {typedMessage.Command}");
            }
        }
    }
}