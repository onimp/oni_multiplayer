using System.Collections;
using System.Collections.Generic;

namespace MultiplayerMod.Core.Events;

public class EventSubscriptions : IEnumerable<IEventSubscription> {

    private readonly List<IEventSubscription> subscriptions = new();

    public EventSubscriptions Add(IEventSubscription subscription) {
        subscriptions.Add(subscription);
        return this;
    }

    public void Cancel() => subscriptions.ForEach(it => it.Cancel());

    public IEnumerator<IEventSubscription> GetEnumerator() => subscriptions.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

}
