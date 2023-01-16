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
        }

        public void OnSpawn()
        {
            new GameObject().AddComponent<PlayerStateEffect>();
        }

        private void OnMouseMoved(Pair<float, float> newMousePosition)
        {
            if ((System.DateTime.Now - _lastUpdateSent).TotalMilliseconds < RefreshDelayMS)
                return;
            _lastUpdateSent = System.DateTime.Now;
            _client.SendToServer(new UserAction
            {
                userActionType = UserAction.UserActionTypeEnum.MouseMove,
                Payload = newMousePosition
            });
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
                default:
                    throw new InvalidEnumArgumentException($"Unknown command received {typedMessage.Command}");
            }
        }
    }
}