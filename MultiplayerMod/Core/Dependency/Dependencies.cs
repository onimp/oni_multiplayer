using System;
using System.Runtime.CompilerServices;

namespace MultiplayerMod.Core.Dependency;

public static class Dependencies {

    private static readonly DependencyContainer container = new();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Register<T>() where T : class => container.Register<T>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Register<I, T>() where T : class, I => container.Register<I, T>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Register<T>(T instance) where T : notnull => container.Register(instance);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Register<I, T>(T instance) where T : class, I => container.Register<T, T>(instance);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Resolve<T>() where T : notnull => container.Resolve<T>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Inject<T>(T instance) where T : notnull => container.Inject(instance);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object Resolve(Type type) => container.Resolve(type);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Get<T>() where T : notnull => container.Get<T>();

}
