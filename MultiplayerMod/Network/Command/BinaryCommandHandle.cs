using System;

namespace MultiplayerMod.Network.Command {

    public class BinaryCommandHandle : IBinaryCommand {

        private IntPtr pointer;
        private long size;

        public BinaryCommandHandle(IntPtr pointer, long size) {
            this.pointer = pointer;
            this.size = size;
        }

        public void Dispose() {
            // No disposal required
        }

        public IntPtr GetPointer() => pointer;

        public long GetSize() => size;

    }

}
