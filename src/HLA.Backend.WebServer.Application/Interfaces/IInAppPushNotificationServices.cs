using HLA.Backend.WebServer.Domain;
using HLA.Backend.WebServer.Domain.InAppPushNotification;

namespace HLA.Backend.WebServer.Application.Interfaces
{
    public interface IInAppPushNotificationServices
    {
        Task<InAppPushNotificationApiResponse> PushNotificationAsync(InAppPushNotificationApiRequest request);
        Task<CommonResponse> TestPushNotificationAsync(InAppPushNotificationApiRequest request);
    }
}
