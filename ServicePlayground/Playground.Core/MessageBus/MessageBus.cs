using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Framing;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Playground.Core.MessageBus
{
    public class MessageBus : IMessageBus
    {
        private readonly BusConfig _config;
        private IConnection _connection;
        private IModel _channel;
        public bool IsConnected => (_connection?.IsOpen ?? false) && (_channel?.IsOpen ?? false);
        private string _serviceQueueName;
        private string _consumerTag;
        private Assembly[] _appDomainAssemblies;
        private IContainer _container;
        private readonly IDictionary<string, Type> _typeMap = new ConcurrentDictionary<string, Type>();
        private readonly IMessageSerializer _serializer;

        public MessageBus(BusConfig busConfig, IServiceCollection services)
        {
            _config = busConfig;
            var containerBuilder = new ContainerBuilder();
            containerBuilder.Populate(services ?? new ServiceCollection());
            ConfigureServices(containerBuilder);

            _serializer = _container.Resolve<IMessageSerializer>();

        }

        public void Connect()
        {
            if (IsConnected) return;
            var connectionFactory = new ConnectionFactory
            {
                HostName = _config.HostName,
                VirtualHost = _config.VirtualHost,
                UserName = _config.UserName,
                Password = _config.Password,
                Port = _config.Port,
                AutomaticRecoveryEnabled = _config.AutoRecoveryEnabled,
                NetworkRecoveryInterval = _config.AutoRecoveryInterval,
                TopologyRecoveryEnabled = _config.TopologyRecoveryEnabled,
                RequestedHeartbeat = (ushort)_config.HeartbeatTimeout.TotalSeconds
            };
            _connection = connectionFactory.CreateConnection();
            _channel = _connection.CreateModel();
            _serviceQueueName = string.IsNullOrEmpty(_config.ServiceQueueName) ?
                _config.ServiceQueueName : Assembly.GetEntryAssembly()?.EntryPoint?.DeclaringType?.Namespace ?? string.Empty;
            if (string.IsNullOrEmpty(_serviceQueueName))
                throw new InvalidOperationException("Unable to determine service queue name automatically. Please set ServiceQueueName on BusConfig!");
            _channel.QueueDeclare(_serviceQueueName, false, false, false, null);

            Expression<Func<Type, bool>> realTypeFilter =
                t => !t.GetTypeInfo().IsInterface && !t.GetTypeInfo().IsAbstract && t.IsInstanceOfType(typeof(IEvent));
            var messageTypeFilter = realTypeFilter;

            var assemblies = GetAppDomainAssemblies();
            var assemblyTypes = new List<Type>();


            foreach (var assembly in assemblies)
            {
                try
                {
                    var types = assembly.GetTypes();
                    if (types != null && types.Any()) assemblyTypes.AddRange(types);
                }
                catch (ReflectionTypeLoadException) { /* ignored */ }
            }

            var messageTypes = assemblyTypes
                .Where(messageTypeFilter.Compile())
                .ToList();

            foreach (var messageType in messageTypes)
            {
                if (!_typeMap.ContainsKey(messageType.FullName))
                    _typeMap.Add(messageType.FullName, messageType);

                if (string.IsNullOrEmpty(messageType.Namespace))
                    continue;

                if (messageType.Namespace.StartsWith(_serviceQueueName))
                {
                    _channel.ExchangeDeclare(messageType.FullName, ExchangeType.Direct, true, false, null);
                }
                else
                {
                    _channel.QueueBind(_serviceQueueName, messageType.FullName, "", null);
                }
            }

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += EventReceived;

            _consumerTag = _channel.BasicConsume(consumer, _serviceQueueName, true);

        }

        private void EventReceived(object sender, BasicDeliverEventArgs message)
        {
            var requestMessageProps = message.BasicProperties;
            var correlationId = requestMessageProps.CorrelationId ?? string.Empty;
            var replayQueueName = requestMessageProps.ReplyTo ?? string.Empty;
            var requestMessageTypeName = requestMessageProps.ContentType ?? string.Empty;
            var requestMessageType = _typeMap[requestMessageTypeName];
            var requestMessage = _serializer.Deserialize(message.Body, requestMessageType);
            var handlerBuilderType = typeof(EventHandlerBuilder);

            handlerBuilderType.GetMethod("Handle").MakeGenericMethod(requestMessageType)
                .Invoke(_container.Resolve<EventHandlerBuilder>(), new[] { requestMessage });
        }

        public void Disconnect()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Publish<T>(T eventMessage)
        {
            if (eventMessage == null || string.IsNullOrEmpty(eventMessage.GetType().Namespace) ||
               !eventMessage.GetType().IsAssignableFrom(typeof(IEvent)))
                return;

            var targetExchange = eventMessage.GetType().FullName;
            var props = new BasicProperties { ContentType = eventMessage.GetType().FullName };

            _channel.BasicPublish(targetExchange, "", props, _serializer.Serialize(eventMessage));

        }

        private Assembly[] GetAppDomainAssemblies()
        {
            if (_appDomainAssemblies != null) return _appDomainAssemblies;

            var runtimeLibraries = DependencyContext.Default.RuntimeLibraries;
            var assemblyList = new List<Assembly>();
            foreach (var library in runtimeLibraries)
            {
                try
                {
                    var assembly = Assembly.Load(new AssemblyName(library.Name));
                    assemblyList.Add(assembly);
                }
                catch (Exception) {/* ignored */}
            }
            _appDomainAssemblies = assemblyList.ToArray();
            return _appDomainAssemblies;
        }

        private void ConfigureServices(ContainerBuilder builder)
        {
            builder.RegisterType<MessageSerializer>()
                .As<IMessageSerializer>()
                .IfNotRegistered(typeof(IMessageSerializer))
                .InstancePerLifetimeScope();

            builder.RegisterAssemblyTypes(GetAppDomainAssemblies())
                .AsClosedTypesOf(typeof(IEventHandler<>))
                .InstancePerLifetimeScope();

            builder.RegisterType<EventHandlerBuilder>().AsSelf().SingleInstance();
            builder.Register<IMessageBus>(ctx => this).SingleInstance();

            _container = builder.Build();
        }

    }
}
