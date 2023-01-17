using System;
using System.ComponentModel;
using System.Linq;
using MultiplayerMod.multiplayer.effect;
using MultiplayerMod.multiplayer.message;
using MultiplayerMod.patch;
using MultiplayerMod.steam;
using Steamworks;
using UnityEngine;

namespace MultiplayerMod.multiplayer
{
    /// <summary>
    /// Contains and handles user events.
    /// Based on the event either send it to the server or change game via `Effect` class.
    /// </summary>
    public class ClientActions : MonoBehaviour
    {
        private Client _client;

        private System.DateTime _lastUpdateSent;

        // 33 ms is 30 hz
        private const int RefreshDelayMS = 33;

        void OnEnable()
        {
            _client = FindObjectsOfType<Client>().FirstOrDefault();
            if (_client == null)
                throw new Exception("Client object is missing.");

            _client.OnCommandReceived += OnCommandReceived;
            InterfaceToolOnMouseMovePatch.OnMouseMove += OnMouseMoved;

            DragToolPatches.AttackToolPatch.OnDragComplete += (payload) =>
                SendToServer(UserAction.UserActionTypeEnum.Attack, payload);
            DragToolPatches.BaseUtilityBuildToolPatch.OnDragTool += (payload) =>
                SendToServer(UserAction.UserActionTypeEnum.Priority, payload);
            DragToolPatches.BuildToolPatch.OnDragTool +=
                (payload) => SendToServer(UserAction.UserActionTypeEnum.Build, payload);
            DragToolPatches.CancelToolPatch.OnDragTool +=
                (payload) => SendToServer(UserAction.UserActionTypeEnum.Cancel, payload);
            DragToolPatches.CaptureToolPatch.OnDragComplete +=
                (payload) => SendToServer(UserAction.UserActionTypeEnum.Capture, payload);
            DragToolPatches.ClearToolPatch.OnDragTool +=
                (payload) => SendToServer(UserAction.UserActionTypeEnum.Clear, payload);
            DragToolPatches.CopySettingsToolPatch.OnDragTool +=
                (payload) => SendToServer(UserAction.UserActionTypeEnum.CopySettings, payload);
            DragToolPatches.DebugToolPatch.OnDragTool +=
                (payload) => SendToServer(UserAction.UserActionTypeEnum.Debug, payload);
            DragToolPatches.DeconstructToolPatch.OnDragTool +=
                (payload) => SendToServer(UserAction.UserActionTypeEnum.Deconstruct, payload);
            DragToolPatches.DigToolPatch.OnDragTool +=
                (payload) => SendToServer(UserAction.UserActionTypeEnum.Dig, payload);
            DragToolPatches.DisinfectToolPatch.OnDragTool +=
                (payload) => SendToServer(UserAction.UserActionTypeEnum.Disinfect, payload);
            DragToolPatches.EmptyPipeToolPatch.OnDragTool +=
                (payload) => SendToServer(UserAction.UserActionTypeEnum.EmptyPipe, payload);
            DragToolPatches.HarvestToolPatch.OnDragTool +=
                (payload) => SendToServer(UserAction.UserActionTypeEnum.Harvest, payload);
            DragToolPatches.MopToolPatch.OnDragTool +=
                (payload) => SendToServer(UserAction.UserActionTypeEnum.Mop, payload);
            DragToolPatches.PlaceToolPatch.OnDragTool +=
                (payload) => SendToServer(UserAction.UserActionTypeEnum.Place, payload);
            DragToolPatches.PrioritizeToolPatch.OnDragTool +=
                (payload) => SendToServer(UserAction.UserActionTypeEnum.Priority, payload);
        }

        public void OnSpawn()
        {
            new GameObject().AddComponent<PlayerStateEffect>();
        }

        private void SendToServer(UserAction.UserActionTypeEnum actionType, object payload)
        {
            _client.SendUserActionToServer(new UserAction
            {
                userActionType = actionType,
                Payload = payload
            });
        }

        private void OnMouseMoved(float x, float y)
        {
            if ((System.DateTime.Now - _lastUpdateSent).TotalMilliseconds < RefreshDelayMS)
                return;
            _lastUpdateSent = System.DateTime.Now;
            _client.SendCommandToServer(Command.MouseMove, new Pair<float, float>(x, y));
        }

        public void ConnectToServer(CSteamID serverId)
        {
            _client.ConnectToServer(serverId);
        }


        public void OnPlayerStateChanged(PlayersState playersState)
        {
            PlayerStateEffect.PlayerState = playersState;
        }

        private void OnCommandReceived(SerializedMessage.TypedMessage typedMessage)
        {
            switch (typedMessage.Command)
            {
                case Command.Pause:
                    WorldTimeManager.PauseWorld();
                    break;
                case Command.Unpause:
                    WorldTimeManager.UnPauseWorld();
                    break;
                case Command.LoadWorld:
                    WorldLoader.LoadWorld(typedMessage.Payload);
                    break;
                case Command.PlayersState:
                    OnPlayerStateChanged((PlayersState)typedMessage.Payload);
                    break;
                case Command.UserAction:
                    HandleUserAction((UserAction)typedMessage.Payload);
                    break;
                default:
                    throw new InvalidEnumArgumentException($"Unknown command received {typedMessage.Command}");
            }
        }

        private void HandleUserAction(UserAction userAction)
        {
            switch (userAction.userActionType)
            {
                case UserAction.UserActionTypeEnum.Dig:
                    var array = (int[])userAction.Payload;
                    DigEffect.Dig(array[0], array[1], array[2]);
                    break;
                case UserAction.UserActionTypeEnum.Attack:
                    // TODO Implement me
                    break;
                case UserAction.UserActionTypeEnum.Build:
                    // TODO Implement me
                    break;
                case UserAction.UserActionTypeEnum.Cancel:
                    // TODO Implement me
                    break;
                case UserAction.UserActionTypeEnum.Capture:
                    // TODO Implement me
                    break;
                case UserAction.UserActionTypeEnum.Clear:
                    // TODO Implement me
                    break;
                case UserAction.UserActionTypeEnum.CopySettings:
                    // TODO Implement me
                    break;
                case UserAction.UserActionTypeEnum.Debug:
                    // TODO Implement me
                    break;
                case UserAction.UserActionTypeEnum.Deconstruct:
                    // TODO Implement me
                    break;
                case UserAction.UserActionTypeEnum.Disinfect:
                    // TODO Implement me
                    break;
                case UserAction.UserActionTypeEnum.EmptyPipe:
                    // TODO Implement me
                    break;
                case UserAction.UserActionTypeEnum.Harvest:
                    // TODO Implement me
                    break;
                case UserAction.UserActionTypeEnum.Mop:
                    // TODO Implement me
                    break;
                case UserAction.UserActionTypeEnum.Place:
                    // TODO Implement me
                    break;
                case UserAction.UserActionTypeEnum.Priority:
                    // TODO Implement me
                    break;
                default:
                    Debug.LogWarning($"Unknown user action received {userAction.userActionType}");
                    break;
            }
        }
    }
}