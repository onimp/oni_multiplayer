using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Unity;

// ReSharper disable FieldCanBeMadeReadOnly.Local

namespace MultiplayerMod.Platform.Steam.Network.Components;

public class SteamClientComponent : MultiplayerMonoBehaviour {

    [InjectDependency]
    private SteamClient client = null!;

    private void Update() => client.Tick();

}
