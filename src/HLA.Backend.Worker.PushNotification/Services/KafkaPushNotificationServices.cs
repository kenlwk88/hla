using Confluent.Kafka;
using HLA.Backend.Core.DataAccess.Implementations;
using HLA.Backend.Core.Domain.Kafka;
using HLA.Backend.Core.Infra.ApacheKafka;
using HLA.Backend.Core.Infra.FCM;
using Newtonsoft.Json;

namespace HLA.Backend.Worker.PushNotification.Services
{
    public interface IKafkaPushNotificationServices
    {
        Task ListeningTopic(CancellationToken token);
    }
    public class KafkaPushNotificationServices : IKafkaPushNotificationServices
    {
        private readonly ILogger<KafkaPushNotificationServices> _logger;
        private readonly IConfiguration _configuration;
        private readonly Consumer<string, string> _consumer;
        private readonly string _topic;
        private readonly int _maxErrorCount;
        private readonly INotificationRepo _notificationRepo;
        private readonly IRegisterRepo _registerRepo;
        private readonly IFcmFactory _fcmFactory;
        public KafkaPushNotificationServices(ILogger<KafkaPushNotificationServices> logger, IConfiguration configuration, Consumer<string, string> consumer, INotificationRepo notificationRepo, IRegisterRepo registerRepo, IFcmFactory fcmFactory)
        {

            _logger = logger;
            _configuration = configuration;
            _consumer = consumer;
            _topic = _configuration.GetSection("KafkaTopic").Value;
            _maxErrorCount = string.IsNullOrEmpty(_configuration.GetSection("MaxPushNotificationErrorCount").Value) ? 3 : Convert.ToInt32(_configuration.GetSection("MaxPushNotificationErrorCount").Value);
            _notificationRepo = notificationRepo;
            _registerRepo = registerRepo;
            _fcmFactory = fcmFactory;
        }

        public async Task ListeningTopic(CancellationToken token)
        {
            _logger.LogInformation($"Start Listening Topic of {_topic}");
            _consumer.OnMessageConsumed += MessageConsumed;
            Task[] consumers = {
                _consumer.StartConsume(_topic,token)
            };
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
                NotificationModel model = new NotificationModel();
                model = JsonConvert.DeserializeObject<NotificationModel>(message);
                if (model.FcmIds.Count > 0)
                {
                    foreach (var fcmId in model.FcmIds) 
                    {
                        var notification = (await _notificationRepo.GetUnpushNotificationAsync(fcmId)).FirstOrDefault();
                        if (notification != null) 
                        {
                            if (!notification.IsProcess)
                            {
                                var updateIsProcess = await _notificationRepo.UpdateIsProcessPushNotificationAsync((int)notification.ID, true);
                                if (updateIsProcess)
                                {
                                    //Initial FCM Service
                                    var fcmService = await _fcmFactory.CreateService(notification.AppSource);
                                    if (fcmService != null)
                                    {
                                        try
                                        {
                                            //Push Notification to active device
                                            //Test on App
                                            Dictionary<string, string> data = new Dictionary<string, string>();
                                            var result = await fcmService.SendToRegisterToken(data, notification.DeviceToken, notification.Title, notification.PushMessage);
                                            if (!string.IsNullOrEmpty(result))
                                                _ = await _notificationRepo.UpdateIsPushNotificationAsync((int)notification.ID, true);
                                        }
                                        catch (FirebaseAdmin.FirebaseException fEx)
                                        {
                                            _logger.LogError(fEx.Message, fEx);
                                            var errorCount = await _notificationRepo.InsertErrorForPushNotificationAsync((int)notification.ID, notification.DeviceToken, fEx.Message);
                                            if (errorCount >= _maxErrorCount)
                                            {
                                                //Invalid the device token
                                                _ = await _registerRepo.UpdateInvalidDeviceTokenAsync(notification.DeviceToken);
                                            }
                                            else
                                            {
                                                //Update IsProcess to false for retry
                                                _ = await _notificationRepo.UpdateIsProcessPushNotificationAsync((int)notification.ID, false);
                                            }

                                        }
                                        catch (Exception ex)
                                        {
                                            _logger.LogError(ex.Message, ex);
                                        }
                                    }
                                }
                            }
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
