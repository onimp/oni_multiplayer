using System;
using System.Collections.Generic;

namespace MultiplayerMod.Multiplayer.Objects.Reference;

public interface Reference<T> {
    public abstract T? Resolve();
}

public interface Reference {
    public abstract object? Resolve();
}

[Serializable]
public abstract class TypedReference<T> : Reference<T>, Reference {
    public abstract T? Resolve();
    object? Reference.Resolve() => Resolve();
}
