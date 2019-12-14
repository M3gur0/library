using System.Threading.Tasks;

namespace M3gur0.Library.Infrastructure.Events
{
    public interface IEventHandler<in TEvent> : IEventHandler where TEvent : Event
    {
        Task Handle(TEvent @event);
    }

    public interface IEventHandler { }
}
