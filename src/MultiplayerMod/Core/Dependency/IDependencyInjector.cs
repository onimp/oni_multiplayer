using System;

namespace MultiplayerMod.Core.Dependency;

public interface IDependencyInjector {
    public T Inject<T>(T instance) where T : notnull;
    public void Inject(Type type);
}
