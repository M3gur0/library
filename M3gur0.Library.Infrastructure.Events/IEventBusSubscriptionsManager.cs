using System;
using System.Collections.Generic;

namespace M3gur0.Library.Infrastructure.Events
{
    public interface IEventBusSubscriptionsManager
    {
        void AddSubscription<T, TH>();

        void RemoveSubscription<T, TH>() where T : Event where TH : IEventHandler;

        bool HasSubscriptionsForEvent(string eventName);

        IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName);

        string GetEventKey<T>();

        Type GetEventTypeByName(string eventName);
    }
}
