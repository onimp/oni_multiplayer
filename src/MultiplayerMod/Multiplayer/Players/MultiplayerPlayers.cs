using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MultiplayerMod.Core.Extensions;

namespace MultiplayerMod.Multiplayer.Players;

public class MultiplayerPlayers : IEnumerable<MultiplayerPlayer> {

    private readonly Dictionary<PlayerIdentity, MultiplayerPlayer> players = new();

    private PlayerIdentity currentPlayerId = null!;

    // ReSharper disable once ParameterHidesMember
    public MultiplayerPlayer Add(MultiplayerPlayer player) => players[player.Id] = player;

    public bool Remove(PlayerIdentity id) => players.Remove(id);

    public MultiplayerPlayer this[PlayerIdentity id] => players[id];

    public bool Ready => players.Values.All(player => player.State == PlayerState.Ready);

    public IEnumerator<MultiplayerPlayer> GetEnumerator() => players.Values.GetEnumerator();

    public int Count => players.Values.Count;

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public MultiplayerPlayer Current => players[currentPlayerId];

    // ReSharper disable once ParameterHidesMember
    public void Synchronize(IEnumerable<MultiplayerPlayer> players) {
        this.players.Clear();
        players.ForEach(it => this.players[it.Id] = it);
    }

    public void SetCurrentPlayerId(PlayerIdentity playerId) {
        currentPlayerId = playerId;
    }

}
