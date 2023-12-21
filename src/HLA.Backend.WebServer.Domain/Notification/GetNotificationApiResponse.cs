using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HLA.Backend.WebServer.Domain.Notification
{
    public class GetNotificationApiResponse : CommonResponse
    {
        List<ApiNotificationDetail> apiNotificationDetail = new();
        [JsonProperty("data", Order = int.MaxValue)]
        [JsonPropertyName("data")]
        public override object? Data 
        {
            get { return apiNotificationDetail ?? new List<ApiNotificationDetail>(); }
            set { apiNotificationDetail = (List<ApiNotificationDetail>)(value ?? new List<ApiNotificationDetail>()); }
        }
    }
    public class ApiNotificationDetail 
    {
        [JsonPropertyName("id")]
        public Int64 ID { get; set; }
        [JsonPropertyName("module_type")]
        public string? ModuleType { get; set; }
        [JsonPropertyName("title")]
        public string? Title { get; set; }
        [JsonPropertyName("short_message")]
        public string? ShortMessage { get; set; }
        [JsonPropertyName("detail_message")]
        public string? DetailMessage { get; set; }
        [JsonPropertyName("is_read")]
        public bool IsRead { get; set; }
        [JsonPropertyName("is_read_date")]
        public DateTime? IsReadDate { get; set; }
        [JsonPropertyName("created_date")]
        public DateTime? CreatedDate { get; set; }
        [JsonPropertyName("data")]
        public Dictionary<string,string> Data { get; set; } = new Dictionary<string,string>();
    }
}
