using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DedicatedServer.Game;

/// <summary>
/// Managed implementations for Unity InternalCall methods.
/// Harmony patches redirect Unity InternalCalls to these methods.
/// Provides component tracking, lifecycle management, and object identity.
/// </summary>
public static class UnityRuntime {

    // --- Object identity ---
    private static int _nextId;
    private static int NextId() => Interlocked.Increment(ref _nextId);

    // --- Component tracking ---
    // Every GameObject has a list of components. Every component knows its parent GO.
    private static readonly Dictionary<IntPtr, List<Component>> GameObjectComponents = new();
    private static readonly Dictionary<IntPtr, GameObject> ComponentToGameObject = new();
    private static readonly Dictionary<IntPtr, string> ObjectNames = new();
    private static readonly Dictionary<IntPtr, Vector3> ObjectPositions = new();
    private static readonly HashSet<IntPtr> ActiveObjects = new();
    private static readonly HashSet<IntPtr> SpawnedObjects = new();

    // --- Stats ---
    public static int TotalGameObjects => GameObjectComponents.Count;
    public static int TotalComponents => ComponentToGameObject.Count;

    // ==================== GameObject ====================

    public static void CreateGameObject(GameObject self, string name) {
        self.m_CachedPtr = new IntPtr(NextId());
        GameObjectComponents[self.m_CachedPtr] = new List<Component>();
        ComponentToGameObject[self.m_CachedPtr] = self; // GO is also a Component
        ObjectNames[self.m_CachedPtr] = name ?? "";
        // Every GameObject gets a Transform
        AddComponent(self, typeof(Transform));
    }

    public static Component AddComponent(GameObject self, Type componentType) {
        Component comp;
        try {
            comp = (Component) Activator.CreateInstance(componentType, true);
        } catch {
            // Some components have no parameterless ctor — create uninitialized
            comp = (Component) System.Runtime.Serialization.FormatterServices
                .GetUninitializedObject(componentType);
        }
        comp.m_CachedPtr = new IntPtr(NextId());
        ComponentToGameObject[comp.m_CachedPtr] = self;

        if (GameObjectComponents.TryGetValue(self.m_CachedPtr, out var components))
            components.Add(comp);

        return comp;
    }

    public static Component GetComponent(GameObject self, Type type) {
        if (!GameObjectComponents.TryGetValue(self.m_CachedPtr, out var components))
            return null;

        Component exact = null;
        foreach (var c in components) {
            if (type.IsInstanceOfType(c)) {
                if (c.GetType() == type) return c; // exact match = best
                exact ??= c; // assignable match = fallback
            }
        }
        return exact;
    }

    public static Array GetComponentsInternal(GameObject self, Type type) {
        if (!GameObjectComponents.TryGetValue(self.m_CachedPtr, out var components))
            return Array.CreateInstance(type, 0);

        var matches = components.Where(type.IsInstanceOfType).ToArray();
        var result = Array.CreateInstance(type, matches.Length);
        Array.Copy(matches, result, matches.Length);
        return result;
    }

    public static Component GetComponentInChildren(GameObject self, Type type, bool includeInactive) {
        return GetComponent(self, type);
    }

    public static unsafe void GetComponentFastPath(
        GameObject self, Type type, IntPtr oneFurtherThanResultValue) {
        var component = GetComponent(self, type);
        if (component == null) return;

#pragma warning disable CS8500
        var instanceIntPtr = (IntPtr) (&component);
#pragma warning restore CS8500
        var adjustedTargetIntPtr = IntPtr.Subtract(oneFurtherThanResultValue, 8);

        var sourceBytePtr = (byte*) instanceIntPtr.ToPointer();
        var targetBytePtr = (byte*) adjustedTargetIntPtr.ToPointer();
        for (var i = 0; i < 8; i++)
            targetBytePtr[i] = sourceBytePtr[i];
    }

    public static unsafe void GetComponentFastPathFromComponent(
        Component self, Type type, IntPtr oneFurtherThanResultValue) {
        var go = GetGameObject(self);
        if (go != null)
            GetComponentFastPath(go, type, oneFurtherThanResultValue);
    }

    public static GameObject Find(string name) {
        foreach (var kvp in GameObjectComponents) {
            if (ObjectNames.TryGetValue(kvp.Key, out var n) && n == name) {
                // Find the GO by its ptr
                if (ComponentToGameObject.TryGetValue(kvp.Key, out var go))
                    return go;
            }
        }
        return null;
    }

