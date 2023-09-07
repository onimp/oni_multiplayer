using JetBrains.Annotations;
using MultiplayerMod.Core.Logging;

namespace MultiplayerMod.ModRuntime.Context;

[UsedImplicitly]
public class ExecutionLevelManager {

    private static readonly Core.Logging.Logger log = LoggerFactory.GetLogger<ExecutionLevelManager>();
    private readonly ExecutionContextManager contextManager;

    public ExecutionLevel BaseLevel {
        get => contextManager.BaseContext.Level;
        set {
            log.Trace(() => $"Changing base execution level to {value}");
            contextManager.BaseContext = new ExecutionContext(value);
        }
    }

    public ExecutionLevel CurrentLevel => contextManager.Context.Level;

    public ExecutionLevelManager(ExecutionContextManager contextManager) {
        this.contextManager = contextManager;
    }

    /// <summary>
    /// Overrides execution level for subsequent code execution.
    /// Must be accompanied with <see cref="LeaveOverrideSection"/>.
    /// </summary>
    /// <param name="level">An execution level that will be set to current code execution</param>
    public void EnterOverrideSection(ExecutionLevel level) {
        log.Trace(() => $"Overriding execution level {CurrentLevel} -> {level}");
        contextManager.EnterOverrideSection(new ExecutionContext(level));
    }

    /// <summary>
    /// Restores overridden execution level to its previous value.
    /// This should be called after each call to <see cref="EnterOverrideSection"/>.
    /// </summary>
    public void LeaveOverrideSection() {
        contextManager.LeaveOverrideSection();
        log.Trace(() => $"Execution level was restored to {CurrentLevel}");
    }

    /// <summary>
    /// Runs an action if level requirements are satisfied.
    /// </summary>
    /// <param name="requiredLevel">An execution level required to run the action</param>
    /// <param name="action">An action</param>
    public void RunIfLevelIsActive(ExecutionLevel requiredLevel, System.Action action) {
        if (LevelIsActive(requiredLevel))
            action();
    }

    /// <summary>
    /// Runs an action if level requirements are satisfied.
    /// </summary>
    /// <param name="requiredLevel">An execution level required to run the action</param>
    /// <param name="actionLevel">An execution level that will be active during action execution</param>
    /// <param name="action">An action</param>
    public void RunIfLevelIsActive(ExecutionLevel requiredLevel, ExecutionLevel actionLevel, System.Action action) {
        if (LevelIsActive(requiredLevel))
            RunUsingLevel(actionLevel, action);
    }

    /// <summary>
    /// Runs an action with overridden execution level.
    /// </summary>
    /// <param name="level">An execution level that will be active during action execution</param>
    /// <param name="action">An action</param>
    public void RunUsingLevel(ExecutionLevel level, System.Action action) {
        try {
            EnterOverrideSection(level);
            action();
        } finally {
            LeaveOverrideSection();
        }
    }

    /// <summary>
    /// Checks whether the required execution level is active.
    /// </summary>
    /// <param name="requiredLevel">A required execution level</param>
    /// <returns></returns>
    public bool LevelIsActive(ExecutionLevel requiredLevel) => contextManager.Context.Level >= requiredLevel;

}
