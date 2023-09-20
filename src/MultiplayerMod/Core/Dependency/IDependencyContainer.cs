using System;

namespace MultiplayerMod.Core.Dependency;

public interface IDependencyContainer {
    public T Get<T>() where T : notnull;
    public object Get(Type type);
}
