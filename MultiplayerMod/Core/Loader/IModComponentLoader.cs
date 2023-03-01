using HarmonyLib;

namespace MultiplayerMod.Core.Loader;

public interface IModComponentLoader {
    void OnLoad(Harmony harmony);
}
