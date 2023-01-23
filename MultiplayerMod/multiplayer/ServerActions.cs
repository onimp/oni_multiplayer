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
    public class ServerActions : KMonoBehaviour
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

            _server!.ServerCreated += OnServerCreated;
            _server!.ClientJoined += HardSyncClients;
            _server!.OnCommandReceived += OnCommandReceived;
        }

        protected override void OnSpawn()
        {
            var go = new GameObject();
            // To send server infos as their become available
            var worldDiffer = go.AddComponent<WorldDebugDiffer>();
            worldDiffer.OnDebugInfoAvailable += info => _server.BroadcastCommand(Command.WorldDebugDiff, info);
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

            _server.BroadcastCommand(Command.UserAction, new UserAction
            {
                userActionType = UserAction.UserActionTypeEnum.Pause
            });
            var saveWorldChunks = WorldSaver.SaveWorld();
            saveWorldChunks.ForEach(chunk =>
                _server.BroadcastCommand(SteamUser.GetSteamID(), Command.LoadWorld, chunk));
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