using System;
using System.Threading;
using MultiplayerMod.Core.Unity;
using MultiplayerMod.Test.Environment.Unity.Patches.Unity;
using Object = UnityEngine.Object;

namespace MultiplayerMod.Test.Environment.Unity;

public static class UnityPlayerObjectManager {

    private static int currentInstanceId;

    public static void Allocate(Object obj) {
        var pointer = UnityPlayerObject.CreateEmpty();
        obj.m_CachedPtr = pointer;
        // Since KObjectManager.GetOrCreateObject uses GameObject.GetInstanceID()
        // we have to provide different values for different objects.
        unsafe {
            var instanceId = (int*)IntPtr.Add(pointer, ObjectPatch.OffsetOfInstanceIDInCPlusPlusObject).ToPointer();
            *instanceId = currentInstanceId;
        }
        Interlocked.Increment(ref currentInstanceId);
    }

    public static void Release(Object obj) {
        UnityPlayerObject.Free(obj.m_CachedPtr);
        obj.m_CachedPtr = IntPtr.Zero;
    }

}
