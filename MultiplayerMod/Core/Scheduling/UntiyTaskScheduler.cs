using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MultiplayerMod.Core.Scheduling;

public class UnityTaskScheduler : TaskScheduler {

    private readonly ConcurrentQueue<Task> tasks = new();

    protected override void QueueTask(Task task) {
        tasks.Enqueue(task);
    }

    protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued) {
        return false;
    }

    protected override IEnumerable<Task> GetScheduledTasks() {
        return tasks;
    }

    public void Tick() {
        foreach (var task in tasks) {
            TryExecuteTask(task);
        }
    }

    public void Run(System.Action action) => tasks.Enqueue(
        Task.Factory.StartNew(action, CancellationToken.None, TaskCreationOptions.None, this)
    );

}
