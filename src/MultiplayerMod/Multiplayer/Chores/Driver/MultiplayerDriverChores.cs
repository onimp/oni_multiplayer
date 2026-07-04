using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using MultiplayerMod.Core.Collections;
using MultiplayerMod.Multiplayer.Objects;

namespace MultiplayerMod.Multiplayer.Chores.Driver;

[Core.Dependency.Dependency, UsedImplicitly]
public class MultiplayerDriverChores {

    private readonly ConditionalWeakTable<ChoreDriver, BoxedValue<bool>> driversAvailability = new();

    private static MultiplayerDriverChores driverChores = null!;
    private static MultiplayerObjects objects = null!;

    public static Chore.Precondition IsDriverBusy = new() {
        id = nameof(IsDriverBusy),
        description = "The chore driver is busy with a host chore",
        fn = (ref Chore.Precondition.Context context, object _) => !driverChores.Busy(ref context),
        sortOrder = -1
    };

    public static Chore.Precondition IsMultiplayerChore = new() {
        id = nameof(IsMultiplayerChore),
        description = "The chore is created in multiplayer and will be executed manually",
        fn = (ref Chore.Precondition.Context context, object _) => objects.Get(context.chore) == null,
        sortOrder = -1
    };

    public MultiplayerDriverChores(MultiplayerObjects objects) {
        driverChores = this;
        MultiplayerDriverChores.objects = objects;
    }

    private bool Busy(ref Chore.Precondition.Context context) => driversAvailability.TryGetValue(
        context.consumerState.choreDriver,
        out var result
    ) && result.Value;

    public void Set(ChoreDriver driver, ref Chore.Precondition.Context context) {
        // A preemptable chore (e.g. a dig) can be reassigned between duplicants on the host. The host
        // stops the old driver before the new one begins, but on the client the chore still points at
        // its previous driver, so ChoreDriver.SetChore -> Chore.Begin logs "driver already set" and
        // leaves the chore attached to two drivers. Detach it from the old driver first, mirroring the
        // host's preemption. (Chores that are never reassigned - Idle/Pee/etc. - simply skip this.)
        var currentDriver = context.chore?.driver;
        if (currentDriver != null && currentDriver != driver)
            currentDriver.StopChore();

        // The same reassignment can also leave the *target* duplicant's worker orphaned mid-work: its
        // ChoreDriver SM and its WorkerBase.state drift apart across the separate SetDriverChore /
        // ReleaseChoreDriver commands, so the worker is still "Working" a chore its driver no longer
        // holds. WorkChore.Begin -> ... -> StandardWorker.StartWork asserts the worker is Idle and logs
        // "state should be idle but it's Working" otherwise. Force any stale work to stop so the new
        // chore begins from a clean worker. (No-op when already Idle - the common case.)
        var worker = driver.GetComponent<WorkerBase>();
        if (worker != null && worker.GetState() != WorkerBase.State.Idle)
            worker.StopWork();

        var busy = driversAvailability.GetValue(driver, _ => new BoxedValue<bool>(true));
        busy.Value = true;
        driver.SetChore(context);
    }

    public void Release(ChoreDriver driver) {
        var busy = driversAvailability.GetValue(driver, _ => new BoxedValue<bool>(false));
        busy.Value = false;
    }

}
