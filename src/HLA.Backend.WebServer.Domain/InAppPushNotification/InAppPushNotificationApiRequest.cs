using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HLA.Backend.WebServer.Domain.InAppPushNotification
{
    public class InAppPushNotificationApiRequest
    {
        [JsonPropertyName("cust_key")]
        [Required(ErrorMessage = "cust_key parameter is required")]
        [StringLength(20, ErrorMessage = "cust_key parameter cannot more than 20 character")]
        public string? CustKey { get; set; }

        [JsonPropertyName("app_source")]
        [Required(ErrorMessage = "app_source parameter is required")]
        [StringLength(20, ErrorMessage = "app_source parameter cannot more than 20 character")]
        public string? AppSource { get; set; }

        [JsonPropertyName("title")]
        [Required(ErrorMessage = "title parameter is required")]
        [StringLength(100, ErrorMessage = "title parameter cannot more than 100 character")]
        public string? Title { get; set; }

        [JsonPropertyName("message")]
        [Required(ErrorMessage = "message parameter is required")]
        [StringLength(300, ErrorMessage = "message parameter cannot more than 300 character")]
        public string? Message { get; set; }
        [JsonPropertyName("data")]
        public Dictionary<string,string> Payload { get; set; } = new Dictionary<string,string>();
    }
}
