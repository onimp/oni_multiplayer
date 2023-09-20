using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Multiplayer;
using Steamworks;

namespace MultiplayerMod.Platform.Steam;

[Dependency, UsedImplicitly]
public class SteamMultiplayerOperations : IMultiplayerOperations {

    public void Join() => SteamFriends.ActivateGameOverlay("friends");

}
