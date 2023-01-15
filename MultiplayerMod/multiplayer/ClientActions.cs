using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using Klei;
using MultiplayerMod.steam;
using Steamworks;
using UnityEngine;

namespace MultiplayerMod.multiplayer
{
    public class ClientActions : MonoBehaviour
    {
        private Client _client;

        void OnEnable()
        {
            _client = FindObjectsOfType<Client>().FirstOrDefault();
            if (_client == null)
            {
                Debug.Log(" Client ois null");
                return;
            }

            _client.OnCommandReceived += OnCommandReceived;
        }

        public void ConnectToServer(CSteamID serverId)
        {
            _client.ConnectToServer(serverId);
        }

        public byte[] SaveWorld()
        {
            var path = string.IsNullOrEmpty(GenericGameSettings.instance.performanceCapture.saveGame)
                ? SaveLoader.GetLatestSaveForCurrentDLC()
                : GenericGameSettings.instance.performanceCapture.saveGame;

            // TODO Check whether it is an actual path
            return File.ReadAllBytes(path);
        }

        public void PauseWorld()
        {
            Debug.Log("ClientActions.PauseWorld");
            // TODO implement me
        }

        public void UnPauseWorld()
        {
            Debug.Log("ClientActions.UnPauseWorld");
            // TODO implement me
        }

        public void LoadWorld(object saveFile)
        {
            Debug.Log("ClientActions.LoadWorld");
            var tempFilePath = Path.GetTempFileName();
            File.WriteAllBytes(tempFilePath, (byte[])saveFile);


            var methodInfo = typeof(LoadScreen).GetMethod("DoLoad",
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static, null,
                CallingConventions.Any,
                new[] { typeof(string) }, null);
            if (methodInfo == null)
            {
                Debug.Log("Didn't find DoLoad method");
                return;
            }

            methodInfo.Invoke(null, new object[] { tempFilePath });
        }

        private void OnCommandReceived(steam.ServerToClientEnvelope.ServerToClientMessage serverToClientMessage)
        {
            switch (serverToClientMessage.Command)
            {
                case Command.Pause:
                    PauseWorld();
                    break;
                case Command.Unpause:
                    UnPauseWorld();
                    break;
                case Command.LoadWorld:
                    LoadWorld(serverToClientMessage.Payload);
                    break;
                default:
                    throw new InvalidEnumArgumentException($"Unknown command received {serverToClientMessage.Command}");
            }
        }
    }
}