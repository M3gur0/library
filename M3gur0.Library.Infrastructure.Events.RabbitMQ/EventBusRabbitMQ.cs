using Autofac;
using Newtonsoft.Json;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace M3gur0.Library.Infrastructure.Events.RabbitMQ
{
    public abstract class EventBusRabbitMQ : IEventBus, IDisposable
    {
        protected abstract string BrokerName { get; }
        protected abstract string AutofacScopeName { get; }

        private readonly IRabbitMQPersistentConnection persistentConnection;
        private readonly IEventBusSubscriptionsManager subscriptionsManager;
        private readonly ILifetimeScope autofac;
        private readonly string queueName;
        private readonly int retryCount;

        private IModel consumerChannel;

        public EventBusRabbitMQ(IRabbitMQPersistentConnection rabbitMQPersistentConnection, IEventBusSubscriptionsManager eventBusSubscriptionsManager, ILifetimeScope lifetimeScope, string subscriptionQueueName = null, int retryCount = 5)
        {
            persistentConnection = rabbitMQPersistentConnection;
            subscriptionsManager = eventBusSubscriptionsManager;
            autofac = lifetimeScope;
            queueName = subscriptionQueueName;
            this.retryCount = retryCount;

            consumerChannel = CreateConsumerChannel();
        }

        public async Task Publish<T>(T @event)
        {
            if (!persistentConnection.IsConnected) persistentConnection.TryConnect();

            var policy = Policy.Handle<BrokerUnreachableException>()
                .Or<SocketException>()
                .WaitAndRetryAsync(retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) => { /*logger*/ });

            var eventName = @event.GetType().Name;

            using (var channel = persistentConnection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: BrokerName, type: "direct");

                var message = JsonConvert.SerializeObject(@event);
                var body = Encoding.UTF8.GetBytes(message);

                await policy.ExecuteAsync(async () =>
                {
                    var properties = channel.CreateBasicProperties();
                    properties.DeliveryMode = 2; // persistent

                    channel.BasicPublish(exchange: BrokerName, routingKey: eventName, mandatory: true, basicProperties: properties, body: body);
                    _ = await Task.FromResult(true);
                });
            }
        }

        public void Subscribe<T, TH>() where T : Event where TH : IEventHandler<T>
        {
            var eventName = subscriptionsManager.GetEventKey<T>();
            DoInternalSubscription(eventName);
            subscriptionsManager.AddSubscription<T, TH>();
            StartBasicConsume();
        }

        public void Unsubscribe<T, TH>() where T : Event where TH : IEventHandler<T> => subscriptionsManager.RemoveSubscription<T, TH>();

        public void Dispose() { }

        private IModel CreateConsumerChannel()
        {
            if (!persistentConnection.IsConnected) persistentConnection.TryConnect();

            var channel = persistentConnection.CreateModel();
            channel.ExchangeDeclare(exchange: BrokerName, type: "direct");
            channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

            channel.CallbackException += (sender, e) =>
            {
                consumerChannel.Dispose();
                consumerChannel = CreateConsumerChannel();
                StartBasicConsume();
            };

            return channel;
        }

        private void DoInternalSubscription(string eventName)
        {
            // Subscription has already been done for this event
            if (subscriptionsManager.HasSubscriptionsForEvent(eventName)) return;

            if (!persistentConnection.IsConnected) persistentConnection.TryConnect();

            using (var channel = persistentConnection.CreateModel())
            {
                channel.QueueBind(queue: queueName, exchange: BrokerName, routingKey: eventName);
            }
        }

        private void StartBasicConsume()
        {
            /// TODO: log and break 
            if (consumerChannel == null) return;

            var consumer = new AsyncEventingBasicConsumer(consumerChannel);
            consumer.Received += OnConsumerReceived;
            consumerChannel.BasicConsume(queue: queueName, autoAck:false, consumer: consumer);
        }

        private async Task OnConsumerReceived(object sender, BasicDeliverEventArgs @event)
        {
            var eventName = @event.RoutingKey;
            var message = Encoding.UTF8.GetString(@event.Body);

            await ProcessEvent(eventName, message);

            consumerChannel.BasicAck(@event.DeliveryTag, multiple: false);
        }

        private async Task ProcessEvent(string eventName, string message)
        {
            /// TODO: log and break
            if (!subscriptionsManager.HasSubscriptionsForEvent(eventName)) return;

            using (var scope = autofac.BeginLifetimeScope(AutofacScopeName))
            {
                var subscriptions = subscriptionsManager.GetHandlersForEvent(eventName);

                foreach(var subscription in subscriptions)
                {
                    var handler = scope.ResolveOptional(subscription.HandlerType);
                    if (handler == null) continue;

                    var eventType = subscriptionsManager.GetEventTypeByName(eventName);
                    var @event = JsonConvert.DeserializeObject(message, eventType);
                    var concreteEventType = typeof(IEventHandler<>).MakeGenericType(eventType);

                    await Task.Yield();
                    await (Task)concreteEventType.GetMethod("Handle").Invoke(handler, new object[] { @event });
                }
            }
        }
    }
}
