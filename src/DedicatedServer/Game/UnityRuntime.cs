using System;
using System.Collections.Generic;
using UnityEngine;

namespace DedicatedServer.Game;

/// <summary>
/// Managed implementations for Unity InternalCall methods.
/// Cecil patcher inserts Call instructions to these methods in UnityEngine.CoreModule.dll.
/// </summary>
public static class UnityRuntime {

    private static readonly List<GameObject> AllGameObjects = new List<GameObject>();
    private static readonly Dictionary<IntPtr, GameObject> ComponentToGameObject = new Dictionary<IntPtr, GameObject>();
    private static readonly Dictionary<IntPtr, string> ObjectNames = new Dictionary<IntPtr, string>();
    private static int _nextId;

    public static void CreateGameObject(GameObject self, string name) {
        self.m_CachedPtr = new IntPtr(++_nextId);
        // GameObject inherits Component — .gameObject should return self
        ComponentToGameObject[self.m_CachedPtr] = self;
        AllGameObjects.Add(self);
    }

    public static Component AddComponent(GameObject self, Type componentType) {
        var comp = (Component)Activator.CreateInstance(componentType);
        comp.m_CachedPtr = new IntPtr(++_nextId);
        ComponentToGameObject[comp.m_CachedPtr] = self;
        return comp;
    }

    public static GameObject Find(string name) {
        for (var i = 0; i < AllGameObjects.Count; i++) {
            if (AllGameObjects[i].name == name)
                return AllGameObjects[i];
        }
        return null;
    }

    public static GameObject GetGameObject(Component self) {
        ComponentToGameObject.TryGetValue(self.m_CachedPtr, out var go);
        return go;
    }

    public static void CreateScriptableObject(ScriptableObject self) {
        self.m_CachedPtr = new IntPtr(++_nextId);
    }

    public static ScriptableObject CreateScriptableObjectInstanceFromType(Type type, bool applyDefaultsAndReset) {
        var obj = (ScriptableObject)Activator.CreateInstance(type);
        obj.m_CachedPtr = new IntPtr(++_nextId);
        return obj;
    }

    // --- Object name ---

    public static string GetName(UnityEngine.Object self) {
        ObjectNames.TryGetValue(self.m_CachedPtr, out var name);
        return name ?? "";
    }

    public static void SetName(UnityEngine.Object self, string name) {
        ObjectNames[self.m_CachedPtr] = name;
    }

    public static int GetInstanceID(UnityEngine.Object self) {
        return self.m_CachedPtr.ToInt32();
    }

    // --- Application ---

    private static readonly string _streamingAssetsPath =
        Environment.GetEnvironmentVariable("ONI_STREAMING_ASSETS") ?? "";

    public static string GetStreamingAssetsPath() => _streamingAssetsPath;
    public static string GetDataPath() => System.IO.Path.GetDirectoryName(_streamingAssetsPath) ?? "";
    public static string GetPersistentDataPath() => System.IO.Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".oni-server");
    public static string GetConsoleLogPath() => "";
    public static bool GetIsPlaying() => true;
    public static RuntimePlatform GetPlatform() => RuntimePlatform.LinuxPlayer;
    public static int GetProcessorCount() => Environment.ProcessorCount;
    public static int GetSystemMemorySize() => 8192;

    // --- Transform ---
    private static readonly Dictionary<IntPtr, Transform> ComponentToTransform = new Dictionary<IntPtr, Transform>();

    public static Transform GetTransform(Component self) {
        if (ComponentToTransform.TryGetValue(self.m_CachedPtr, out var t)) return t;
        // Create a stub Transform for this component's gameObject
        var transform = (Transform)Activator.CreateInstance(typeof(Transform), true);
        transform.m_CachedPtr = new IntPtr(++_nextId);
        ComponentToTransform[self.m_CachedPtr] = transform;
        return transform;
    }

    // --- Resources ---

    public static UnityEngine.Object[] FindObjectsOfTypeAll(Type type) => Array.Empty<UnityEngine.Object>();

    public static UnityEngine.Object LoadResource(string path, Type type) {
        // ScriptableObject-based singletons loaded via Resources.Load in game
        if (typeof(ScriptableObject).IsAssignableFrom(type)) {
            return ScriptableObject.CreateInstance(type);
        }
        return null;
    }

    // --- MonoBehaviour ---

    public static bool IsObjectMonoBehaviour(UnityEngine.Object obj) => obj is MonoBehaviour;

    public static Coroutine StartCoroutineManaged2(MonoBehaviour self, System.Collections.IEnumerator routine) {
        // Skip coroutines on headless server — no frame loop
        return null;
    }
}
