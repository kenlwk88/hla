using Confluent.Kafka;

namespace HLA.Backend.Core.Infra.ApacheKafka
{
    public class Consumer<TKey, TValue>
    {
        private readonly ConsumerConfig _consumerConfig;

        public delegate Task MessageConsumedDelegate(Message<TKey, TValue> message, TopicPartitionOffset topicPartitionOffset);

        public event MessageConsumedDelegate? OnMessageConsumed;

        public Consumer(ConsumerConfig config)
        {
            _consumerConfig = new ConsumerConfig(config);
        }

        public Task StartConsume(string topic, CancellationToken token)
        {

            var task = Task.Run(async () =>
            {
                var consumer = new ConsumerBuilder<TKey, TValue>(_consumerConfig)
                                //.SetValueDeserializer(new JsonDeserializer<TValue>())
                                .Build();

                consumer.Subscribe(topic);

                try
                {
                    while (!token.IsCancellationRequested)
                    {
                        try
                        {
                            ConsumeResult<TKey, TValue> consumResult = consumer.Consume(token);

                            if (consumResult != null && OnMessageConsumed != null)
                            {
                                await OnMessageConsumed.Invoke(consumResult.Message, consumResult.TopicPartitionOffset);
                            }
                        }
                        catch (ConsumeException e)
                        {
                            throw new ArgumentException($"Consumer error occured: {e.Error.Reason}");
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    consumer.Close();
                }
            }, token);
            return task;
        }
    }
}
