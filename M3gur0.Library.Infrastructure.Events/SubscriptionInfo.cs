using System;

namespace M3gur0.Library.Infrastructure.Events
{
    public class SubscriptionInfo
    {
        public Type HandlerType { get; }

        public SubscriptionInfo(Type handlerType)
        {
            HandlerType = handlerType;
        }
    }
}
