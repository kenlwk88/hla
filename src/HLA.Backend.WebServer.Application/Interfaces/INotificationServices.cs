using HLA.Backend.WebServer.Domain;
using HLA.Backend.WebServer.Domain.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HLA.Backend.WebServer.Application.Interfaces
{
    public interface INotificationServices
    {
        Task<NotificationApiResponse> PostAsync(NotificationApiRequest request);
        Task<CommonResponse> PutAsync(string appSource, string custKey, string moduleType, int id);
        Task<GetNotificationApiResponse> GetAsync(string appSource, string custKey, string moduleTypes, int id, bool isHtmlEncoded, int? index, int? size, int? days);
    }
}
