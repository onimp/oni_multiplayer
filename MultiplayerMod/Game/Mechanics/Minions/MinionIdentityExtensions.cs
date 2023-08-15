using MultiplayerMod.Multiplayer.Objects;

namespace MultiplayerMod.Game.Mechanics.Minions;

public static class MinionIdentityExtensions {

    public static MultiplayerInstance GetMultiplayerInstance(this MinionIdentity identity) {
        identity.ValidateProxy();
        var proxy = identity.assignableProxy.Get();
        return proxy.GetComponent<MultiplayerInstance>();
    }

}
