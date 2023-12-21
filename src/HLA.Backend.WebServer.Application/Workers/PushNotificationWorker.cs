using HLA.Backend.WebServer.Application.Workers.Implementations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HLA.Backend.WebServer.Application.Workers
{
    public class PushNotificationWorker : BackgroundService
    {
        private readonly ILogger<PushNotificationWorker> _logger;
        private readonly IKafkaPushNotificationServices _kafkaPushNotificationServices;
        private readonly IIntervalRunPushNotificationServices _intervalRunPushNotificationServices;
        private readonly IBulkPushNotificationServices _bulkPushNotificationServices;
        private readonly bool _isKafkaImplementatin;
        public PushNotificationWorker(ILogger<PushNotificationWorker> logger, IConfiguration configuration, IKafkaPushNotificationServices kafkaPushNotificationServices, 
            IIntervalRunPushNotificationServices intervalRunPushNotificationServices, IBulkPushNotificationServices bulkPushNotificationServices)
        {
            _logger = logger;
            _kafkaPushNotificationServices = kafkaPushNotificationServices;
            _intervalRunPushNotificationServices = intervalRunPushNotificationServices;
            _isKafkaImplementatin = string.IsNullOrEmpty(configuration.GetSection("IsKafkaImplementatin").Value) ? throw new ArgumentException("Missing IsKafkaImplementatin Setting") : Convert.ToBoolean(configuration.GetSection("IsKafkaImplementatin").Value);
            _bulkPushNotificationServices = bulkPushNotificationServices;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            List<Task> tasks = new List<Task>();
            if (_isKafkaImplementatin)
            {
                tasks.Add(_kafkaPushNotificationServices.ListeningTopic(stoppingToken));
            }
            else 
            {
                tasks.Add(_intervalRunPushNotificationServices.StartAsync(stoppingToken));
            }

            //For Bulk Push Notification
            tasks.Add(_bulkPushNotificationServices.StartAsync(stoppingToken));

            await Task.WhenAll(tasks.ToArray());
        }
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting Push Notification Service");
            return base.StartAsync(cancellationToken);
        }
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping Push Notification Service");
            return base.StopAsync(cancellationToken);
        }
    }
}
