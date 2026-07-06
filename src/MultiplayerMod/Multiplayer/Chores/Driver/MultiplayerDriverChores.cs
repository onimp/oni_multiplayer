using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using MultiplayerMod.Core.Collections;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.Multiplayer.Objects;

namespace MultiplayerMod.Multiplayer.Chores.Driver;

[Core.Dependency.Dependency, UsedImplicitly]
public class MultiplayerDriverChores {

    private static readonly Core.Logging.Logger log = LoggerFactory.GetLogger<MultiplayerDriverChores>();

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
        // EatChore resolves its edible from the *client's* own ClosestEdibleSensor at Begin time (the
        // materials economy isn't replicated). If the client duplicant has nothing in reach GetEdible()
        // returns null, and EatChore.Begin dereferences it unguarded (`edible.gameObject`) -> NRE. On the
        // host EdibleIsNotNull gates the chore before Begin; the client bypasses preconditions, so re-run
        // just that one here and skip the assignment if it fails. The dupe reconciles at the next
        // host-driven eat / hard-sync. (See EatChoreSynchronizer for the rest of the Eat sync contract.)
        if (context.chore is EatChore && !HasEdible(ref context)) {
            log.Warning("Skipping EatChore assignment: client duplicant has no edible in reach");
            return;
        }

        // SleepChore takes its `bed` GameObject in the constructor, and CreateChore resolves that reference on
        // the client. A floor / passed-out sleep locator (or a bed built since the last hard-sync) has no
        // shared MultiplayerId, so the reference degrades to a grid cell and can resolve to whatever occupies
        // it - often the sleeping duplicant itself. That "bed" passes Begin (the dupe is already at the cell),
        // so it does NOT fail harmlessly like the SleepChoreSynchronizer comment assumes; it reaches the sleep
        // state where SleepChore.SetAnim does bed.Get<Sleepable>() -> "<dupe> does not have component
        // Sleepable" + NRE. Guard like EatChore: if the resolved bed isn't a Sleepable, skip the assignment
        // and let the dupe reconcile at the next host-driven sleep / hard-sync.
        if (context.chore is SleepChore sleepChore && !HasResolvedBed(sleepChore)) {
            log.Warning("Skipping SleepChore assignment: client couldn't resolve a valid bed/locator");
            return;
        }

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

    // Mirrors EatChore.EdibleIsNotNull, but tolerates a missing RationMonitor on the resolved consumer
    // (the precondition fn would otherwise NRE on GetSMI) - either way "no edible" means skip.
    private static bool HasEdible(ref Chore.Precondition.Context context) {
        try {
            return EatChore.EdibleIsNotNull.fn(ref context, null);
        } catch (Exception exception) {
            log.Warning($"EatChore edible check threw, treating as unavailable: {exception.Message}");
            return false;
        }
    }

    // The bed TargetParameter holds the GameObject SleepChore.SetAnim will call Get<Sleepable>() on. Read it
    // with the non-logging Get(smi) and check the component ourselves, so an unresolved/wrong bed is a quiet
    // skip rather than SetAnim's logged error + NRE.
    private static bool HasResolvedBed(SleepChore chore) {
        var bed = chore.smi.sm.bed.Get(chore.smi);
        return bed != null && bed.GetComponent<Sleepable>() != null;
    }

    public void Release(ChoreDriver driver) {
        var busy = driversAvailability.GetValue(driver, _ => new BoxedValue<bool>(false));
        busy.Value = false;
    }

}
