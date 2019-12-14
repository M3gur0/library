using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.Net.Sockets;

namespace M3gur0.Library.Infrastructure.Events.RabbitMQ
{
    public class DefaultRabbitMQPersistentConnection : IRabbitMQPersistentConnection
    {
        private readonly IConnectionFactory connectionFactory;
        private readonly int retryCount;
        private IConnection connection;
        private bool disposed = false;
        private object connectionLocker = new object();

        public DefaultRabbitMQPersistentConnection(IConnectionFactory connectionFactory, int retryCount = 5)
        {
            this.connectionFactory = connectionFactory;
            this.retryCount = retryCount;
        }

        public bool IsConnected => connection != null && connection.IsOpen && !disposed;

        public IModel CreateModel()
        {
            if (!IsConnected) throw new InvalidOperationException("No RabbitMQ connections are available to perform this action");

            return connection.CreateModel();
        }

        public void Dispose()
        {
            if (disposed) return;
            disposed = true;

            try { connection.Dispose(); }
            catch { }
        }

        public bool TryConnect()
        {
            lock (connectionLocker)
            {
                var policy = Policy.Handle<SocketException>()
                    .Or<BrokerUnreachableException>()
                    .WaitAndRetry(retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) => { /*logger*/ });

                policy.Execute(() => connection = connectionFactory.CreateConnection());

                if (IsConnected)
                {
                    connection.ConnectionShutdown += OnConnectionShutdown;
                    connection.ConnectionBlocked += OnConnectionBlocked;
                    connection.CallbackException += OnCallbackException;

                    return true;
                }

                return false;
            }
        }

        private void OnConnectionBlocked(object sender, ConnectionBlockedEventArgs e)
        {
            /// TODO: logger

            if (disposed) return;
            TryConnect();
        }

        private void OnCallbackException(object sender, CallbackExceptionEventArgs e)
        {
            /// TODO: logger

            if (disposed) return;
            TryConnect();
        }

        private void OnConnectionShutdown(object sender, ShutdownEventArgs reason)
        {
            /// TODO: logger

            if (disposed) return;
            TryConnect();
        }
    }
}
