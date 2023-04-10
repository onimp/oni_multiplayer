using HarmonyLib;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Loader;
using MultiplayerMod.Multiplayer.Configuration;

namespace MultiplayerMod.Multiplayer;

[ModComponentOrder(ModComponentOrder.Configuration)]
public class MultiplayerLoader : IModComponentLoader {

    public void OnLoad(Harmony harmony) {
        Container.Register(new MultiplayerCoordinator());
    }

}
