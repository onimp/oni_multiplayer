using MultiplayerMod.Network;
using Steamworks;

namespace MultiplayerMod.Platform.Steam.Network;

public record SteamServerEndpoint(CSteamID LobbyID) : IMultiplayerEndpoint;

public record DevServerEndpoint() : IMultiplayerEndpoint;