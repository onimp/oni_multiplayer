using HarmonyLib;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Loader;
using MultiplayerMod.Multiplayer.Configuration;
using MultiplayerMod.Multiplayer.Objects;

namespace MultiplayerMod.Multiplayer;

// ReSharper disable once UnusedType.Global
[ModComponentOrder(ModComponentOrder.Configuration)]
public class MultiplayerLoader : IModComponentLoader {

    public void OnLoad(Harmony harmony) {
        Container.Register(new MultiplayerIdentityProvider());
        Container.Register(new MultiplayerObjectsConfigurator());
        Container.Register(new MultiplayerCoordinator());
    }

}
