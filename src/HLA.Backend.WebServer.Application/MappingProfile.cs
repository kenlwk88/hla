using AutoMapper;
using HLA.Backend.Core.DataAccess.Models;
using HLA.Backend.WebServer.Domain.Notification;
using Newtonsoft.Json;

namespace HLA.Backend.WebServer.Application
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<NotificationDetail, ApiNotificationDetail>()
              .ForMember(x => x.Data, cfg => { cfg.MapFrom(j => j.Data != null ? JsonConvert.DeserializeObject<Dictionary<string, string>>(j.Data) : new Dictionary<string, string>()); });
        }
    }
}
