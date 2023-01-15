using System.Collections.Generic;
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
        private Dictionary<int, WorldSaveChunk> _saveChunks = new Dictionary<int, WorldSaveChunk>();

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
            Debug.Log("Save World call");
            var path = string.IsNullOrEmpty(GenericGameSettings.instance.performanceCapture.saveGame)
                ? SaveLoader.GetLatestSaveForCurrentDLC()
                : GenericGameSettings.instance.performanceCapture.saveGame;
            Debug.Log(path);

            // TODO Check whether it is an actual path
            // Otherwise trigger world saving.
            var result = File.ReadAllBytes(path);
            Debug.Log(result.Length);
            return result;
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

        public void LoadWorld(object obj)
        {
            var chunk = (WorldSaveChunk)obj;
            Debug.Log($"ClientActions.LoadWorld chunk {chunk.chunkIndex + 1}/{chunk.totalChunks}");
            _saveChunks[chunk.chunkIndex] = chunk;
            if (chunk.chunkIndex + 1 != chunk.totalChunks)
                return;

            if (_saveChunks.Count != chunk.totalChunks)
            {
                Debug.Log("Some load chunks have been lost.");
                _saveChunks.Clear();
                return;
            }

            var tempFilePath = Path.GetTempFileName();
            using (var writer = new BinaryWriter(File.OpenWrite(tempFilePath)))
            {
                for (var i = 0; i < chunk.totalChunks; i++)
                {
                    writer.Write(_saveChunks[i].chunkData);
                }
            }
            _saveChunks.Clear();

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

        private void OnCommandReceived(ServerToClientEnvelope.ServerToClientMessage serverToClientMessage)
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