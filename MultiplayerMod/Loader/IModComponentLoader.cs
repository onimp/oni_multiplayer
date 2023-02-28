using HarmonyLib;

namespace MultiplayerMod.Loader;

public interface IModComponentLoader {
    void OnLoad(Harmony harmony);
}
