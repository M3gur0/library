using RabbitMQ.Client;
using System;

namespace M3gur0.Library.Infrastructure.Events.RabbitMQ
{
    public interface IRabbitMQPersistentConnection : IDisposable
    {
        bool IsConnected { get; }

        bool TryConnect();

        IModel CreateModel();
    }
}
