using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace MultiplayerMod.Multiplayer.Chores;

[Core.Dependency.Dependency, UsedImplicitly]
public class DriverChoresQueue {

    private readonly ConditionalWeakTable<ChoreDriver, DriverQueue> driversQueue = new();

    private static DriverChoresQueue choresQueue = null!;

    public static Chore.Precondition IsDriverBusy = new() {
        id = nameof(IsDriverBusy),
        description = "The chore driver is busy with a host chore",
        fn = (ref Chore.Precondition.Context context, object _) => !choresQueue.DriverBusy(ref context),
        sortOrder = -1
    };

    public DriverChoresQueue() {
        choresQueue = this;
    }

    private DriverQueue GetQueue(ChoreDriver driver) => driversQueue.GetValue(driver, _ => new DriverQueue());

    public bool DriverBusy(ref Chore.Precondition.Context context) => driversQueue.TryGetValue(
        context.consumerState.choreDriver,
        out var queue
    ) && queue.Busy;

    public void Enqueue(ChoreDriver driver, ref Chore.Precondition.Context context) {
        var queue = GetQueue(driver);
        queue.Contexts.Enqueue(context);
        context.chore.onExit += _ => {
            if (queue.Contexts.Count > 0)
                driver.SetChore(queue.Contexts.Dequeue());
            else
                queue.Busy = false;
        };

        if (queue.Busy)
            return;

        driver.SetChore(queue.Contexts.Dequeue());
        queue.Busy = true;
    }

    public void Clear(ChoreDriver driver, bool forceStop = false) {
        var queue = GetQueue(driver);
        if (!queue.Busy)
            return;

        queue.Contexts.Clear();
        if (forceStop)
            driver.StopChore();
    }

    private class DriverQueue {
        public Queue<Chore.Precondition.Context> Contexts { get; } = new();
        public bool Busy { get; set; }
    }

}
