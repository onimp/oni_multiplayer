using MultiplayerMod.Multiplayer.Chores.Events;
using NUnit.Framework;

namespace MultiplayerMod.Test.Multiplayer.Chores;

[TestFixture]
public class ChoresInstantiationTests : ChoreTest {

    [Test, TestCaseSource(nameof(ChoresInstantiationTestCases))]
    public void ChoreCreatedEventMustBeFired(ChoreFactory factory) {
        ChoreCreatedEvent firedEvent = null!;
        Events.Subscribe<ChoreCreatedEvent>((@event, subscription) => {
            firedEvent = @event;
            subscription.Cancel();
        });
        factory.Create();
        Assert.NotNull(firedEvent);
        Assert.That(firedEvent, Is.Not.Null);
        Assert.That(firedEvent.Type, Is.EqualTo(factory.Type));
        Assert.That(firedEvent.Arguments, Is.EqualTo(factory.GetArguments()));
    }

}
