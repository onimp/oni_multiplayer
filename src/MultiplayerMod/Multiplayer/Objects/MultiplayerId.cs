using System;
using System.Runtime.CompilerServices;
using MultiplayerMod.Core.Extensions;

namespace MultiplayerMod.Multiplayer.Objects;

[Serializable]
public class MultiplayerId {

    private long UuidA { get; }
    private long UuidB { get; }
    private long InternalId { get; }

    public MultiplayerId(long internalId) {
        InternalId = internalId;
    }

    public MultiplayerId(Guid guid) {
        var bytes = guid.ToByteArray();
        UuidA = GetLong(bytes, 0);
        UuidB = GetLong(bytes, 8);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private long GetLong(byte[] bytes, int offset) {
        return bytes[offset] |
               (long) bytes[offset + 1] << 8 |
               (long) bytes[offset + 2] << 16 |
               (long) bytes[offset + 3] << 24 |
               (long) bytes[offset + 4] << 32 |
               (long) bytes[offset + 5] << 40 |
               (long) bytes[offset + 6] << 48 |
               (long) bytes[offset + 7] << 56;
    }

    protected bool Equals(MultiplayerId other) {
        return UuidA == other.UuidA && UuidB == other.UuidB && InternalId == other.InternalId;
    }

    public override bool Equals(object? other) {
        if (ReferenceEquals(null, other))
            return false;
        if (ReferenceEquals(this, other))
            return true;

        return other.GetType() == GetType() && Equals((MultiplayerId) other);
    }

    public override int GetHashCode() {
        var hashCode = UuidA.GetHashCode();
        hashCode = hashCode * 397 ^ UuidB.GetHashCode();
        hashCode = hashCode * 397 ^ InternalId.GetHashCode();
        return hashCode;
    }

    public static bool operator ==(MultiplayerId? left, MultiplayerId? right) => Equals(left, right);

    public static bool operator !=(MultiplayerId? left, MultiplayerId? right) => !Equals(left, right);

    public override string ToString()
        => $"{UuidA.ToString(radix: 36)}:{UuidB.ToString(radix: 36)}:{InternalId.ToString(radix: 36)}";

}
