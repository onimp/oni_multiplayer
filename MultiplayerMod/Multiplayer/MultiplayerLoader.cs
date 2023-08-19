using HarmonyLib;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Loader;
using MultiplayerMod.Multiplayer.Configuration;
using MultiplayerMod.Multiplayer.Objects;
using MultiplayerMod.Multiplayer.State;

namespace MultiplayerMod.Multiplayer;

// ReSharper disable once UnusedType.Global
[ModComponentOrder(ModComponentOrder.Configuration)]
public class MultiplayerLoader : IModComponentLoader {

    public void OnLoad(Harmony harmony) {
        Dependencies.Register<MultiplayerIdentityProvider>();
        Dependencies.Register<MultiplayerGame>();
        Dependencies.Register(Dependencies.Resolve<MultiplayerObjectsConfigurator>());
        Dependencies.Register(Dependencies.Resolve<MultiplayerCoordinator>());
    }

}
