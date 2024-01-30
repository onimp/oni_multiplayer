using System;
using System.Runtime.InteropServices;

namespace MultiplayerMod.Core.Unity;

public static class UnityPlayerObject {

    private const int unityPlayerObjectSize = 0x48;

    public static IntPtr CreateEmpty() {
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

        var res = Marshal.AllocHGlobal(unityPlayerObjectSize);
        var data = new byte[unityPlayerObjectSize];
        Marshal.Copy(data, 0, res, unityPlayerObjectSize);
        return res;
    }

    public static void Free(IntPtr unityPlayerObject) {
        Marshal.FreeHGlobal(unityPlayerObject);
    }

}
