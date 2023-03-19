using MultiplayerMod.Network.Messaging;
using Steamworks;

namespace MultiplayerMod.Platform.Steam.Network;

public static class SteamExtensions {

    public static INetworkMessageHandle GetNetworkMessageHandle(this SteamNetworkingMessage_t message) =>
        new NetworkMessageHandle(message.m_pData, message.m_cbSize);

}
