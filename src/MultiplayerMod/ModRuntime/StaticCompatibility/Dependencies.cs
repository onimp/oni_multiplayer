using MultiplayerMod.Core.Collections;
using MultiplayerMod.Core.Dependency;

namespace MultiplayerMod.ModRuntime.StaticCompatibility;

public static class Dependencies {

    private static IDependencyContainer Container => Runtime.Instance.Dependencies;

    public static T Get<T>() where T : notnull => Container.Get<T>();

    public static Deconstructable<T1, T2> Get<T1, T2>()
        where T1 : notnull
        where T2 : notnull
        => Container.Get<T1, T2>();

}
