using System.Threading;
using JetBrains.Annotations;

namespace MultiplayerMod.ModRuntime.Context;

[UsedImplicitly]
public class ExecutionContextManager {

    public ExecutionContext MainContext { get; set; } = new(ExecutionLevel.System);

    private readonly ThreadLocal<ExecutionContextOverrides> overrides = new(() => new ExecutionContextOverrides());

    public ExecutionContext EffectiveContext => overrides.Value.Current ?? MainContext;

    public void UsingContext(ExecutionContext context, System.Action action) {
        OverrideContext(context);
        try {
            action();
        } finally {
            RestoreContext();
        }
    }

    public void OverrideContext(ExecutionContext context) => overrides.Value.Push(context);

    public void RestoreContext() => overrides.Value.Pop();

}
