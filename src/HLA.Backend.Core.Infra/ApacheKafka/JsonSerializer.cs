using Confluent.Kafka;
using Newtonsoft.Json;
using System.Text;

namespace HLA.Backend.Core.Infra.ApacheKafka
{
    public class JsonSerializer<T> : ISerializer<T>
    {
        public byte[] Serialize(T data, SerializationContext context)
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));
        }
    }

    public class JsonDeserializer<T> : IDeserializer<T>
    {

        T IDeserializer<T>.Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
        {
            return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(data))!;
        }
    }
}
