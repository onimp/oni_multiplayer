using System.IO;
using Klei;

namespace MultiplayerMod.multiplayer.effect
{
    public static class WorldSaver
    {
        public static byte[] SaveWorld()
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
    }
}