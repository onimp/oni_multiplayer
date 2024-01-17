using System.Linq;
using System.Reflection;
using MultiplayerMod.Core.Extensions;
using MultiplayerMod.Game.Chores;
using NUnit.Framework;

namespace MultiplayerMod.Test.Game.Chores;

//[TestFixture]
public class ChoreListTest {
    [Test]
    public void ContainsAllChores() {
        var foundChoreTypes = Assembly.GetAssembly(typeof(Chore))
            .GetTypes()
            .Where(
                type => typeof(Chore).IsAssignableFrom(type)
                        && type != typeof(Chore)
                        && type != typeof(Chore<>)
                        && !type.FullName.Contains("ChoreTableChore")
            )
            .OrderBy(a => a.FullName)
            .ToHashSet();

        Assert.AreEqual(
            foundChoreTypes,
            ChoreList.Config.Keys
                .OrderBy(a => a.FullName)
                .ToHashSet()
        );
    }

    [Test]
    public void StateSyncRequiresCreationSyncEnabled() {
        ChoreList.Config.Where(
            config => config.Value.CreationSync != ChoreList.CreationStatusEnum.On &&
                      config.Value.StatesTransitionSync.Status == ChoreList.StatesTransitionConfig.SyncStatus.On
        ).ForEach(config => Assert.Fail($"State transition sync requires Creation sync being turned on. {config}"));
    }
}
