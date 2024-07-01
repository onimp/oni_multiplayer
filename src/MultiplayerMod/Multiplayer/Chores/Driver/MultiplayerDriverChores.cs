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
        var busy = driversAvailability.GetValue(driver, _ => new BoxedValue<bool>(true));
        busy.Value = true;
        driver.SetChore(context);
    }

    public void Release(ChoreDriver driver) {
        var busy = driversAvailability.GetValue(driver, _ => new BoxedValue<bool>(false));
        busy.Value = false;
    }

}
