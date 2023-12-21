using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HLA.Backend.WebServer.Domain.Notification
{
    public class NotificationApiRequest
    {
        [JsonPropertyName("cust_key")]
        [Required(ErrorMessage = "cust_key parameter is required")]
        [StringLength(20, ErrorMessage = "cust_key parameter cannot more than 20 character")]
        public string? CustKey { get; set; }

        [JsonPropertyName("app_source")]
        [Required(ErrorMessage = "app_source parameter is required")]
        [StringLength(20, ErrorMessage = "app_source parameter cannot more than 20 character")]
        public string? AppSource { get; set; }

        [JsonPropertyName("module_type")]
        [Required(ErrorMessage = "module_type parameter is required")]
        [StringLength(20, ErrorMessage = "module_type parameter cannot more than 20 character")]
        public string? ModuleType { get; set; }

        [JsonPropertyName("title")]
        [Required(ErrorMessage = "title parameter is required")]
        [StringLength(100, ErrorMessage = "title parameter cannot more than 100 character")]
        public string? Title { get; set; }

        [JsonPropertyName("short_message")]
        [Required(ErrorMessage = "short_message parameter is required")]
        [StringLength(200, ErrorMessage = "short_message parameter cannot more than 200 character")]
        public string? ShortMessage { get; set; }

        [JsonPropertyName("detail_message")]
        [Required(ErrorMessage = "detail_message parameter is required")]
        [StringLength(600, ErrorMessage = "detail_message parameter cannot more than 600 character")]
        public string? DetailMessage { get; set; }

        [JsonPropertyName("push_message")]
        [Required(ErrorMessage = "push_message parameter is required")]
        [StringLength(300, ErrorMessage = "push_message parameter cannot more than 300 character")]
        public string? PushMessage { get; set; }

        [JsonPropertyName("data")]
        public Dictionary<string, string> Payload { get; set; } = new Dictionary<string, string>();
    }
}
