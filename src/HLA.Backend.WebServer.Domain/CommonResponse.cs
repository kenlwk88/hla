using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace HLA.Backend.WebServer.Domain
{
    public class CommonResponse
    {
        [JsonProperty("code", Order = int.MinValue)]
        public virtual int Code { get; set; } = 0;
        [JsonProperty("message", Order = int.MinValue)]
        public virtual string? Message { get; set; } = string.Empty;
        [JsonProperty("data", Order = int.MaxValue)]
        public virtual object? Data { get; set; }
    }
}
