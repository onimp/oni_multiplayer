using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Multiplayer;
using MultiplayerMod.Network;
using MultiplayerMod.Platform.Gns.Network;

namespace MultiplayerMod.Platform.Gns;

public class GnsMultiplayerOperations : IMultiplayerOperations {

    public void Join() {
        Container.Get<IMultiplayerClient>().Connect(new DevServerEndpoint());
    }

}
