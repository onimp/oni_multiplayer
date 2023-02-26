using System;

namespace MultiplayerMod.Network.Command {

    public interface IBinaryCommand : IDisposable {
        IntPtr GetPointer();
        long GetSize();
    }

}
