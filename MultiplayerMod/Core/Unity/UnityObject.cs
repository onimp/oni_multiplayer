using System;
using System.Runtime.InteropServices;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MultiplayerMod.Core.Unity;

public static class UnityObject {

    private const int unityPlayerObjectSize = 0x48;
    private static readonly IntPtr unityPlayerNullObject;

    static UnityObject() {
        // Unity mono GC extension reads m_InstanceID from m_CachedPtr and to prevent access violation
        // we have to allocate UnityPlayer!Object and have 0 in m_InstanceID.
        //
        // More details:
        // GC fails at UnityPlayer.dll: mov eax,dword ptr [rax+8]
        // rax contains UnityPlayer!Object {0x048 bytes}:
        // ...
        //    +0x008 m_InstanceID     : Int4B
        // ...
        // if m_InstanceID is 0 then the calling function UnityPlayer.dll!RegisterFilteredObjectCallback should ignore
        // this UnityPlayer!Object.

        unityPlayerNullObject = Marshal.AllocHGlobal(unityPlayerObjectSize);
        var data = new byte[unityPlayerObjectSize];
        Marshal.Copy(data, 0, unityPlayerNullObject, unityPlayerObjectSize);
    }

    public static GameObject CreateStaticWithComponent<T>() where T : Component =>
        CreateWithComponents(false, typeof(T));

    public static GameObject CreateWithComponent<T1, T2>() where T1 : Component where T2 : Component =>
        CreateWithComponents(true, typeof(T1), typeof(T2));

    private static GameObject CreateWithComponents(bool destroyOnLoad, params Type[] components) {
        var gameObject = new GameObject(null, components);
        if (!destroyOnLoad)
            Object.DontDestroyOnLoad(gameObject);
        return gameObject;
    }

    public static void Destroy(GameObject gameObject) {
        Object.Destroy(gameObject);
    }

    public static T CreateStub<T>() where T : MonoBehaviour, new() => new() {
        m_CachedPtr = unityPlayerNullObject // Support for != and == for Unity objects
    };

}
