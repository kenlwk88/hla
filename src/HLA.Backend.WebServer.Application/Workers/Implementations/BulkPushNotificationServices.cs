using HLA.Backend.Core.DataAccess.Implementations;
using HLA.Backend.WebServer.Application.Interfaces;
using HLA.Backend.WebServer.Domain.Notification;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace HLA.Backend.WebServer.Application.Workers.Implementations
{
    public interface IBulkPushNotificationServices
    {
        Task StartAsync(CancellationToken stoppingToken);
    }
    public class BulkPushNotificationServices : IBulkPushNotificationServices
    {
        private readonly ILogger<BulkPushNotificationServices> _logger;
        private readonly int _intervalRunInSecond;
        private readonly INotificationRepo _notificationRepo;
        private readonly INotificationServices _notificationServices;
        public BulkPushNotificationServices(ILogger<BulkPushNotificationServices> logger, IConfiguration configuration, INotificationRepo notificationRepo, INotificationServices notificationServices)
        {
            _logger = logger;
            _intervalRunInSecond = string.IsNullOrEmpty(configuration.GetSection("IntervalRunInSecond_BulkPushNotification").Value) ? 60 : Convert.ToInt32(configuration.GetSection("IntervalRunInSecond_BulkPushNotification").Value);
            _notificationRepo = notificationRepo;
            _notificationServices = notificationServices;
        }
        public async Task StartAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await BulkPushNotification();
                await Task.Delay(_intervalRunInSecond * 1000, stoppingToken);
            }
        }
        private async Task BulkPushNotification()
        {
            _logger.LogInformation($"Starting Bulk Push Notification Service at {DateTime.Now}");
            var notifications = await _notificationRepo.GetBulkNotificationAsync();
            foreach (var notification in notifications)
            {
                try
                {
                    NotificationApiRequest request = new();
                    request.CustKey = notification.CustKey;
                    request.AppSource = notification.AppSource;
                    request.ModuleType = notification.ModuleType;
                    request.Title = notification.Title;
                    request.ShortMessage = notification.ShortMessage;
                    request.DetailMessage = notification.DetailMessage;
                    request.PushMessage = notification.PushMessage;
                    if (!string.IsNullOrEmpty(notification.Data) && notification.Data.Trim().Length > 0)
                    {
                        request.Payload = JsonConvert.DeserializeObject<Dictionary<string, string>>(notification.Data)!;
                    }
                    var result = await _notificationServices.PostAsync(request);
                    if (result != null)
                    {
                        //Insert into log and remove from transaction table
                        _ = await _notificationRepo.SaveBulkNotificationLogAsync((int)notification.ID, isError: result.Code != 0, errorMessage: (result.Code == 0 ? null : result.Message)!);
                    }
                    else 
                    {
                        //Insert into log and remove from transaction table
                        _ = await _notificationRepo.SaveBulkNotificationLogAsync((int)notification.ID, isError: true, errorMessage: "Unknown Error");
                    }
                }
                catch (Exception ex) 
                {
                    _logger.LogError(ex.Message, nameof(BulkPushNotification));

                    //Insert into log with error
                    _ = await _notificationRepo.SaveBulkNotificationLogAsync((int)notification.ID, isError: true, errorMessage: ex.Message);
                }

            }
        }
    }
}
