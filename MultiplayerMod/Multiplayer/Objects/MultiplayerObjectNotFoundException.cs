using System;

namespace MultiplayerMod.Multiplayer.Objects;

public class MultiplayerObjectNotFoundException : Exception {

    public MultiplayerId Id { get; }

    public MultiplayerObjectNotFoundException(MultiplayerId id) {
        Id = id;
    }

}
