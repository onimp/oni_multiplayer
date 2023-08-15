using HarmonyLib;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Loader;
using MultiplayerMod.Game.Extension;

namespace MultiplayerMod.Game;

// ReSharper disable once UnusedType.Global
public class GameComponentLoader : IModComponentLoader {

    public void OnLoad(Harmony harmony) {
        Dependencies.Register(new GameExtensionsConfigurator());
    }

}
