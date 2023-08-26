using System.Threading;
using JetBrains.Annotations;

namespace MultiplayerMod.ModRuntime.Context;

[UsedImplicitly]
public class ExecutionContextManager {

    public ExecutionContext Context => stack.Value.Current;

    private readonly ThreadLocal<ExecutionContextStack> stack = new(() => new ExecutionContextStack());

    public void Push(ExecutionContext context) => stack.Value.Push(context);

    public void Pop() => stack.Value.Pop();

    public void Replace(ExecutionContext context) => stack.Value.Replace(context);

}
