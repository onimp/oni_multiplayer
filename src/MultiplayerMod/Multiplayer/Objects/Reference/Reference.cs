using System;

namespace MultiplayerMod.Multiplayer.Objects.Reference;

[Serializable]
public abstract class Reference<T> : Reference {

    public abstract T? Resolve();

    public object? ResolveRaw() => Resolve();
}

public interface Reference {
    public abstract object? ResolveRaw();
}
