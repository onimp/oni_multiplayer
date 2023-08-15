using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace MultiplayerMod.Core.Patch.Context;

public class PatchContextGuard {

    private int lastPushUpdateCycle;
    private readonly Stack<TrackedPatchContext> stack = new();

    public PatchContextGuard(PatchContext rootContext) {
        stack.Push(new TrackedPatchContext(rootContext, new StackTrace(true)));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public PatchContext Peek() => stack.Peek().TargetContext;

    public void Push(PatchContext context) {
        if (lastPushUpdateCycle < Time.frameCount) {
            if (stack.Count != 1)
                throw new PatchContextIntegrityFailureException(
                    $"Patch context stack contains context from a previous cycle.\n" +
                    $"{ExtractContextStack()}"
                );
        }
        stack.Push(new TrackedPatchContext(context, new StackTrace(1)));
        lastPushUpdateCycle = Time.frameCount;
    }

    public void Pop() {
        stack.Pop();
        if (stack.Count == 0)
            throw new PatchContextIntegrityFailureException("Root patch context was evacuated");
    }

    private string ExtractContextStack() {
        var lastIndex = stack.Count - 1;
        var messages = stack.Select((it, i) => $"Context #{lastIndex - i} set {it.Origin.ToString().TrimStart()}");
        var message = string.Join("\n",messages);
        return $"============== Context stack ==============\n" +
               $"{message}\n" +
               $"===========================================";
    }

    private record TrackedPatchContext(PatchContext TargetContext, StackTrace Origin);

}
