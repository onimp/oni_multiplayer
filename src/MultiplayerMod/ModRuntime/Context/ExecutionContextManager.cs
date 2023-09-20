using System.Threading;
using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;

namespace MultiplayerMod.ModRuntime.Context;

[Dependency, UsedImplicitly]
public class ExecutionContextManager {

    public ExecutionContext BaseContext { get; set; } = new(ExecutionLevel.System);

    public ExecutionContext Context => stack.Value.Current ?? BaseContext;

    private readonly ThreadLocal<ExecutionContextStack> stack = new(() => new ExecutionContextStack());

    public void EnterOverrideSection(ExecutionContext context) => stack.Value.Push(context);

    public void LeaveOverrideSection() => stack.Value.Pop();

}
