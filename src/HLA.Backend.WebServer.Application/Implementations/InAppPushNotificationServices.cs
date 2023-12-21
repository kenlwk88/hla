using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using HLA.Backend.WebServer.Domain;
using HLA.Backend.WebServer.Domain.InAppPushNotification;
using HLA.Backend.WebServer.Application.Interfaces;
using HLA.Backend.Core.DataAccess.Implementations;
using HLA.Backend.Core.Infra.ApacheKafka;
using HLA.Backend.Core.Domain.Kafka;
using HLA.Backend.Core.Domain.Enum;
using HLA.Backend.Core.Infra.FCM;

namespace HLA.Backend.WebServer.Application.Implementations
{
    public class InAppPushNotificationServices : IInAppPushNotificationServices
    {
        private readonly ILogger<InAppPushNotificationServices> _logger;
        private readonly INotificationRepo _notificationRepo;
        private readonly IRegisterRepo _registerRepo;
        private readonly Publisher _publisher;
        private readonly string _topic;
        private readonly bool _isKafkaImplementatin;
        private readonly IFcmFactory _fcmFactory;
        public InAppPushNotificationServices(ILogger<InAppPushNotificationServices> logger, IConfiguration configuration, INotificationRepo notificationRepo, IRegisterRepo registerRepo, Publisher publisher, IFcmFactory fcmFactory)
        {
            _logger = logger;
            _notificationRepo = notificationRepo;
            _registerRepo = registerRepo;
            _publisher = publisher;
            _topic = configuration.GetSection("KafkaTopic").Value ?? throw new ArgumentException("Missing KafkaTopic Setting");
            _isKafkaImplementatin = string.IsNullOrEmpty(configuration.GetSection("IsKafkaImplementatin").Value) ? throw new ArgumentException("Missing IsKafkaImplementatin Setting") : Convert.ToBoolean(configuration.GetSection("IsKafkaImplementatin").Value);
            _fcmFactory = fcmFactory;
        }
        public async Task<InAppPushNotificationApiResponse> PushNotificationAsync(InAppPushNotificationApiRequest request)
        {
            InAppPushNotificationApiResponse response = new();
            try
            {
                //Validate AppSource
                if(!Enum.TryParse<AppSource>(request.AppSource, true, out AppSource result))
                    return Error.Response(201).TryCast<InAppPushNotificationApiResponse>();

                //Validate CustKey
                var registers = await _registerRepo.GetRegisterUsersAsync(request.CustKey!.Trim(), request.AppSource.Trim());
                if (registers == null || (registers != null && !registers.Any()))
                    return Error.Response(202).TryCast<InAppPushNotificationApiResponse>();

                NotificationModel fcmIdList = new();
                List<int> fcmIds = new List<int>();
                foreach (var register in registers!) 
                {
                    //Only allow valid device token
                    if (register.IsInvalidDeviceToken == null || (register.IsInvalidDeviceToken != null && register.IsInvalidDeviceToken == false)) 
                    {
                        //Save into DB
                        string payload = string.Empty;
                        if (request.Payload.Count > 0)
                            payload = JsonConvert.SerializeObject(request.Payload);
                        int? fcmId = await _notificationRepo.InsertPushNotificationAsync(register.CustKey!, register.AppSource!, register.DeviceToken!, request.Title!, request.Message!, payload);
                        if (fcmId != null)
                            fcmIds.Add(Convert.ToInt32(fcmId));
                    }
                }
                fcmIdList.FcmIds = fcmIds;

                //Publish to Kafka
                if (_isKafkaImplementatin)
                {
                    if (fcmIdList.FcmIds.Count > 0)
                        await _publisher.Publish<NotificationModel>(fcmIdList, _topic);
                    _logger.LogInformation($"Publish Message '{JsonConvert.SerializeObject(fcmIds)}' to {_topic}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, nameof(PushNotificationAsync));
                return Error.Response(801).TryCast<InAppPushNotificationApiResponse>();
            }
            return response;
        }
        public async Task<CommonResponse> TestPushNotificationAsync(InAppPushNotificationApiRequest request)
        {
            CommonResponse response = new();
            try
            {
                //Validate AppSource
                if (!Enum.TryParse<AppSource>(request.AppSource, true, out AppSource result))
                    return Error.Response(201).TryCast<CommonResponse>();

                //Validate CustKey
                var registers = await _registerRepo.GetRegisterUsersAsync(request.CustKey!.Trim(), request.AppSource.Trim());
                if (registers == null || (registers != null && !registers.Any()))
                    return Error.Response(202).TryCast<CommonResponse>();

                foreach (var register in registers!)
                {
                    if (register.IsInvalidDeviceToken == null || (register.IsInvalidDeviceToken != null && register.IsInvalidDeviceToken == false))
                    {
                        var fcmService = _fcmFactory.CreateService(request.AppSource!);
                        //Push to mobile and ignore for any errors
                        try
                        {
                            _ = await fcmService.SendToRegisterToken(request.Payload, register.DeviceToken!, request.Title!, request.Message!, true);
                        }
                        catch (Exception ex) 
                        {
                            _logger.LogError(ex.Message, nameof(TestPushNotificationAsync));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, nameof(TestPushNotificationAsync));
                return Error.Response(801).TryCast<CommonResponse>();
            }
            return response;
        }
    }
}
