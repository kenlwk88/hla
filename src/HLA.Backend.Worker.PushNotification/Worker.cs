using HLA.Backend.Worker.PushNotification.Services;

namespace HLA.Backend.Worker.PushNotification
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IKafkaPushNotificationServices _kafkaPushNotificationServices;

        public Worker(ILogger<Worker> logger, IKafkaPushNotificationServices kafkaPushNotificationServices)
        {
            _logger = logger;
            _kafkaPushNotificationServices = kafkaPushNotificationServices;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            List<Task> tasks = new List<Task>();
            tasks.Add(_kafkaPushNotificationServices.ListeningTopic(stoppingToken));
            await Task.WhenAll(tasks);
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