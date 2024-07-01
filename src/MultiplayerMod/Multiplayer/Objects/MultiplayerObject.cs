namespace MultiplayerMod.Multiplayer.Objects;

public class MultiplayerObject(MultiplayerId id, int generation, bool persistent = true) {

    public MultiplayerId Id { get; } = id;

    public bool Persistent { get; } = persistent;
    public int Generation { get; } = generation;

    protected bool Equals(MultiplayerObject other) => Id.Equals(other.Id);

    public override bool Equals(object? other) {
        if (ReferenceEquals(null, other))
            return false;
        if (ReferenceEquals(this, other))
            return true;

        return other.GetType() == GetType() && Equals((MultiplayerObject) other);
    }

    public override int GetHashCode() => Id.GetHashCode();

    public static bool operator ==(MultiplayerObject? left, MultiplayerObject? right) => Equals(left, right);

    public static bool operator !=(MultiplayerObject? left, MultiplayerObject? right) => !Equals(left, right);

    public override string ToString() => $"{(Persistent ? "[P] " : "")}{Id}";

}
