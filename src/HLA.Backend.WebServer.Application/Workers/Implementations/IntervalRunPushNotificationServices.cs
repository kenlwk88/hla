using HLA.Backend.Core.DataAccess.Implementations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HLA.Backend.WebServer.Application.Workers.Implementations
{
    public interface IIntervalRunPushNotificationServices
    {
        Task StartAsync(CancellationToken token);
    }
    public class IntervalRunPushNotificationServices : IIntervalRunPushNotificationServices
    {
        private readonly ILogger<IntervalRunPushNotificationServices> _logger;
        private readonly IConfiguration _configuration;
        private readonly int _intervalRunInSecond;
        private readonly ICommonPushNotification _commonPushNotification;
        private readonly INotificationRepo _notificationRepo;
        public IntervalRunPushNotificationServices(ILogger<IntervalRunPushNotificationServices> logger, IConfiguration configuration, ICommonPushNotification commonPushNotification,
            INotificationRepo notificationRepo)
        {
            _logger = logger;
            _configuration = configuration;
            _commonPushNotification = commonPushNotification;
            _intervalRunInSecond = string.IsNullOrEmpty(_configuration.GetSection("IntervalRunInSecond").Value) ? 60 : Convert.ToInt32(_configuration.GetSection("IntervalRunInSecond").Value);
            _notificationRepo = notificationRepo;
        }

        public async Task StartAsync(CancellationToken stoppingToken) 
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await PushNotification();
                await Task.Delay(_intervalRunInSecond * 1000, stoppingToken);
            }
        }
        private async Task PushNotification() 
        {
            _logger.LogInformation($"Starting Push Notification Message at {DateTime.Now}");
            var notifications = await _notificationRepo.GetUnpushNotificationAsync(null);
            foreach (var notification in notifications) 
            {
                await _commonPushNotification.Publish(notification);
            }
        }
    }
}
