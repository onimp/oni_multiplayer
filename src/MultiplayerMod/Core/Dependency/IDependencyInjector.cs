namespace MultiplayerMod.Core.Dependency;

public interface IDependencyInjector {
    public T Inject<T>(T instance) where T : notnull;
}
