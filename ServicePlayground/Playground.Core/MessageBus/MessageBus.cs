using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
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

        public MessageBus(BusConfig busConfig, IServiceCollection services)
        {
            _config = busConfig;
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

            var consumer = new EventingBasicConsumer(_channel); 
            
        }

        public void Disconnect()
        {
            throw new System.NotImplementedException();
        }

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }

        public void Publish<T>(T eventMessage)
        {
            throw new System.NotImplementedException();
        }
    }
}
