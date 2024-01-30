using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using HarmonyLib;
using MultiplayerMod.Core.Extensions;
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

    private static readonly HashSet<Component> enabledComponents = new();
    private static readonly HashSet<Component> startAwaitingComponents = new();
    private static readonly HashSet<Component> newComponents = new();
    private static readonly Dictionary<IntPtr, ObjectCompanion> companionData = new();
    private static int lastCell;

    public static int FrameCount { get; private set; }
    public static float Time => FrameCount;

    private static readonly HashSet<Type> supportedComponents = new() {
        typeof(UnityTaskExecutor),
        typeof(LoadingOverlay),
        typeof(KMonoBehaviour),
        typeof(KPrefabID),
        typeof(PathProber),
        typeof(Facing),
        typeof(Notifier),
        typeof(Clearable),
        typeof(Prioritizable),
        typeof(Ownables)
    };

    public static void RegisterGameObject(GameObject gameObject) {
        SetCompanion(
            gameObject,
            new GameObjectCompanion(
                new List<Component>(),
                GenerateUniquePosition(),
                gameObject,
                "New game object"
            )
        );
        AddComponent(gameObject, typeof(Transform));
    }

    public static void RegisterObject(Object obj, GameObject? parent) {
        if (GetGameObjectCompanionSafe(obj) != null) {
            return;
        }
        SetCompanion(obj, new ObjectCompanion($"New {obj.GetType()}", parent));
    }

    private static Vector3 GenerateUniquePosition() {
        var cell = ++lastCell;
        var width = 40; // random
        return new Vector3(cell % width, (float) cell / width, 0.0f);
    }

    public static void GetPosition(Transform transform, out Vector3 position) {
        position = GetGameObjectCompanion(transform.gameObject).Position;
    }

    public static void SetPositionFromTransform(Transform transform, ref Vector3 position) =>
        SetPosition(transform.gameObject, position);

    public static void SetPosition(GameObject gameObject, Vector3 position) =>
        GetGameObjectCompanion(gameObject).Position = position;

    public static string GetName(Object obj) =>
        GetGameObjectCompanionSafe(obj)?.Name ?? $"Error-{obj.GetType()}-{obj.m_CachedPtr}";

    public static void SetName(Object obj, string desiredName) => GetGameObjectCompanion(obj).Name = desiredName;

    public static Component AddComponent(GameObject gameObject, Type type) {
        var component = (Component) Activator.CreateInstance(type, true);
        GetGameObjectCompanion(component).Parent = gameObject;
        GetGameObjectCompanion(gameObject).Components.Add(component);
        newComponents.Add(component);
        Trigger(component, UnityEvent.Awake);
        return component;
    }

    public static Object Clone(Object original) {
        var newObject = (Object) Activator.CreateInstance(original.GetType(), true);
        if (original is GameObject gameObject) {
            var companion = GetGameObjectCompanion(gameObject);
            foreach (var component in companion.Components) {
                AddComponent((GameObject)newObject, component.GetType());
            }
        }
        return newObject;
    }

    public static void Destroy(Object obj) {
        switch (obj) {
            case null:
                return;
            case Component component: {
                var parent = GetGameObjectCompanion(component).Parent!;
                GetGameObjectCompanion(parent).Components.Remove(component);
                if (!newComponents.Remove(component)) {
                    throw new Exception($"Can not remove {component.GetType()}-{component.m_CachedPtr}");
                }
                RemoveCompanion(obj);
                return;
            }
            case GameObject gameObject: {
                var companion = GetGameObjectCompanion(gameObject);
                companion.Components.ForEach(
                    it => {
                        if (!newComponents.Remove(it)) {
                            throw new Exception($"Can not remove {it.GetType()}-{it.m_CachedPtr}");
                        }
                        RemoveCompanion(it);
                    }
                );
                RemoveCompanion(obj);
                return;
            }
            default:
                throw new Exception($"Unknown thing to destroy {obj.GetType()}-{obj.m_CachedPtr}");
        }
    }

    public static void GetComponentFastPathFromComponent(
        Component component,
        Type type,
        IntPtr oneFurtherThanResultValue
    ) {
        GetComponentFastPath(component.gameObject, type, oneFurtherThanResultValue);
    }

    public static Component? GetComponent(GameObject gameObject, Type type) {
        var components = GetGameObjectCompanion(gameObject).Components;
        var assignableTypes = components.Where(type.IsInstanceOfType).ToList();
        if (assignableTypes.Count == 1) {
            return assignableTypes.Single();
        }
        var exactTypes = assignableTypes.Where(component => type == component.GetType()).ToList();
        return exactTypes.Count > 0 ? exactTypes.First() : assignableTypes.FirstOrDefault();
    }

    public static Array GetComponentsInternal(GameObject gameObject, Type type) {
        var res = GetGameObjectCompanion(gameObject).Components.Where(type.IsInstanceOfType).ToArray();
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

    public static GameObject? GetGameObject(Component component) => GetGameObjectCompanion(component).Parent;

    public static GameObject? Find(string name) =>
        companionData
            .Where(it => it.Value is GameObjectCompanion)
            .Select(it => ((GameObjectCompanion) it.Value).GameObject)
            .FirstOrDefault(it => it.name == name);

    public static void NextFrame() {
        FrameCount++;

        newComponents.ForEach(
            it => {
                Trigger(it, UnityEvent.Enable);
                enabledComponents.Add(it);
                startAwaitingComponents.Add(it);
            }
        );
        newComponents.Clear();

        startAwaitingComponents.ForEach(it => Trigger(it, UnityEvent.Start));
        startAwaitingComponents.Clear();

        enabledComponents.ForEach(it => Trigger(it, UnityEvent.Update));
        enabledComponents.ForEach(it => Trigger(it, UnityEvent.LateUpdate));
    }

    public static void Install() => PatchesSetup.Install(harmony, unityPatches);

    public static void Uninstall() {
        PatchesSetup.Uninstall(harmony);
        FrameCount = 0;
        lastCell = 0;
        enabledComponents.Clear();
        startAwaitingComponents.Clear();
        newComponents.Clear();
        companionData.Clear();
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

    private static GameObjectCompanion? GetGameObjectCompanionSafe(GameObject obj) {
        return (GameObjectCompanion) companionData.GetValueSafe(GenerateKey(obj));
    }

    private static ObjectCompanion? GetGameObjectCompanionSafe(Object obj) {
        return companionData.GetValueSafe(GenerateKey(obj));
    }

    private static GameObjectCompanion GetGameObjectCompanion(GameObject obj) {
        var gameObjectCompanion = GetGameObjectCompanionSafe(obj);
        if (gameObjectCompanion == null) {
            throw new Exception($"No companion found for {obj.GetType()}-{obj.m_CachedPtr}");
        }
        return gameObjectCompanion;
    }

    private static ObjectCompanion GetGameObjectCompanion(Object obj) {
        var gameObjectCompanion = GetGameObjectCompanionSafe(obj);
        if (gameObjectCompanion == null) {
            throw new Exception($"No companion found for {obj.GetType()}-{obj.m_CachedPtr}");
        }
        return gameObjectCompanion;
    }

    private static void SetCompanion(Object obj, ObjectCompanion companion) {
        var key = GenerateKey(obj);
        if (companionData.ContainsKey(key)) {
            throw new Exception("Trying to override companion data");
        }
        companionData[key] = companion;
    }

    private static void SetCompanion(GameObject obj, GameObjectCompanion companion) {
        var key = GenerateKey(obj);
        if (companionData.ContainsKey(key)) {
            throw new Exception("Trying to override companion data");
        }
        companionData[key] = companion;
    }

    private static void RemoveCompanion(Object obj) {
        companionData.Remove(GenerateKey(obj));
        UnityPlayerObjectManager.Release(obj);
    }

    private static IntPtr GenerateKey(Object obj) {
        return obj.m_CachedPtr;
    }

    private class ObjectCompanion {
        public string Name { get; set; }

        public GameObject? Parent { get; set; }

        public ObjectCompanion(string name, GameObject? parent) {
            Name = name;
            Parent = parent;
        }

    }

    private class GameObjectCompanion : ObjectCompanion {

        public List<Component> Components { get; }
        public Vector3 Position { get; set; }

        public GameObject GameObject { get; }

        public GameObjectCompanion(
            List<Component> components,
            Vector3 position,
            GameObject gameObject,
            string name
        ) : base(name, null!) {
            Components = components;
            Position = position;
            GameObject = gameObject;
        }
    }
}
