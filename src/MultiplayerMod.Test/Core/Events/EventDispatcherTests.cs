using System.Collections.Generic;
using MultiplayerMod.Core.Events;
using NUnit.Framework;

namespace MultiplayerMod.Test.Core.Events;

[TestFixture]
[Parallelizable]
public class EventDispatcherTests {

    [Test]
    public void SubscribersMustBeExecuted() {
        var result = new List<int>();
        var dispatcher = new EventDispatcher();

        dispatcher.Subscribe<Event<int>>(it => result.Add(it.Value));
        dispatcher.Subscribe<Event<int>>(it => result.Add(it.Value * 3));

        dispatcher.Dispatch(new Event<int>(2));

        Assert.AreEqual(expected: new List<int> { 2, 6 }, actual: result);
    }

    [Test]
    public void ActionMustBeUnsubscribed() {
        var result = new List<int>();
        var dispatcher = new EventDispatcher();

        IEventSubscription subscription = dispatcher.Subscribe<Event<int>>(it => result.Add(it.Value));
        dispatcher.Subscribe<Event<int>>(it => result.Add(it.Value * 3));

        subscription.Cancel();

        dispatcher.Dispatch(new Event<int>(2));

        Assert.AreEqual(expected: new List<int> { 6 }, actual: result);
    }

    private class Event<T> {
        public T Value { get; }

        public Event(T value) {
            Value = value;
        }
    }

}
