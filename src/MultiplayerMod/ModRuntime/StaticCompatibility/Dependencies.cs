using MultiplayerMod.Core.Dependency;

namespace MultiplayerMod.ModRuntime.StaticCompatibility;

public static class Dependencies {

    private static DependencyContainer Container => Runtime.Instance.Dependencies;

    public static T Inject<T>(T instance) where T : notnull => Container.Inject(instance);
    public static T Get<T>() where T : notnull => Container.Get<T>();

}
