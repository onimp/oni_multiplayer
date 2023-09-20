using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Unity;

// ReSharper disable FieldCanBeMadeReadOnly.Local

namespace MultiplayerMod.Platform.Steam.Network.Components;

public class SteamServerComponent : MultiplayerMonoBehaviour {

    [InjectDependency]
    private SteamServer server = null!;

    private void Update() => server.Tick();

}
