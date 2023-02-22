using System;
using HarmonyLib;

namespace MultiplayerMod.patch
{

    [HarmonyPatch(typeof(SaveLoader), nameof(SaveLoader.Save), typeof(string), typeof(bool), typeof(bool))]
    public abstract class SaveLoaderPatch
    {
        public static bool DisablePatch;

        public static event Action<string> OnWorldSaved;

        public static void Postfix(string filename)
        {
            if (DisablePatch) return;

            OnWorldSaved?.Invoke(filename);
        }
    }

}
