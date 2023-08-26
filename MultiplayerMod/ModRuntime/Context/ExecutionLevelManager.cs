using JetBrains.Annotations;

namespace MultiplayerMod.ModRuntime.Context;

[UsedImplicitly]
public class ExecutionLevelManager {

    private readonly ExecutionContextManager contextManager;

    public ExecutionLevelManager(ExecutionContextManager contextManager) {
        this.contextManager = contextManager;
    }

    public void EnterSection(ExecutionLevel level) => contextManager.Push(new ExecutionContext(level));

    public void LeaveSection() => contextManager.Pop();

    public void RunUsingLevel(ExecutionLevel level, System.Action action) {
        try {
            EnterSection(level);
            action();
        } finally {
            LeaveSection();
        }
    }

    public void ReplaceLevel(ExecutionLevel level) => contextManager.Replace(new ExecutionContext(level));

}
