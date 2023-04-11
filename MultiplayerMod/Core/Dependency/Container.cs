﻿using System;
using System.Collections.Concurrent;

namespace MultiplayerMod.Core.Dependency;

public abstract class Container {

    private static readonly ConcurrentDictionary<Type, Lazy<object>> instances = new();

    public static T Get<T>() {
        var singleton = instances[typeof(T)];
        if (singleton == null)
            throw new ContainerException($"Type {typeof(T).FullName} not registered");
        return (T) singleton.Value;
    }

    public static void Register<T>() where T : class, new() => TryAdd<T, T>();

    public static void Register<T>(T instance) => TryAdd(instance);

    public static void Register<I, T>() where T : class, I, new() => TryAdd<I, T>();

    private static void TryAdd<T>(T instance) {
        if (!instances.TryAdd(typeof(T), new Lazy<object>(() => instance)))
            throw new ContainerException($"A type {typeof(T).FullName} is already registered");
    }

    private static void TryAdd<I, T>() where T : class, I, new() {
        if (!instances.TryAdd(typeof(I), new Lazy<object>(() => new T())))
            throw new ContainerException($"A type {typeof(I).FullName} is already registered");
    }

}
