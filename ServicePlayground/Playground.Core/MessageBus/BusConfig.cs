using System;

namespace Playground.Core.MessageBus
{
    public class BusConfig
    {
        public string HostName { get; set; } = "localhost";
        public string VirtualHost { get; set; } = "/";
        public string UserName { get; set; } = "guest";
        public string Password { get; set; } = "guest";
        public int Port { get; set; } = 5672;
        public bool AutoRecoveryEnabled { get; set; } = true;
        public TimeSpan AutoRecoveryInterval { get; set; } = TimeSpan.FromSeconds(5);
        public bool TopologyRecoveryEnabled { get; set; } = true;
        public TimeSpan RequestReplayTimeout { get; set; } = TimeSpan.FromSeconds(5);
        public TimeSpan HeartbeatTimeout { get; set; } = TimeSpan.FromSeconds(30);

        public string ServiceQueueName { get; set; } = string.Empty;

    }
}
