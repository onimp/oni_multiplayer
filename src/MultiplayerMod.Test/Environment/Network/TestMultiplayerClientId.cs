using System;
using MultiplayerMod.Network;

namespace MultiplayerMod.Test.Environment.Network;

public class TestMultiplayerClientId : IMultiplayerClientId {

    public TestMultiplayerClient Client { get; set; } = null!;

    private readonly int id;

    public TestMultiplayerClientId(int id) {
        this.id = id;
    }

    bool IEquatable<IMultiplayerClientId>.Equals(IMultiplayerClientId other) {
        return other is TestMultiplayerClientId player && player.Equals(this);
    }

    protected bool Equals(TestMultiplayerClientId other) => id == other.id;

    public override bool Equals(object? obj) {
        if (ReferenceEquals(null, obj))
            return false;
        if (ReferenceEquals(this, obj))
            return true;

        return obj.GetType() == GetType() && Equals((TestMultiplayerClientId) obj);
    }

    public override int GetHashCode() => id;

}
