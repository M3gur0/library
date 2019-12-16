using System;

namespace M3gur0.Library.Infrastructure.Events
{
    public class Event
    {
        public Event()
        {
            EventId = Guid.NewGuid();
            CreateDate = DateTime.UtcNow;
        }

        public Event(Guid eventId, DateTime createDate)
        {
            EventId = eventId;
            CreateDate = createDate;
        }

        public Guid EventId { get; }

        public DateTime CreateDate { get; }
    }
}
