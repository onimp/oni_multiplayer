using System;

namespace MultiplayerMod.Multiplayer.Objects.Reference;

public class ObjectNotFoundException(Reference reference) : Exception {

    public Reference Reference { get; } = reference;

}
