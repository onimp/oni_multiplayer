using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using MultiplayerMod.Core.Extensions;
using MultiplayerMod.Core.Scheduling;
using UnityEngine;

namespace MultiplayerMod.Test.Environment.Unity;

public static class UnityTestRuntime {

    private static readonly Harmony harmony = new("Unity.Test");
    private static readonly List<Type> unityPatches = typeof(UnityTestRuntime).Assembly.GetTypes()
        .Where(type => type.Namespace == typeof(UnityTestRuntime).Namespace + ".Patches")
        .ToList();

    private static readonly Dictionary<Type, Dictionary<UnityEvent, MethodInfo>> eventMethodCache = new();

    public static readonly Dictionary<Component, GameObject> Components = new();
    public static readonly List<Component> EnabledComponents = new();
    public static readonly List<Component> StartAwaitingComponents = new();
    public static readonly List<Component> NewComponents = new();
    public static readonly Dictionary<GameObject, List<Component>> Objects = new();

    public static int FrameCount { get; private set; }

    private static readonly HashSet<Type> supportedComponents = new() {
        typeof(UnityTaskExecutor)
    };

    public static void Register(GameObject gameObject) {
        Objects[gameObject] = new List<Component>();
        AddComponent(gameObject, typeof(Transform));
    }

    public static Component AddComponent(GameObject gameObject, Type type) {
        var component = (Component) Activator.CreateInstance(type, true);
        Objects[gameObject].Add(component);
        Components[component] = gameObject;
        NewComponents.Add(component);
        Trigger(component, UnityEvent.Awake);
        return component;
    }

    public static GameObject GetGameObject(Component component) => Components[component];

    public static GameObject? Find(string name) => Objects.Keys.FirstOrDefault(it => it.name == name);

    public static void NextFrame() {
        FrameCount++;

        NewComponents.ForEach(
            it => {
                Trigger(it, UnityEvent.Enable);
                EnabledComponents.Add(it);
                StartAwaitingComponents.Add(it);
            }
        );
        NewComponents.Clear();

        StartAwaitingComponents.ForEach(it => Trigger(it, UnityEvent.Start));
        StartAwaitingComponents.Clear();

        EnabledComponents.ForEach(it => Trigger(it, UnityEvent.Update));
        EnabledComponents.ForEach(it => Trigger(it, UnityEvent.LateUpdate));
    }

    public static void Install() => unityPatches.ForEach(it => harmony.CreateClassProcessor(it).Patch());

    public static void Uninstall() {
        foreach (var methodBase in harmony.GetPatchedMethods()) {
            if (!methodBase.HasMethodBody())
                continue;
            var patches = Harmony.GetPatchInfo(methodBase);
            patches.Prefixes
                .Union(patches.Postfixes)
                .Union(patches.Transpilers)
                .Union(patches.Finalizers)
                .Where(it => it.owner == harmony.Id)
                .ForEach(it => harmony.Unpatch(methodBase, it.PatchMethod));
        }
    }

    private static void Trigger(Component component, UnityEvent @event) {
        var type = component.GetType();
        if (!supportedComponents.Contains(type))
            return;

        var method = GetEventMethod(type, @event);
        method?.Invoke(component, new object[] { });
    }

    private static MethodInfo? GetEventMethod(Type type, UnityEvent @event) {
        if (!eventMethodCache.TryGetValue(type, out var eventMethods)) {
            var method = AccessTools.Method(type, @event.MethodName, new Type[] { });
            var methods = new Dictionary<UnityEvent, MethodInfo> { [@event] = method };
            eventMethodCache[type] = methods;
            return method;
        }
        if (!eventMethods.TryGetValue(@event, out var eventMethod)) {
            var method = AccessTools.Method(type, @event.MethodName, new Type[] { });
            eventMethods[@event] = method;
            return method;
        }
        return eventMethod;
    }

}
