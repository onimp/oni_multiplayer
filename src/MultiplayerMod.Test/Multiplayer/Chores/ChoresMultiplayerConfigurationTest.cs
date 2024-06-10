using System.Linq;
using System.Reflection;
using HarmonyLib;
using MultiplayerMod.Multiplayer.Chores;
using NUnit.Framework;

namespace MultiplayerMod.Test.Multiplayer.Chores;

[TestFixture]
public class ChoresMultiplayerConfigurationTest {

    [Test]
    public void GeneralChoresMustBeSynchronized() {
        var choreTypes = AccessTools.GetTypesFromAssembly(Assembly.GetAssembly(typeof(Chore)))
            .Where(it => typeof(Chore).IsAssignableFrom(it))
            .Where(it => it != typeof(Chore))
            .Where(it => !it.IsGenericType);

        var configuredTypes = ChoresMultiplayerConfiguration.Configuration.Select(it => it.ChoreType);

        Assert.That(configuredTypes, Is.EquivalentTo(choreTypes));
    }

}
