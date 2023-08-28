using MultiplayerMod.Core.Dependency;

namespace MultiplayerMod.ModRuntime.StaticCompatibility;

public static class Dependencies {

    private static readonly DependencyContainer container = Runtime.Instance.Dependencies;

    public static T Inject<T>(T instance) where T : notnull => container.Inject(instance);
    public static T Get<T>() where T : notnull => container.Get<T>();

}
