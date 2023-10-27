using MultiplayerMod.Core.Logging;

namespace MultiplayerMod.Multiplayer.Chores;

public static class ChoreMultiplayerPreconditions {

    private static Core.Logging.Logger log = LoggerFactory.GetLogger(typeof(ChoreMultiplayerPreconditions));

    public static Chore.Precondition HasHostSelectedDriver = new() {
        id = "HasHostSelectedDriver",
        description = "This chore has no driver on host",
        fn = (ref Chore.Precondition.Context context, object data) => {
            var pooledChore = MultiplayerHostChores.FindPooledChore(ref context);
            if (pooledChore == null)
                return false;

            var chore = context.chore;
            var driver = context.consumerState.choreDriver;

            log.Debug($"Chore driver {driver.GetProperName()} found a host pooled chore {pooledChore}");

            var shouldBeAssigned = pooledChore.Driver == driver;
            var runByDesiredDriver = pooledChore.Driver == chore.driver;

            if (chore.driver != null && !runByDesiredDriver && shouldBeAssigned && pooledChore.Preemptable) {
                log.Debug(
                    $"Chore driver {chore.driver.GetProperName()} stops a host pooled chore {pooledChore} " +
                    $"in favor of {pooledChore.Driver.GetProperName()}"
                );
                chore.driver.StopChore();
            }

            return shouldBeAssigned;
        },
        sortOrder = -1
    };

}
