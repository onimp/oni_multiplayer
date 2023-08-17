using System;
using JetBrains.Annotations;

namespace MultiplayerMod.Core.Dependency;

public static class Dependencies {

    [UsedImplicitly]
    private static DependencyContainer container = new();

    public static void Register<T>() where T : class => container.Register<T>();
    public static void Register<I, T>() where T : class, I => container.Register<I, T>();
    public static T Register<T>(T instance) where T : notnull => container.Register(instance);
    public static T Register<I, T>(T instance) where T : class, I => container.Register<T, T>(instance);
    public static T Resolve<T>() where T : notnull => container.Resolve<T>();
    public static object Resolve(Type type) => container.Resolve(type);
    public static T Inject<T>(T instance) where T : notnull => container.Inject(instance);
    public static T Get<T>() where T : notnull => container.Get<T>();

}
