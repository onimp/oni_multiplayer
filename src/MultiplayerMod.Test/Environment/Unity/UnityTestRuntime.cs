using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using MultiplayerMod.Core.Scheduling;
using MultiplayerMod.Test.Environment.Patches;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MultiplayerMod.Test.Environment.Unity;

public static class UnityTestRuntime {

    private static readonly Harmony harmony = new("Unity.Test");

    private static readonly List<Type> unityPatches = typeof(UnityTestRuntime).Assembly.GetTypes()
        .Where(type => type.Namespace.StartsWith(typeof(UnityTestRuntime).Namespace + ".Patches"))
        .ToList();

    private static readonly Dictionary<Type, Dictionary<UnityEvent, MethodInfo>> eventMethodCache = new();

    public static readonly Dictionary<Component, GameObject> Components = new();
    public static readonly List<Component> EnabledComponents = new();
    public static readonly List<Component> StartAwaitingComponents = new();
    public static readonly List<Component> NewComponents = new();
    public static readonly Dictionary<GameObject, List<Component>> Objects = new();
    private static readonly Dictionary<int, string> objectNames = new();
    private static readonly Dictionary<GameObject, Vector3> objectPositions = new();
    private static int lastCell;

    public static int FrameCount { get; private set; }

    public static float RealtimeSinceStartup => FrameCount;

    private static readonly HashSet<Type> supportedComponents = new() {
        typeof(UnityTaskExecutor),
        typeof(LoadingOverlay),
        typeof(KMonoBehaviour),
        typeof(KPrefabID)
    };

    public static void Register(GameObject gameObject) {
        Objects[gameObject] = new List<Component>();
        SetName(gameObject, "New game object");
        AddComponent(gameObject, typeof(Transform));
        SetUpUniquePosition(gameObject);
    }

    private static void SetUpUniquePosition(GameObject gameObject) {
        var cell = lastCell++;
        var width = 40; // random
        var cellSizeInMeters = 100;  // random
        SetPosition(
            gameObject,
            new Vector3(cellSizeInMeters * (float) (cell % width), cellSizeInMeters * (float) (cell / width), 0.0f)
        );
    }

    public static void GetPosition(Transform transform, out Vector3 position) {
        position = objectPositions[transform.gameObject];
    }

    public static void SetPosition(GameObject gameObject, Vector3 position) => objectPositions[gameObject] = position;

    public static string GetName(Object obj) {
        return objectNames[obj.GetHashCode()];
    }

    public static void SetName(Object obj, string name) {
        if (objectNames.Values.Contains(name)) {
            SetName(obj, $"New {name}");
            return;
        }
        objectNames[obj.GetHashCode()] = name;
    }

    public static Component AddComponent(GameObject gameObject, Type type) {
        var component = (Component) Activator.CreateInstance(type, true);
        Objects[gameObject].Add(component);
        Components[component] = gameObject;
        NewComponents.Add(component);
        SetName(component, $"New {type}");
        Trigger(component, UnityEvent.Awake);
        return component;
    }

    public static void GetComponentFastPathFromComponent(
        Component component,
        Type type,
        IntPtr oneFurtherThanResultValue
    ) {
        GetComponentFastPath(component.gameObject, type, oneFurtherThanResultValue);
    }

    public static Component GetComponent(GameObject gameObject, Type type) =>
        Objects[gameObject].SingleOrDefault(component => component.GetType() == type);

    public static unsafe void GetComponentFastPath(GameObject gameObject, Type type, IntPtr oneFurtherThanResultValue) {
        var component = GetComponent(gameObject, type);

        if (component == null) return;

#pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
        var instanceIntPtr = (IntPtr) (&component);
#pragma warning restore CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
        var adjustedTargetIntPtr = IntPtr.Subtract(oneFurtherThanResultValue, 8);

        var sourceBytePtr = (byte*) instanceIntPtr.ToPointer();
        var targetBytePtr = (byte*) adjustedTargetIntPtr.ToPointer();

        for (var i = 0; i < 8; i++) {
            targetBytePtr[i] = sourceBytePtr[i];
        }
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

    public static void Install() => PatchesSetup.Install(harmony, unityPatches);

    public static void Uninstall() {
        PatchesSetup.Uninstall(harmony);
        FrameCount = 0;
        Components.Clear();
        EnabledComponents.Clear();
        StartAwaitingComponents.Clear();
        NewComponents.Clear();
        Objects.Clear();
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
