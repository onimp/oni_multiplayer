using MultiplayerMod.Core.Dependency;

namespace MultiplayerMod.ModRuntime.StaticCompatibility;

public static class Dependencies {

    private static IDependencyContainer Container => Runtime.Instance.Dependencies;

    public static T Get<T>() where T : notnull => Container.Get<T>();

}
