using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
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

    public static readonly Dictionary<string, GameObject> Components = new();
    public static readonly List<Component> EnabledComponents = new();
    public static readonly List<Component> StartAwaitingComponents = new();
    public static readonly List<Component> NewComponents = new();
    public static readonly Dictionary<string, GameObjectCompanion> GameObjectCompanionData = new();
    private static readonly Dictionary<int, string> objectNames = new();
    private static int lastCell;

    public static int FrameCount { get; private set; }
    public static float Time => FrameCount;

    private static readonly HashSet<Type> supportedComponents = new() {
        typeof(UnityTaskExecutor),
        typeof(LoadingOverlay),
        typeof(KMonoBehaviour),
        typeof(KPrefabID)
    };

    public static void Register(GameObject gameObject) {
        SetName(gameObject, "New game object");
        GameObjectCompanionData[gameObject.name] = new GameObjectCompanion(
            new List<Component>(),
            GenerateUniquePosition(),
            gameObject
        );
        AddComponent(gameObject, typeof(Transform));
    }

    private static Vector3 GenerateUniquePosition() {
        var cell = ++lastCell;
        var width = 40; // random
        return new Vector3((float) (cell % width), (float) (cell / width), 0.0f);
    }

    public static void GetPosition(Transform transform, out Vector3 position) {
        position = GameObjectCompanionData[transform.gameObject.name].Position;
    }

    public static void SetPositionFromTransform(Transform transform, ref Vector3 position) =>
        SetPosition(transform.gameObject, position);

    public static void SetPosition(GameObject gameObject, Vector3 position) =>
        GameObjectCompanionData[gameObject.name].Position = position;

    public static string GetName(Object obj) {
        return objectNames[RuntimeHelpers.GetHashCode(obj)];
    }

    public static void SetName(Object obj, string desiredName, int? numberOfCollisions = null) {
        var name = desiredName;
        if (numberOfCollisions != null) {
            name += $" ({numberOfCollisions})";
        }
        while (objectNames.Values.Contains(name)) {
            numberOfCollisions = (numberOfCollisions ?? 0) + 1;
            name = $"{desiredName} ({numberOfCollisions})";
        }
        var hashCode = RuntimeHelpers.GetHashCode(obj);
        if (obj is GameObject && objectNames.ContainsKey(hashCode)) {
            var oldName = objectNames[hashCode];
            if (GameObjectCompanionData.ContainsKey(oldName)) {
                var oldData = GameObjectCompanionData[oldName];
                GameObjectCompanionData.Remove(oldName);
                GameObjectCompanionData[name] = oldData;
            }
        }
        objectNames[hashCode] = name;
    }

    public static Component AddComponent(GameObject gameObject, Type type) {
        var component = (Component) Activator.CreateInstance(type, true);
        SetName(component, $"New {type}");
        GameObjectCompanionData[gameObject.name].Components.Add(component);
        Components[component.name] = gameObject;
        NewComponents.Add(component);
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

    public static Component? GetComponent(GameObject gameObject, Type type) {
        var components = GameObjectCompanionData[gameObject.name].Components;
        var assignableTypes = components.Where(type.IsInstanceOfType).ToList();
        if (assignableTypes.Count == 1) {
            return assignableTypes.Single();
        }
        var exactTypes = assignableTypes.Where(component => type == component.GetType()).ToList();
        if (exactTypes.Count == 1) {
            return exactTypes.Single();
        }
        return exactTypes.Count > 0 ? exactTypes.First() : null;
    }

    public static Array GetComponentsInternal(GameObject gameObject, Type type) {
        var res = GameObjectCompanionData[gameObject.name].Components.Where(type.IsInstanceOfType).ToArray();
        var newArray = Array.CreateInstance(type, res.Length);
        Array.Copy(res, newArray, newArray.Length);
        return newArray;
    }

    public static Component? GetComponentInChildren(GameObject gameObject, Type type, bool includeInactive) =>
        GetComponent(gameObject, type);

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

    public static GameObject GetGameObject(Component component) => Components[component.name];

    public static GameObject? Find(string name) =>
        GameObjectCompanionData.Select(it => it.Value.GameObject).FirstOrDefault(it => it.name == name);

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
        lastCell = 0;
        Components.Clear();
        EnabledComponents.Clear();
        StartAwaitingComponents.Clear();
        NewComponents.Clear();
        GameObjectCompanionData.Clear();
        objectNames.Clear();
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

    public class GameObjectCompanion {

        public List<Component> Components { get; set; }
        public Vector3 Position { get; set; }

        public GameObject GameObject { get; }

        public GameObjectCompanion(List<Component> components, Vector3 position, GameObject gameObject) {
            Components = components;
            Position = position;
            GameObject = gameObject;
        }
    }
}
