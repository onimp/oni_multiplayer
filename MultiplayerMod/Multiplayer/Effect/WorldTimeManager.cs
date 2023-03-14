using MultiplayerMod.patch;

namespace MultiplayerMod.Multiplayer.Effect
{
    public static class WorldTimeManager
    {
        public static void PauseWorld()
        {
            if (SpeedControlScreen.Instance.IsPaused) return;
            SpeedControlScreenPatches.DisablePatch = true;
            SpeedControlScreen.Instance.Pause();
            SpeedControlScreenPatches.DisablePatch = false;
        }

        public static void UnPauseWorld()
        {
            if (!SpeedControlScreen.Instance.IsPaused) return;

            SpeedControlScreenPatches.DisablePatch = true;
            SpeedControlScreen.Instance.Unpause();
            SpeedControlScreenPatches.DisablePatch = false;
        }

        public static void SetSpeed(int speed)
        {
            if (SpeedControlScreen.Instance.GetSpeed() == speed) return;

            SpeedControlScreenPatches.DisablePatch = true;
            SpeedControlScreen.Instance.SetSpeed(speed);
            SpeedControlScreenPatches.DisablePatch = false;
        }
    }
}
