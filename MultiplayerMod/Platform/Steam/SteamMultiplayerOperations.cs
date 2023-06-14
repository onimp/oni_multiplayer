using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Multiplayer;
using MultiplayerMod.Network;
using Steamworks;

namespace MultiplayerMod.Platform.Steam;

public class SteamMultiplayerOperations : IMultiplayerOperations {

    public void Join()
    {
#if !USE_DEV_NET
        SteamFriends.ActivateGameOverlay("friends");
#else
        Container.Get<IMultiplayerClient>().Connect(null);
#endif
    }

}
