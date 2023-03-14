using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using MultiplayerMod.Multiplayer.Effect;
using MultiplayerMod.Multiplayer.Message;
using MultiplayerMod.patch;
using MultiplayerMod.steam;
using Steamworks;
using UnityEngine;

namespace MultiplayerMod.Multiplayer
{

    /// <summary>
    ///     Handles server side events.
    ///     Server here is a Multiplayer itself. Game is not considered as a part of the server (it is one of clients).
    ///     Based on the event either broadcast a message or change server state accordingly .
    /// </summary>
    public class ServerActions : KMonoBehaviour
    {

        // 33 ms is 30 hz
        private const int refreshDelayMS = 33;
        private ClientActions clientActions;

        private System.DateTime lastUpdateTime;
        private PlayersState playersState;
        private Server server;

        private void Update()
        {
            if ((System.DateTime.Now - lastUpdateTime).TotalMilliseconds < refreshDelayMS)
                return;

            lastUpdateTime = System.DateTime.Now;
            server.BroadcastCommand(Command.PlayersState, playersState);
        }

        private void OnEnable()
        {
            server = FindObjectsOfType<Server>().FirstOrDefault();
            clientActions = FindObjectsOfType<ClientActions>().FirstOrDefault();

            server!.ServerCreated += OnServerCreated;
            server!.ClientJoined += OnClientJoined;
            server!.OnCommandReceived += OnCommandReceived;

            SaveLoaderPatch.OnWorldSaved += SaveLoaderPatchOnOnWorldSaved;
            ChoreConsumerPatch.OnFindNextChore += ChoreDriverPatchOnOnChoreSet;
        }

        protected override void OnSpawn()
        {
            var go = new GameObject();
            // To send server infos as their become available
            var worldDiffer = go.AddComponent<WorldDebugDiffer>();
            worldDiffer.OnDebugInfoAvailable += info => server.BroadcastCommand(Command.WorldDebugDiff, info);
        }

        private void OnServerCreated()
        {
            clientActions.ConnectToServer(server.lobbyId);
            playersState = new PlayersState();
            SteamFriends.ActivateGameOverlayToUser("friends", SteamUser.GetSteamID());
        }

        private void SaveLoaderPatchOnOnWorldSaved(string saveFileName)
        {
            HardSyncClients(WorldSaver.ReadWorldSave(saveFileName));
        }

        private void ChoreDriverPatchOnOnChoreSet(object[] payload)
        {
            server.BroadcastCommand(SteamUser.GetSteamID(), Command.ChoreSet, payload);
        }

        private void OnClientJoined(CSteamID steamID)
        {
            // if it is resulted from ourself - skip
            if (steamID == SteamUser.GetSteamID()) return;

            HardSyncClients(WorldSaver.SaveWorld());
        }

        private void HardSyncClients(List<WorldSaveChunk> saveWorldChunks)
        {
            server.BroadcastCommand(
                Command.UserAction,
                new UserAction
                {
                    userActionType = UserAction.UserActionTypeEnum.Pause
                }
            );
            saveWorldChunks.ForEach(
                chunk =>
                    server.BroadcastCommand(SteamUser.GetSteamID(), Command.LoadWorld, chunk)
            );
        }

        private void OnCommandReceived(CSteamID userId, SerializedMessage.TypedMessage typedMessage)
        {
            switch (typedMessage.Command)
            {
                case Command.UserAction:
                    server.BroadcastCommand(userId, Command.UserAction, typedMessage.Payload);
                    break;
                case Command.MouseMove:
                    playersState.UpdateMousePos(userId, (Pair<float, float>)typedMessage.Payload);
                    break;
                default:
                    throw new InvalidEnumArgumentException($"Unknown command received {typedMessage.Command}");
            }
        }
    }

}
