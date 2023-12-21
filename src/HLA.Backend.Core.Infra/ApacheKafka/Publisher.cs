using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HLA.Backend.Core.Infra.ApacheKafka
{
    public class Publisher
    {
        private readonly ProducerConfig _producerConfig;
        private readonly Random rnd;
        public Publisher(ProducerConfig config)
        {
            _producerConfig = new ProducerConfig(config);
            rnd = new Random();
        }
        public async Task Publish<T>(T message, string topic)
        {
            using (var producer = new ProducerBuilder<string, T>(_producerConfig)
                    .SetValueSerializer(new JsonSerializer<T>())
                    .Build()
                  )
            {
                await producer.ProduceAsync(topic, new Message<string, T>()
                {
                    Key = rnd.Next(5).ToString(),
                    Value = message
                });
            }

        }
    }
}
