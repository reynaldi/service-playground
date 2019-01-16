using Newtonsoft.Json;
using System;
using System.Text;

namespace Playground.Core.MessageBus
{
    public class MessageSerializer : IMessageSerializer
    {
        public object Deserialize(byte[] source, Type sourceType)
        {
            var strObject = Encoding.UTF8.GetString(source);
            return JsonConvert.DeserializeObject(strObject, sourceType);
        }

        public T Deserialize<T>(byte[] source)
        {
            var strObject = Encoding.UTF8.GetString(source);
            return JsonConvert.DeserializeObject<T>(strObject);
        }

        public byte[] Serialize(object source)
        {
            var json = JsonConvert.SerializeObject(source);
            return Encoding.UTF8.GetBytes(json);
        }

        public byte[] Serialize<T>(T source)
        {
            return Serialize(source);
        }
    }
}
