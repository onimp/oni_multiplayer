using System;
using HarmonyLib;

namespace MultiplayerMod.patch
{
    static class SpeedControlScreenPatches
    {
        public static bool DisablePatch;

        [HarmonyPatch(typeof(SpeedControlScreen), nameof(SpeedControlScreen.SetSpeed))]
        public static class SetSpeedPatch
        {
            public static event Action<int> OnSetSpeed;

            public static void Postfix(int Speed)
            {
                if (DisablePatch) return;
                OnSetSpeed?.Invoke(Speed);
            }
        }

        [HarmonyPatch(typeof(SpeedControlScreen), nameof(SpeedControlScreen.TogglePause))]
        public static class TogglePausePatch
        {
            public static event System.Action OnPause;
            public static event System.Action OnUnpause;

            public static void Prefix()
            {
                if (DisablePatch) return;
                if (SpeedControlScreen.Instance.IsPaused)
                {
                    OnUnpause?.Invoke();
                }
                else
                    OnPause?.Invoke();
            }
        }
    }
}