    public static void SetActive(GameObject self, bool value) {
        if (value) {
            ActiveObjects.Add(self.m_CachedPtr);
            // Trigger lifecycle on activation (like Unity does)
            TriggerLifecycle(self);
        } else {
            ActiveObjects.Remove(self.m_CachedPtr);
        }
    }

    public static bool GetActive(GameObject self) {
        return ActiveObjects.Contains(self.m_CachedPtr);
    }

    public static void SetLayer(GameObject self, int layer) {
        // No-op in headless
    }

    // ==================== Component ====================

    public static GameObject GetGameObject(Component self) {
        ComponentToGameObject.TryGetValue(self.m_CachedPtr, out var go);
        return go;
    }

    public static Transform GetTransformFromComponent(Component self) {
        var go = GetGameObject(self);
        return go != null ? (Transform) GetComponent(go, typeof(Transform)) : null;
    }

    public static Transform GetTransformFromGameObject(GameObject self) {
        return (Transform) GetComponent(self, typeof(Transform));
    }

    // ==================== Object ====================

    public static string GetName(Object self) {
        ObjectNames.TryGetValue(self.m_CachedPtr, out var name);
        return name ?? "";
    }

    public static void SetName(Object self, string name) {
        ObjectNames[self.m_CachedPtr] = name ?? "";
    }

    public static int GetInstanceID(Object self) {
        return self.m_CachedPtr.ToInt32();
    }

    public static void ObjectConstructor(Object obj) {
        if (obj.m_CachedPtr == IntPtr.Zero)
            obj.m_CachedPtr = new IntPtr(NextId());
        if (obj is GameObject go) {
            if (!GameObjectComponents.ContainsKey(go.m_CachedPtr)) {
                GameObjectComponents[go.m_CachedPtr] = new List<Component>();
                ComponentToGameObject[go.m_CachedPtr] = go;
            }
        }
    }

    // --- Instantiate (clone) ---

    public static Object CloneSingle(Object data) {
        if (data == null) return null;

        if (data is GameObject srcGo) {
            // Clone GameObject: create new GO, then clone each component preserving field values.
            // Unity's Instantiate copies all serialized fields — we replicate this by MemberwiseClone
            // each component, then fix up its identity (m_CachedPtr) and owner (ComponentToGameObject).
            // This is critical: without field copying, prefab-configured fields like Navigator.NavGridName
            // are null on the clone, breaking OnPrefabInit (GetNavGrid("") returns null → NPE).
            var clone = new GameObject();
            clone.name = srcGo.name;
            if (GameObjectComponents.TryGetValue(srcGo.m_CachedPtr, out var srcComponents)) {
                var cloneComponents = GameObjectComponents[clone.m_CachedPtr];
                // Remove the stub Transform that CreateGameObject added — we'll clone the real one
                cloneComponents.Clear();
                foreach (var srcComp in srcComponents) {
                    // MemberwiseClone copies all fields (including NavGridName, NavType, etc.)
                    var cloneComp = (Component) srcComp.GetType()
                        .GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic)!
                        .Invoke(srcComp, null);
                    cloneComp.m_CachedPtr = new IntPtr(NextId());
                    ComponentToGameObject[cloneComp.m_CachedPtr] = clone;
                    cloneComponents.Add(cloneComp);
                }
            }
            // Copy position
            if (ObjectPositions.TryGetValue(srcGo.m_CachedPtr, out var pos))
                ObjectPositions[clone.m_CachedPtr] = pos;
            return clone;
        }

