using Confluent.Kafka;
using HLA.Backend.Core.DataAccess.Implementations;
using HLA.Backend.Core.Domain.Kafka;
using HLA.Backend.Core.Infra.ApacheKafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace HLA.Backend.WebServer.Application.Workers.Implementations
{
    public interface IKafkaPushNotificationServices
    {
        Task ListeningTopic(CancellationToken token);
    }
    public class KafkaPushNotificationServices : IKafkaPushNotificationServices
    {
        private readonly ILogger<KafkaPushNotificationServices> _logger;
        private readonly Consumer<string, string> _consumer;
        private readonly string _topic;
        private readonly ICommonPushNotification _commonPushNotification;
        private readonly INotificationRepo _notificationRepo;

        public KafkaPushNotificationServices(ILogger<KafkaPushNotificationServices> logger, IConfiguration configuration, Consumer<string, string> consumer, ICommonPushNotification commonPushNotification, 
            INotificationRepo notificationRepo)
        {

            _logger = logger;
            _consumer = consumer;
            _commonPushNotification = commonPushNotification;
            _topic = configuration.GetSection("KafkaTopic").Value ?? throw new ArgumentException("Missing KafkaTopic Setting");
            _notificationRepo = notificationRepo;
        }

        public Task ListeningTopic(CancellationToken token)
        {
            _logger.LogInformation($"Start Listening Topic of {_topic}");
            _consumer.OnMessageConsumed += MessageConsumed;
            _consumer.StartConsume(_topic, token);
            return Task.CompletedTask;
        }

        private Task MessageConsumed(Message<string, string> message, TopicPartitionOffset topicPartitionOffset)
        {

            var task = Task.Factory.StartNew(async () =>
            {
                await PushNotification(message.Value, topicPartitionOffset);
            });
            return task;
        }
        private async Task PushNotification(string message, TopicPartitionOffset topicPartitionOffset)
        {
            _logger.LogInformation($"Received message '{message}' at: '{topicPartitionOffset}'");
            try
            {
                var model = JsonConvert.DeserializeObject<NotificationModel>(message);
                if (model?.FcmIds.Count > 0)
                {
                    foreach (var fcmId in model.FcmIds)
                    {
                        var notification = (await _notificationRepo.GetUnpushNotificationAsync(fcmId)).FirstOrDefault();
                        if (notification != null)
                        {
                            await _commonPushNotification.Publish(notification);
                        }
                    }
                }
                else
                {
                    _logger.LogInformation($"Invalid Message '{message}' at: '{topicPartitionOffset}'");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception error occured: {ex.Message}");
            }
        }
    }
}
