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
        private bool _worldSpawned;

        // 33 ms is 30 hz
        private const int RefreshDelayMS = 33;

        void OnEnable()
        {
            _client = FindObjectsOfType<Client>().FirstOrDefault();
            if (_client == null)
                throw new Exception("Client object is missing.");

            _client.OnCommandReceived += OnCommandReceived;
            InterfaceToolOnMouseMovePatch.OnMouseMove += OnMouseMoved;

            SpeedControlScreenPatches.SetSpeedPatch.OnSetSpeed +=
                (speed) => SendToServer(UserAction.UserActionTypeEnum.SetSpeed, speed);
            SpeedControlScreenPatches.TogglePausePatch.OnPause +=
                () => SendToServer(UserAction.UserActionTypeEnum.Pause);
            SpeedControlScreenPatches.TogglePausePatch.OnUnpause +=
                () => SendToServer(UserAction.UserActionTypeEnum.Unpause);

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
            _worldSpawned = true;
        }

        private void SendToServer(UserAction.UserActionTypeEnum actionType, object payload = null)
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
            if (!_worldSpawned) return;
            switch (userAction.userActionType)
            {
                case UserAction.UserActionTypeEnum.Pause:
                    WorldTimeManager.PauseWorld();
                    break;
                case UserAction.UserActionTypeEnum.Unpause:
                    WorldTimeManager.UnPauseWorld();
                    break;
                case UserAction.UserActionTypeEnum.SetSpeed:
                    WorldTimeManager.SetSpeed((int)userAction.Payload);
                    break;

                case UserAction.UserActionTypeEnum.Dig:
                    DragToolEffect.OnDragTool(DigTool.Instance, userAction.Payload);
                    break;
                case UserAction.UserActionTypeEnum.Attack:
                    DragToolEffect.OnDragComplete(new AttackTool(), userAction.Payload);
                    break;
                case UserAction.UserActionTypeEnum.Build:
                    DragToolEffect.OnDragTool(BuildTool.Instance, userAction.Payload);
                    break;
                case UserAction.UserActionTypeEnum.Cancel:
                    DragToolEffect.OnDragTool(CancelTool.Instance, userAction.Payload);
                    break;
                case UserAction.UserActionTypeEnum.Capture:
                    DragToolEffect.OnDragComplete(new CaptureTool(), userAction.Payload);
                    break;
                case UserAction.UserActionTypeEnum.Clear:
                    DragToolEffect.OnDragTool(ClearTool.Instance, userAction.Payload);
                    break;
                case UserAction.UserActionTypeEnum.CopySettings:
                    DragToolEffect.OnDragTool(CopySettingsTool.Instance, userAction.Payload);
                    break;
                case UserAction.UserActionTypeEnum.Debug:
                    DragToolEffect.OnDragTool(DebugTool.Instance, userAction.Payload);
                    break;
                case UserAction.UserActionTypeEnum.Deconstruct:
                    DragToolEffect.OnDragTool(DeconstructTool.Instance, userAction.Payload);
                    break;
                case UserAction.UserActionTypeEnum.Disinfect:
                    DragToolEffect.OnDragTool(DisinfectTool.Instance, userAction.Payload);
                    break;
                case UserAction.UserActionTypeEnum.EmptyPipe:
                    DragToolEffect.OnDragTool(EmptyPipeTool.Instance, userAction.Payload);
                    break;
                case UserAction.UserActionTypeEnum.Harvest:
                    DragToolEffect.OnDragTool(HarvestTool.Instance, userAction.Payload);
                    break;
                case UserAction.UserActionTypeEnum.Mop:
                    DragToolEffect.OnDragTool(MopTool.Instance, userAction.Payload);
                    break;
                case UserAction.UserActionTypeEnum.Place:
                    DragToolEffect.OnDragTool(PlaceTool.Instance, userAction.Payload);
                    break;
                case UserAction.UserActionTypeEnum.Priority:
                    DragToolEffect.OnDragTool(PrioritizeTool.Instance, userAction.Payload);
                    break;
                default:
                    Debug.LogWarning($"Unknown user action received {userAction.userActionType}");
                    break;
            }
        }
    }
}