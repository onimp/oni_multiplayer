using JetBrains.Annotations;
using MultiplayerMod.Core.Logging;

namespace MultiplayerMod.ModRuntime.Context;

[UsedImplicitly]
public class ExecutionLevelManager {

    private static readonly Core.Logging.Logger log = LoggerFactory.GetLogger<ExecutionLevelManager>();

    private readonly ExecutionContextManager contextManager;

    public ExecutionLevelManager(ExecutionContextManager contextManager) {
        this.contextManager = contextManager;
    }

    /// <summary>
    /// Overrides execution level for subsequent code execution.
    /// Must be accompanied with <see cref="LeaveSection"/>.
    /// </summary>
    /// <param name="level">An execution level that will be set to current code execution</param>
    public void EnterSection(ExecutionLevel level) {
        contextManager.Push(new ExecutionContext(level));
        log.Trace(() => $"Entering execution level {contextManager.Context.Level}");
    }

    /// <summary>
    /// Restores overridden execution level to its previous value.
    /// This should be called after each call to <see cref="EnterSection"/>.
    /// </summary>
    public void LeaveSection() {
        contextManager.Pop();
        log.Trace(() => $"Entering execution level {contextManager.Context.Level}");
    }

    /// <summary>
    /// Runs an action if level requirements are satisfied.
    /// </summary>
    /// <param name="requiredLevel">An execution level required to run the action</param>
    /// <param name="action">An action</param>
    public void RunIfPossible(ExecutionLevel requiredLevel, System.Action action) {
        if (ExecutionLevelMatcher.Matches(contextManager.Context.Level, requiredLevel))
            action();
    }

    /// <summary>
    /// Runs an action if level requirements are satisfied.
    /// </summary>
    /// <param name="requiredLevel">An execution level required to run the action</param>
    /// <param name="actionLevel">An execution level that will be active during action execution</param>
    /// <param name="action">An action</param>
    public void RunIfPossible(ExecutionLevel requiredLevel, ExecutionLevel actionLevel, System.Action action) {
        if (ExecutionLevelMatcher.Matches(contextManager.Context.Level, requiredLevel))
            RunUsingLevel(actionLevel, action);
    }

    /// <summary>
    /// Runs an action with overridden execution level.
    /// </summary>
    /// <param name="level">An execution level that will be active during action execution</param>
    /// <param name="action">An action</param>
    public void RunUsingLevel(ExecutionLevel level, System.Action action) {
        try {
            EnterSection(level);
            action();
        } finally {
            LeaveSection();
        }
    }

    /// <summary>
    /// Replaces the current execution level with a new one. This is possible only for root (non-overridden) level.
    /// </summary>
    /// <param name="level">New execution level</param>
    /// <exception cref="ExecutionContextIntegrityFailureException">An attempt was made to replace an overridden context</exception>
    public void ReplaceLevel(ExecutionLevel level) {
        log.Trace(() => $"Replacing execution level {contextManager.Context.Level}");
        contextManager.Replace(new ExecutionContext(level));
    }

}
