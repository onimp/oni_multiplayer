using System;
using System.Runtime.CompilerServices;

namespace MultiplayerMod.Multiplayer.Objects;

[Serializable]
public class MultiplayerId {

    private readonly long uuidA;
    private readonly long uuidB;
    private readonly int instanceId;

    public MultiplayerId(int instanceId) {
        this.instanceId = instanceId;
    }

    public MultiplayerId(Guid guid) {
        var bytes = guid.ToByteArray();
        uuidA = GetLong(bytes, 0);
        uuidB = GetLong(bytes, 8);
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
        return uuidA == other.uuidA && uuidB == other.uuidB && instanceId == other.instanceId;
    }

    public override bool Equals(object? other) {
        if (ReferenceEquals(null, other))
            return false;
        if (ReferenceEquals(this, other))
            return true;

        return other.GetType() == GetType() && Equals((MultiplayerId) other);
    }

    public override int GetHashCode() {
        var hashCode = uuidA.GetHashCode();
        hashCode = hashCode * 397 ^ uuidB.GetHashCode();
        hashCode = hashCode * 397 ^ instanceId;
        return hashCode;
    }

    public static bool operator ==(MultiplayerId? left, MultiplayerId? right) => Equals(left, right);

    public static bool operator !=(MultiplayerId? left, MultiplayerId? right) => !Equals(left, right);

    public override string ToString() {
        return $"MultiplayerId {{ UUID = {uuidA:x16}{uuidB:x16}, InstanceId = {instanceId} }}";
    }

}
