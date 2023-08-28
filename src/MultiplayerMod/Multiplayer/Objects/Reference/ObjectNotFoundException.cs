using System;

namespace MultiplayerMod.Multiplayer.Objects.Reference;

public class ObjectNotFoundException : Exception {

    public GameObjectReference Reference { get; }

    public ObjectNotFoundException(GameObjectReference reference) {
        Reference = reference;
    }

}
