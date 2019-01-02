using System;

namespace Playground.Core.MessageBus
{
    public interface IMessageBus : IDisposable
    {
        bool IsConnected { get; }
        void Publish<T>(T eventMessage);
        void Connect();
        void Disconnect();
    }
}
