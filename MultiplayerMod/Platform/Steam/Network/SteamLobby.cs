using MultiplayerMod.Core.Logging;
using MultiplayerMod.Platform.Base.Network;
using Steamworks;

namespace MultiplayerMod.Platform.Steam.Network;

public class SteamLobby {

    public CSteamID Id { get; private set; } = CSteamID.Nil;
    public bool Connected => Id != CSteamID.Nil;

    public CSteamID GameServerId {
        get {
            if (!Connected)
                return CSteamID.Nil;

            return !SteamMatchmaking.GetLobbyGameServer(Id, out _, out _, out var serverId) ? CSteamID.Nil : serverId;
        }
        set {
            if (Connected)
                SteamMatchmaking.SetLobbyGameServer(Id, 0, 0, value);
        }
    }

    public event System.Action? OnCreate;
    public event System.Action? OnLeave;
    public event System.Action? OnJoin;

    private readonly Core.Logging.Logger log = LoggerFactory.GetLogger<SteamLobby>();
    private readonly CallResult<LobbyCreated_t> lobbyCreatedCallback;
    private readonly CallResult<LobbyEnter_t> lobbyEnteredCallback;

    public SteamLobby() {
        lobbyCreatedCallback = CallResult<LobbyCreated_t>.Create(LobbyCreatedCallback);
        lobbyEnteredCallback = CallResult<LobbyEnter_t>.Create(LobbyEnteredCallback);
    }

    public void Create(ELobbyType type = ELobbyType.k_ELobbyTypeFriendsOnly, int maxPlayers = 4) {
        if (Id != CSteamID.Nil)
            Leave();
        lobbyCreatedCallback.Set(SteamMatchmaking.CreateLobby(type, maxPlayers));
    }

    public void Join(CSteamID lobbyId) {
        if (Id == lobbyId)
            return;
        if (Id != CSteamID.Nil)
            Leave();
        lobbyEnteredCallback.Set(SteamMatchmaking.JoinLobby(lobbyId));
    }

    public void Leave() {
        if (Id == CSteamID.Nil)
            return;
        SteamMatchmaking.LeaveLobby(Id);
        try {
            OnLeave?.Invoke();
        } finally {
            Id = CSteamID.Nil;
        }
    }

    private void LobbyCreatedCallback(LobbyCreated_t result, bool failure) {
        if (failure)
            throw new NetworkPlatformException("I/O failure.");

        if (result.m_eResult != EResult.k_EResultOK)
            throw new NetworkPlatformException($"Unable to create a lobby. Error: {result.m_eResult}");

        Id = new CSteamID(result.m_ulSteamIDLobby);
        log.Debug($"Lobby {Id} created");
        OnCreate?.Invoke();
    }

    private void LobbyEnteredCallback(LobbyEnter_t result, bool failure) {
        if (failure)
            throw new NetworkPlatformException("I/O failure.");

        Id = new CSteamID(result.m_ulSteamIDLobby);
        log.Debug($"Joined to lobby {Id}");
        OnJoin?.Invoke();
    }

}
