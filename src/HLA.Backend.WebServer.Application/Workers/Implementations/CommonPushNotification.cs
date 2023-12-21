using HLA.Backend.Core.DataAccess.Implementations;
using HLA.Backend.Core.DataAccess.Models;
using HLA.Backend.Core.Infra.FCM;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace HLA.Backend.WebServer.Application.Workers.Implementations
{
    public interface ICommonPushNotification
    {
        Task Publish(Notifiaction notification);
    }
    public class CommonPushNotification : ICommonPushNotification
    {
        private readonly ILogger<CommonPushNotification> _logger;
        private readonly int _maxErrorCount;
        private readonly INotificationRepo _notificationRepo;
        private readonly IRegisterRepo _registerRepo;
        private readonly IFcmFactory _fcmFactory;
        private readonly object lockUpdateProcess = new();
        public CommonPushNotification(ILogger<CommonPushNotification> logger, IConfiguration configuration, INotificationRepo notificationRepo, IRegisterRepo registerRepo, IFcmFactory fcmFactory)
        {
            _logger = logger;
            _maxErrorCount = string.IsNullOrEmpty(configuration.GetSection("MaxPushNotificationErrorCount").Value) ? 3 : Convert.ToInt32(configuration.GetSection("MaxPushNotificationErrorCount").Value);
            _notificationRepo = notificationRepo;
            _registerRepo = registerRepo;
            _fcmFactory = fcmFactory;
        }
        public async Task Publish(Notifiaction notification)
        {
            try
            {
                if (!notification.IsProcess)
                {
                    var updateIsProcess = false;
                    lock (lockUpdateProcess)
                    {
                        updateIsProcess = _notificationRepo.UpdateIsProcessPushNotificationAsync((int)notification.ID, true).Result;
                    }
                    if (updateIsProcess)
                    {
                        //Initial FCM Service
                        var fcmService = _fcmFactory.CreateService(notification.AppSource!);
                        if (fcmService != null)
                        {
                            try
                            {
                                //Push Notification to active device
                                Dictionary<string, string> data = new Dictionary<string, string>();
                                if (!string.IsNullOrEmpty(notification.Payload))
                                    data = JsonConvert.DeserializeObject<Dictionary<string, string>>(notification.Payload)!;
                                var result = await fcmService.SendToRegisterToken(data, notification.DeviceToken!, notification.Title!, notification.PushMessage!);
                                if (!string.IsNullOrEmpty(result))
                                    _ = await _notificationRepo.UpdateIsPushNotificationAsync((int)notification.ID, true);
                            }
                            catch (FirebaseAdmin.FirebaseException fEx)
                            {
                                _logger.LogError(fEx.Message, fEx);
                                var errorCount = await _notificationRepo.InsertErrorForPushNotificationAsync((int)notification.ID, notification.DeviceToken!, fEx.Message);
                                if (errorCount >= _maxErrorCount)
                                {
                                    //Invalid the device token
                                    await _registerRepo.UpdateInvalidDeviceTokenAsync(notification.DeviceToken!);
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
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
        }
    }
}
