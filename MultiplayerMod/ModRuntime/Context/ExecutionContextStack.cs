using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

namespace MultiplayerMod.ModRuntime.Context;

public class ExecutionContextStack {

    private readonly Stack<TrackedExecutionContext> stack = new();
    private int lastPushFrame;

    public ExecutionContext Current => stack.Peek().Target;

    public ExecutionContextStack() {
        stack.Push(new TrackedExecutionContext(new ExecutionContext(ExecutionLevel.System), new StackTrace()));
    }

    public void Push(ExecutionContext context) {
        if (lastPushFrame < Time.frameCount) {
            if (stack.Count > 1)
                throw new ExecutionContextIntegrityFailureException(
                    $"Execution context stack contains a context from a previous cycle\n{ExtractContextStack()}"
                );
        }
        stack.Push(new TrackedExecutionContext(context, new StackTrace(1)));
        lastPushFrame = Time.frameCount;
    }

    public void Pop() {
        if (stack.Count == 1)
            throw new ExecutionContextIntegrityFailureException(
                "An attempt was made to evacuate the root execution context"
            );

        stack.Pop();
    }

    public void Replace(ExecutionContext context) {
        if (stack.Count != 1)
            throw new ExecutionContextIntegrityFailureException(
                $"An attempt was made to replace a non root execution context\n{ExtractContextStack()}"
            );

        stack.Pop();
        Push(context);
    }

    public IReadOnlyList<ExecutionContext> Get() =>
        new ReadOnlyCollection<ExecutionContext>(stack.Select(it => it.Target).Reverse().ToList());

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
