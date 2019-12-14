using System;

namespace M3gur0.Library.Infrastructure.Events
{
    public class Event
    {
        public Event()
        {
            Id = Guid.NewGuid();
            CreateDate = DateTime.UtcNow;
        }

        public Event(Guid id, DateTime createDate)
        {
            Id = id;
            CreateDate = createDate;
        }

        public Guid Id { get; private set; }

        public DateTime CreateDate { get; private set; }
    }
}
