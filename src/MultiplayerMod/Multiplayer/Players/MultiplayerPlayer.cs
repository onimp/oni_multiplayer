using System;

namespace MultiplayerMod.Multiplayer.Players;

[Serializable]
public class MultiplayerPlayer {

    public PlayerIdentity Id { get; } = new();
    public PlayerState State { get; set; } = PlayerState.Initializing;
    public PlayerRole Role { get; }
    public PlayerProfile Profile { get; }

    public MultiplayerPlayer(PlayerRole role, PlayerProfile profile) {
        Role = role;
        Profile = profile;
    }

    protected bool Equals(MultiplayerPlayer other) => Id.Equals(other.Id);

    public override bool Equals(object? other) {
        if (ReferenceEquals(null, other))
            return false;
        if (ReferenceEquals(this, other))
            return true;

        return other is MultiplayerPlayer player && Equals(player);
    }

    public override int GetHashCode() => Id.GetHashCode();

    public static bool operator ==(MultiplayerPlayer? left, MultiplayerPlayer? right) => Equals(left, right);

    public static bool operator !=(MultiplayerPlayer? left, MultiplayerPlayer? right) => !Equals(left, right);

    public override string ToString() => $"{nameof(MultiplayerPlayer)} {{ Id = {Id} }}";

}
