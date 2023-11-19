using System.Linq;
using System.Reflection;
using MultiplayerMod.Game.Chores;
using NUnit.Framework;

namespace MultiplayerMod.Test.Game.Chores;

[TestFixture]
public class ChoreListTest {
    [Test]
    public void EnsureChoreListContainsAllChores() {
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

        Assert.AreEqual(foundChoreTypes, ChoreList.AllChoreTypes);
    }
}
