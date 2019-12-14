using System;
using System.Collections.Generic;
using System.Linq;

namespace M3gur0.Library.Infrastructure.Events.InMemory
{
    public class InMemoryEventBusSubscriptionsManager : IEventBusSubscriptionsManager
    {
        private readonly Dictionary<string, List<SubscriptionInfo>> handlers;
        private readonly List<Type> eventTypes;

        public event EventHandler<string> OnEventRemoved;

        public InMemoryEventBusSubscriptionsManager()
        {
            handlers = new Dictionary<string, List<SubscriptionInfo>>();
            eventTypes = new List<Type>();
        }

        public void AddSubscription<T, TH>()
        {
            var eventName = GetEventKey<T>();
            DoAddSubscription(typeof(TH), eventName);

            if (!eventTypes.Contains(typeof(T))) eventTypes.Add(typeof(T));
        }

        public string GetEventKey<T>() => typeof(T).Name;

        public Type GetEventTypeByName(string eventName) => eventTypes.SingleOrDefault(e => e.Name == eventName);

        public IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName) => handlers[eventName];

        public bool HasSubscriptionsForEvent(string eventName) => handlers.ContainsKey(eventName);

        public void RemoveSubscription<T, TH>() where T : Event where TH : IEventHandler
        {
            var handlerToRemove = FindSubscription<T, TH>();
            var eventName = GetEventKey<T>();
            DoRemoveHandler(eventName, handlerToRemove);
        }

        private void DoAddSubscription(Type handlerType, string eventName)
        {
            if (!HasSubscriptionsForEvent(eventName))
                handlers.Add(eventName, new List<SubscriptionInfo>());

            if (!handlers[eventName].Any(handler => handler.HandlerType == handlerType))
                handlers[eventName].Add(new SubscriptionInfo(handlerType));
        }

        private void DoRemoveHandler(string eventName, SubscriptionInfo subscriptionToRemove)
        {
            if (subscriptionToRemove == null) return;

            handlers[eventName].Remove(subscriptionToRemove);
            var eventType = eventTypes.SingleOrDefault(e => e.Name == eventName);
            if (eventType != null) eventTypes.Remove(eventType);
            RaiseOnEventRemoved(eventName);
        }

        private SubscriptionInfo FindSubscription<T, TH>() where T : Event where TH : IEventHandler
        {
            var eventName = GetEventKey<T>();

            if (!HasSubscriptionsForEvent(eventName)) return null;

            var handlerType = typeof(TH);
            return handlers[eventName].SingleOrDefault(sub => sub.HandlerType == handlerType);
        }

        private void RaiseOnEventRemoved(string eventName)
        {
            var hanler = OnEventRemoved;
            hanler?.Invoke(this, eventName);
        }
    }
}