        // Non-GameObject: MemberwiseClone
        var cloneObj = (Object) data.GetType()
            .GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic)!
            .Invoke(data, null);
        cloneObj.m_CachedPtr = new IntPtr(NextId());
        return cloneObj;
    }

    public static Object CloneSingleWithParent(Object data, Transform parent, bool worldPositionStays) {
        return CloneSingle(data);
    }

    public static Object CloneSingleWithParams(Object data, Vector3 pos, Quaternion rot) {
        var clone = CloneSingle(data);
        if (clone is GameObject go)
            ObjectPositions[go.m_CachedPtr] = pos;
        return clone;
    }

    public static Object InstantiateSingleWithParent(Object data, Transform parent, Vector3 pos, Quaternion rot) {
        return CloneSingleWithParams(data, pos, rot);
    }

    public static bool IsNativeObjectAlive(Object o) {
        return o != null && o.m_CachedPtr != IntPtr.Zero;
    }

    // --- Destroy ---

    public static void DestroyObject(Object obj, float t) {
        DestroyInternal(obj);
    }

    public static void DestroyImmediate(Object obj, bool allowDestroyingAssets) {
        DestroyInternal(obj);
    }

    private static void DestroyInternal(Object obj) {
        if (obj == null) return;
        if (obj is GameObject go && GameObjectComponents.TryGetValue(go.m_CachedPtr, out var comps)) {
            foreach (var c in comps.ToList()) {
                ComponentToGameObject.Remove(c.m_CachedPtr);
                ObjectNames.Remove(c.m_CachedPtr);
            }
            GameObjectComponents.Remove(go.m_CachedPtr);
            ActiveObjects.Remove(go.m_CachedPtr);
            SpawnedObjects.Remove(go.m_CachedPtr);
        }
        ComponentToGameObject.Remove(obj.m_CachedPtr);
        ObjectNames.Remove(obj.m_CachedPtr);
    }

    // ==================== ScriptableObject ====================

    public static void CreateScriptableObject(ScriptableObject self) {
        if (self.m_CachedPtr == IntPtr.Zero)
            self.m_CachedPtr = new IntPtr(NextId());
    }

    public static ScriptableObject CreateScriptableObjectInstanceFromType(Type type, bool applyDefaultsAndReset) {
        var obj = (ScriptableObject) Activator.CreateInstance(type);
        if (obj.m_CachedPtr == IntPtr.Zero)
            obj.m_CachedPtr = new IntPtr(NextId());
        return obj;
    }

    // ==================== Application ====================

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

    // ==================== Transform ====================

    public static void GetPosition(Transform self, out Vector3 result) {
        var go = GetGameObject(self);
        if (go != null && ObjectPositions.TryGetValue(go.m_CachedPtr, out var pos))
            result = pos;
        else
            result = Vector3.zero;
    }

    public static void SetPositionFromTransform(Transform self, ref Vector3 position) {
        var go = GetGameObject(self);
        if (go != null)
            ObjectPositions[go.m_CachedPtr] = position;
    }

    public static void SetPosition(GameObject go, Vector3 position) {
        ObjectPositions[go.m_CachedPtr] = position;
    }

    public static void SetParent(Transform self, Transform parent, bool worldPositionStays) {
        // No-op in headless
    }

    // ==================== TextAsset ====================

    private static readonly Dictionary<IntPtr, string> TextAssetContent = new();

    public static void CreateTextAsset(Object self, string text) {
        if (self.m_CachedPtr == IntPtr.Zero)
            self.m_CachedPtr = new IntPtr(NextId());
        TextAssetContent[self.m_CachedPtr] = text ?? "";
    }

    public static string GetTextAssetText(TextAsset self) {
        TextAssetContent.TryGetValue(self.m_CachedPtr, out var text);
        return text ?? "";
    }

    public static byte[] GetTextAssetBytes(TextAsset self) {
        TextAssetContent.TryGetValue(self.m_CachedPtr, out var text);
        return System.Text.Encoding.UTF8.GetBytes(text ?? "");
    }

    // ==================== Resources ====================

    public static Object[] FindObjectsOfTypeAll(Type type) => Array.Empty<Object>();

    public static Object[] FindObjectsOfType(Type type, bool includeInactive) {
        return Array.Empty<Object>();
    }

    public static Object LoadResource(string path, Type type) {
        if (typeof(ScriptableObject).IsAssignableFrom(type))
            return ScriptableObject.CreateInstance(type);
        return null;
    }

    // ==================== Behaviour ====================

    public static bool GetIsActiveAndEnabled(Behaviour self) => true;
    public static bool GetEnabled(Behaviour self) => true;
    public static void SetEnabled(Behaviour self, bool value) { }

    // ==================== MonoBehaviour ====================

    public static bool IsObjectMonoBehaviour(Object obj) => obj is MonoBehaviour;

    public static Coroutine StartCoroutineManaged(MonoBehaviour self, IEnumerator routine) {
        // No coroutines in headless — just run synchronously if simple
        return null;
    }

    // ==================== DontDestroyOnLoad ====================

    public static void DontDestroyOnLoad(Object target) { }

    // ==================== Misc ====================

    public static int GetOffsetOfInstanceIDInCPlusPlusObject() => 0x08;

    public static string ObjectToString(Object obj) {
        return (obj.name ?? "") + obj.GetHashCode();
    }

    // ==================== Lifecycle ====================

    /// <summary>
    /// Triggers the KMonoBehaviour lifecycle (Awake→OnPrefabInit, Start→OnSpawn) on a GameObject.
    /// Called when SetActive(true) or manually after KInstantiate.
    /// </summary>
    public static void TriggerLifecycle(GameObject go) {
        if (SpawnedObjects.Contains(go.m_CachedPtr)) return;
        SpawnedObjects.Add(go.m_CachedPtr);

        if (!GameObjectComponents.TryGetValue(go.m_CachedPtr, out var components)) return;

        // Phase 1: Awake (InitializeComponent → OnPrefabInit)
        var snapshot = components.ToList(); // snapshot — components may be added during Awake
        foreach (var comp in snapshot) {
            if (comp is KMonoBehaviour kmb) {
                try { kmb.InitializeComponent(); }
                catch (Exception ex) {
                    Console.WriteLine($"[Lifecycle] Awake failed [{comp.GetType().Name}]: {ex.GetBaseException().Message}");
                }
            }
        }

        // Phase 2: Start (Spawn → OnSpawn)
        snapshot = components.ToList(); // re-snapshot after Awake may have added more
        foreach (var comp in snapshot) {
            if (comp is KMonoBehaviour kmb) {
                try { kmb.Spawn(); }
                catch (Exception ex) {
                    Console.WriteLine($"[Lifecycle] Start failed [{comp.GetType().Name}]: {ex.GetBaseException().Message}");
                }
            }
        }
    }

    /// <summary>
    /// Creates a stub UnityEngine.Object of the given type without calling its constructor.
    /// Assigns a valid m_CachedPtr so name/identity operations work.
    /// </summary>
    // ==================== KAnim stubs ====================

    private static KAnimFile _stubAnim;

    /// <summary>Returns a stub KAnimFile for headless use when a real asset isn't available.</summary>
    public static KAnimFile GetStubAnim() {
        if (_stubAnim != null) return _stubAnim;
        var stub = CreateStub<KAnimFile>();
        _stubAnim = stub;
        return stub;
    }

    public static T CreateStub<T>() where T : Object {
        var obj = (T) System.Runtime.Serialization.FormatterServices.GetUninitializedObject(typeof(T));
        obj.m_CachedPtr = new IntPtr(NextId());
        return obj;
    }

    /// <summary>
    /// Install Harmony patches to redirect Unity InternalCalls to this runtime.
    /// Must be called BEFORE any Unity code executes.
    /// </summary>
    public static void Install() {
        // InternalCall methods in UnityEngine.CoreModule.dll are pre-patched at build time
        // by PatchInternalCalls (Mono.Cecil) — they already delegate to UnityRuntime statics.
        // Harmony runtime patches on InternalCalls hang on Mono (no IL body → native detour blocks).
        // Only apply Harmony patches for purely managed methods (non-InternalCall).
        var harmony = new HarmonyLib.Harmony("DedicatedServer.UnityRuntime");
        // Discover all patch classes in the Patches namespace
        var patchTypes = typeof(UnityRuntime).Assembly.GetTypes()
            .Where(t => t.Namespace?.StartsWith("DedicatedServer.Game.Patches") == true)
            .ToList();

        // Skip patch classes that target Unity InternalCall methods — pre-patched by PatchInternalCalls.
        // Harmony on InternalCall stubs causes a deadlock on Mono (no IL body → native detour blocks).
        // TransformPatches is included: get/set_position_Injected are patched by PatchInternalCalls
        //   callMap → GetPosition/SetPositionFromTransform (Cecil, build-time, no Harmony needed).
        var skipTypes = new HashSet<string> {
            "ApplicationPatches", "GameObjectPatches", "ComponentPatches",
            "TransformPatches", "ObjectPatches", "BehaviourPatches",
            "MonoBehaviourPatches", "ScriptableObjectPatches", "TextAssetPatches",
            "RandomPatches", "DebugPatches", "SystemInfoPatches"
        };

        int applied = 0, skipped = 0;
        foreach (var type in patchTypes) {
            if (skipTypes.Contains(type.Name)) {
                skipped++;
                continue;
            }
            Console.WriteLine($"[UnityRuntime] Patching {type.Name}...");
            Console.Out.Flush();
            try {
                harmony.CreateClassProcessor(type).Patch();
                applied++;
            } catch (Exception ex) {
                Console.WriteLine($"[UnityRuntime] Patch failed [{type.Name}]: {ex.GetBaseException().Message}");
            }
        }
        Console.WriteLine($"[UnityRuntime] Installed {applied} patch classes ({skipped} skipped — pre-patched by PatchInternalCalls)");
    }
}
