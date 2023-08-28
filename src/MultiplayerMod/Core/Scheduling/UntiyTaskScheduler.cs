using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MultiplayerMod.Core.Scheduling;

// ReSharper disable once ClassNeverInstantiated.Global
public class UnityTaskScheduler : TaskScheduler {

    private const int maxTasksPerTick = 1000;

    private readonly Task[] snapshot = new Task[maxTasksPerTick];
    private readonly ConcurrentQueue<Task> tasks = new();

    protected override void QueueTask(Task task) => tasks.Enqueue(task);

    protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued) => false;

    protected override IEnumerable<Task> GetScheduledTasks() => tasks;

    public void Tick() {
        var length = CreateSnapshot();
        for (var i = 0; i < length; ++i)
            TryExecuteTask(snapshot[i]);
    }

    public void Run(System.Action action) => tasks.Enqueue(
        Task.Factory.StartNew(action, CancellationToken.None, TaskCreationOptions.None, this)
    );

    private int CreateSnapshot() {
        var length = 0;
        while (length < snapshot.Length && tasks.TryDequeue(out var task))
            snapshot[length++] = task;
        return length;
    }

}
