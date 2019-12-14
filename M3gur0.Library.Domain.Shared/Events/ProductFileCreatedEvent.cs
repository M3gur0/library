using M3gur0.Library.Infrastructure.Events;
using System;

namespace M3gur0.Library.Domain.Shared.Events
{
    public class ProductFileCreatedEvent : Event
    {
        public string Cab { get; private set; }

        public string Cai { get; private set; }

        public string Lpc { get; private set; }

        public DateTime CuringDate { get; private set; }

        public ProductFileCreatedEvent(string cab, string cai, string lpc, DateTime curingDate)
        {
            Cab = cab;
            Cai = cai;
            Lpc = lpc;
            CuringDate = curingDate;
        }
    }
}
