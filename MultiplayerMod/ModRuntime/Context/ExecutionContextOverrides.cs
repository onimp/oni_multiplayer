using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

namespace MultiplayerMod.ModRuntime.Context;

public class ExecutionContextOverrides {

    private readonly Stack<TrackedExecutionContext> stack = new();
    private int lastPushFrame;

    public ExecutionContext? Current => stack.Count > 0 ? stack.Peek().Target : null;

    public void Push(ExecutionContext context) {
        if (lastPushFrame < Time.frameCount) {
            if (stack.Count > 0)
                throw new ExecutionContextIntegrityFailureException(
                    $"Execution context override stack contains a context from a previous cycle.\n" +
                    $"{ExtractContextStack()}"
                );
        }
        stack.Push(new TrackedExecutionContext(context, new StackTrace(1)));
        lastPushFrame = Time.frameCount;
    }

    public void Pop() {
        if (stack.Count == 0)
            throw new ExecutionContextIntegrityFailureException("Execution context override stack is empty.");
        stack.Pop();
    }

    private string ExtractContextStack() {
        var lastIndex = stack.Count - 1;
        var messages = stack.Select(
            (it, i) => $"Execution context #{lastIndex - i} set {it.Origin.ToString().TrimStart()}"
        );
        var message = string.Join("\n", messages);
        return $"============== Context stack ==============\n" +
               $"{message}\n" +
               $"===========================================";
    }

    private record TrackedExecutionContext(ExecutionContext Target, StackTrace Origin);

}
