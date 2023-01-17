using MultiplayerMod.patch;

namespace MultiplayerMod.multiplayer.effect
{
    public static class WorldTimeManager
    {
        public static void PauseWorld()
        {
            SpeedControlScreenPatches.DisablePatch = true;
            SpeedControlScreen.Instance.Pause();
            SpeedControlScreenPatches.DisablePatch = false;
        }

        public static void UnPauseWorld()
        {
            SpeedControlScreenPatches.DisablePatch = true;
            SpeedControlScreen.Instance.Unpause();
            SpeedControlScreenPatches.DisablePatch = false;
        }

        public static void SetSpeed(int speed)
        {
            SpeedControlScreenPatches.DisablePatch = true;
            SpeedControlScreen.Instance.SetSpeed(speed);
            SpeedControlScreenPatches.DisablePatch = false;
        }
    }
}