using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;

namespace MultiplayerMod.Core.Scheduling;

[Dependency, UsedImplicitly]
public class UnityTaskScheduler : TaskScheduler {

    private const int maxTasksPerTick = 1000;

    private readonly Task[] snapshot = new Task[maxTasksPerTick];
    private readonly ConcurrentQueue<Task> tasks = new();

    protected override void QueueTask(Task task) => tasks.Enqueue(task);

    protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued) => false;

    protected override IEnumerable<Task> GetScheduledTasks() => tasks;

    public void Tick() {
        var length = CreateSnapshot();
        for (var i = 0; i < length; ++i) {
            var task = snapshot[i];
            TryExecuteTask(task);
            if (task.Status == TaskStatus.RanToCompletion)
                continue;
            if (task.Exception != null)
                throw new Exception("Scheduled task threw an exception", task.Exception);
        }
    }

    public void Run(System.Action action) => Task.Factory.StartNew(
        action,
        CancellationToken.None,
        TaskCreationOptions.None,
        this
    );

    private int CreateSnapshot() {
        var length = 0;
        while (length < snapshot.Length && tasks.TryDequeue(out var task))
            snapshot[length++] = task;
        return length;
    }

}
