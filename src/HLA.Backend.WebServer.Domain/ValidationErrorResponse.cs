using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace HLA.Backend.WebServer.Domain
{
    public class ValidationErrorResponse
    {
        [JsonProperty("code", Order = int.MinValue)]
        public int Code { get; set; }
        [JsonProperty("message", Order = int.MinValue)]
        public string? Message { get; set; }
        [JsonProperty("data", Order = int.MaxValue)]
        public List<string>? Data { get; set; }
    }
}
