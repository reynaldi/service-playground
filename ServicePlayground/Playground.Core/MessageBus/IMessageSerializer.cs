using System;

namespace Playground.Core.MessageBus
{
    public interface IMessageSerializer
    {
        byte[] Serialize(object source);

        byte[] Serialize<T>(T source);

        object Deserialize(byte[] source, Type sourceType);

        T Deserialize<T>(byte[] source);
    }
}
