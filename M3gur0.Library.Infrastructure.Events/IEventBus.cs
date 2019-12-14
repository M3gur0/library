using System.Threading.Tasks;

namespace M3gur0.Library.Infrastructure.Events
{
    public interface IEventBus
    {
        Task Publish<T>(T @event);

        void Subscribe<T, TH>() where T : Event where TH : IEventHandler<T>;

        void Unsubscribe<T, TH>() where T : Event where TH : IEventHandler<T>;
    }
}
