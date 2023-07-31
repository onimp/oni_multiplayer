using System.Runtime.CompilerServices;
using System.Threading;

namespace MultiplayerMod.Core.Patch.Context;

public class PatchContext {

    private static readonly PatchContext rootContext = new(true);

    private static readonly ThreadLocal<PatchContextGuard> holder = new(() => new PatchContextGuard(rootContext));

    public static PatchContext Current => holder.Value.Peek();
    public static PatchContext Global { get; set; } = new(true);

    public bool PatchesEnabled { get; private set; }

    public PatchContext(bool patchesEnabled) {
        PatchesEnabled = patchesEnabled;
    }

    public static void Use(PatchContext context, System.Action action) {
        Enter(context);
        try {
            action();
        } finally {
            Leave();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Enter(PatchContext context) => holder.Value.Push(context);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Leave() => holder.Value.Pop();

}
