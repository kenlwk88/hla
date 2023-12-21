using AutoMapper;
using HLA.Backend.Core.DataAccess.Implementations;
using HLA.Backend.Core.DataAccess.Models;
using HLA.Backend.Core.Domain.Enum;
using HLA.Backend.Core.Domain.Kafka;
using HLA.Backend.Core.Infra.ApacheKafka;
using HLA.Backend.Core.Infra.Pagination;
using HLA.Backend.WebServer.Application.Interfaces;
using HLA.Backend.WebServer.Domain;
using HLA.Backend.WebServer.Domain.Notification;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;

namespace HLA.Backend.WebServer.Application.Implementations
{
    public class NotificationServices : INotificationServices
    {
        private readonly ILogger<NotificationServices> _logger;
        private readonly IMapper _mapper;
        private readonly INotificationRepo _notificationRepo;
        private readonly IRegisterRepo _registerRepo;
        private readonly Publisher _publisher;
        private readonly string _topic;
        private readonly bool _isKafkaImplementatin;
        public NotificationServices(ILogger<NotificationServices> logger, IConfiguration configuration, IMapper mapper, INotificationRepo notificationRepo, IRegisterRepo registerRepo, Publisher publisher)
        {
            _logger = logger;
            _mapper = mapper;
            _notificationRepo = notificationRepo;
            _registerRepo = registerRepo;
            _publisher = publisher;
            _topic = configuration.GetSection("KafkaTopic").Value ?? throw new ArgumentException("Missing KafkaTopic Setting");
            _isKafkaImplementatin = string.IsNullOrEmpty(configuration.GetSection("IsKafkaImplementatin").Value) ? throw new ArgumentException("Missing IsKafkaImplementatin Setting") : Convert.ToBoolean(configuration.GetSection("IsKafkaImplementatin").Value);
        }
        public async Task<NotificationApiResponse> PostAsync(NotificationApiRequest request)
        {
            NotificationApiResponse response = new();
            try
            {
                //Validate AppSource
                if (!Enum.TryParse<AppSource>(request.AppSource, true, out AppSource app))
                    return Error.Response(201).TryCast<NotificationApiResponse>();

                //Save into Notification List
                int? NotificationId = await _notificationRepo.InsertNotificationAsync(request.AppSource, request.ModuleType!, request.CustKey!, request.Title!, request.ShortMessage!, 
                    request.DetailMessage!, (request.Payload.Any() ? JsonConvert.SerializeObject(request.Payload) : null)!);
                if (NotificationId != null && NotificationId > 0)
                {
                    //Get Register User
                    var registers = await _registerRepo.GetRegisterUsersAsync(request.CustKey!, request.AppSource);
                    if (registers != null && registers.Any())
                    {
                        NotificationModel fcmIdList = new();
                        List<int> fcmIds = new List<int>();

                        //Always push the notification id and module type to app
                        Dictionary<string, string> dicPayload = new();
                        dicPayload.Add("notificationID", NotificationId.ToString()!);
                        dicPayload.Add("moduleType", request.ModuleType!);
                        foreach (var register in registers)
                        {
                            //Only allow valid device token
                            if (register.IsInvalidDeviceToken == null || (register.IsInvalidDeviceToken != null && register.IsInvalidDeviceToken == false))
                            {
                                //Save into DB
                                string payload = string.Empty;
                                payload = JsonConvert.SerializeObject(dicPayload);
                                int? fcmId = await _notificationRepo.InsertPushNotificationAsync(register.CustKey!, register.AppSource!, register.DeviceToken!, request.Title!, request.PushMessage!, payload);
                                if (fcmId != null && fcmId > 0)
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
                }
                else
                {
                    return Error.Response(801).TryCast<NotificationApiResponse>();
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, nameof(PostAsync));
                return Error.Response(999).TryCast<NotificationApiResponse>();
            }
            return response;
        }
        public async Task<CommonResponse> PutAsync(string appSource, string custKey, string moduleType, int id)
        {
            CommonResponse response = new();
            try
            {
                //Validate AppSource
                if (!Enum.TryParse<AppSource>(appSource, true, out AppSource app))
                    return Error.Response(201).TryCast<CommonResponse>();

                //Validate CustKey
                var registers = await _registerRepo.GetRegisterUsersAsync(custKey, appSource);
                if (registers == null || (registers != null && !registers.Any()))
                    return Error.Response(202).TryCast<CommonResponse>();

                var isUpdate = await _notificationRepo.UpdateIsReadNotificationAsync(appSource, moduleType, custKey,id);
                if(!isUpdate)
                    return Error.Response(801).TryCast<CommonResponse>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, nameof(PutAsync));
                return Error.Response(999).TryCast<CommonResponse>();
            }
            return response;
        }
        public async Task<GetNotificationApiResponse> GetAsync(string appSource, string custKey, string moduleTypes, int id, bool isHtmlEncoded, int? index, int? size, int? days)
        {
            GetNotificationApiResponse response = new();
            try
            {
                //Validate AppSource
                if (!Enum.TryParse<AppSource>(appSource, true, out AppSource app))
                    return Error.Response(201).TryCast<GetNotificationApiResponse>();

                //Validate CustKey
                var registers = await _registerRepo.GetRegisterUsersAsync(custKey, appSource);
                if (registers == null || (registers != null && !registers.Any()))
                    return Error.Response(202).TryCast<GetNotificationApiResponse>();

                //Get notifications from DB
                List<NotificationDetail> notifications = new();
                if (id > 0)
                {
                    //Allow only one moduleTypes
                    if (moduleTypes.Split('|').Length > 1)
                        return Error.Response(203).TryCast<GetNotificationApiResponse>();

                    //Get notification by id
                    notifications = (await _notificationRepo.GetNotificationsAsync(appSource, moduleTypes, custKey, id, days)).ToList();
                }
                else 
                {
                    //Get all notifications based on module types provided
                    foreach (var type in moduleTypes.Split('|')) 
                    {
                        notifications.AddRange(await _notificationRepo.GetNotificationsAsync(appSource, type, custKey, id, days));
                    }

                    //Order Descending by Created Date
                    notifications = notifications.OrderByDescending(x => x.CreatedDate).ToList();

                    //Pagination
                    notifications = notifications.ToPagination<NotificationDetail>(index, size);
                }

                //Html Encoded for Short Message and Detail Message
                if (isHtmlEncoded)
                {
                    notifications.ForEach(i =>
                    {
                        i.ShortMessage = WebUtility.HtmlEncode(i.ShortMessage);
                        i.DetailMessage = WebUtility.HtmlEncode(i.DetailMessage);
                    });
                }

                //Bind Final List
                var finalList = _mapper.Map<List<ApiNotificationDetail>>(notifications);
                response.Data = finalList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, nameof(GetAsync));
                return Error.Response(999).TryCast<GetNotificationApiResponse>();
            }
            return response;
        }
    }
}
