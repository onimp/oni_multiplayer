using System;
using System.Runtime.CompilerServices;
using MultiplayerMod.Core.Extensions;

namespace MultiplayerMod.Multiplayer.Objects;

[Serializable]
public class MultiplayerId {

    public long HighPart { get; }
    public long LowPart { get; }
    public MultiplayerIdType Type { get; }

    public MultiplayerId(InternalMultiplayerIdType type, long internalId) {
        Type = MultiplayerIdType.Internal;
        HighPart = (long)type;
        LowPart = internalId;
    }

    public MultiplayerId(Guid guid) {
        var bytes = guid.ToByteArray();
        Type = MultiplayerIdType.Generated;
        HighPart = GetLong(bytes, 0);
        LowPart = GetLong(bytes, 8);
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
        return Type == other.Type && HighPart == other.HighPart && LowPart == other.LowPart;
    }

    public override bool Equals(object? other) {
        if (ReferenceEquals(null, other))
            return false;
        if (ReferenceEquals(this, other))
            return true;

        return other.GetType() == GetType() && Equals((MultiplayerId) other);
    }

    public override int GetHashCode() {
        var hashCode = HighPart.GetHashCode();
        hashCode = hashCode * 397 ^ LowPart.GetHashCode();
        hashCode = hashCode * 397 ^ Type.GetHashCode();
        return hashCode;
    }

    public static bool operator ==(MultiplayerId? left, MultiplayerId? right) => Equals(left, right);

    public static bool operator !=(MultiplayerId? left, MultiplayerId? right) => !Equals(left, right);

    public override string ToString()
        => $"{((long)Type).ToString(radix: 36)}:{HighPart.ToString(radix: 36)}:{LowPart.ToString(radix: 36)}";

}
