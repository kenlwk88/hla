using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HLA.Backend.WebServer.Domain.Register
{
    public class RegisterApiRequest
    {
        [JsonPropertyName("app_source")]
        [Required(ErrorMessage = "app_source parameter is required")]
        [StringLength(20, ErrorMessage = "app_source parameter cannot more than 20 character")]
        public string? AppSource { get; set; }
        [JsonPropertyName("cust_key")]
        [Required(ErrorMessage = "cust_key parameter is required")]
        [StringLength(20, ErrorMessage = "cust_key parameter cannot more than 20 character")]
        public string? CustKey { get; set; }
        [JsonPropertyName("device_os_type")]
        [Required(ErrorMessage = "device_os_type parameter is required")]
        [Range(1, 2, ErrorMessage = "Only allow 1 for android and 2 for IOS")]
        public int DeviceOSType { get; set; } //1 - Android, 2 - IOS
        [JsonPropertyName("device_token")]
        [Required(ErrorMessage = "device_token parameter is required")]
        [StringLength(300, ErrorMessage = "device_token parameter cannot more than 300 character")]
        public string? DeviceToken { get; set; }
    }
}
