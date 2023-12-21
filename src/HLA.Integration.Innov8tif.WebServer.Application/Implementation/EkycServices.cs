using HLA.Integration.Innov8tif.WebServer.Domain;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using HLA.Integration.Innov8tif.WebServer.Application.Util;
using Newtonsoft.Json.Converters;
using System.Dynamic;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace HLA.Integration.Innov8tif.WebServer.Application.Implementation
{
    public class EkycServices : IEkycServices
    {
        private readonly ILogger<EkycServices> _logger;
        private readonly HttpClientUtils _httpClientUtils;
        private readonly ExpandoObjectConverter _converter;
        public EkycServices(ILogger<EkycServices> logger, HttpClientUtils httpClientUtils)
        {
            _logger = logger;
            _httpClientUtils = httpClientUtils;
            _converter = new ExpandoObjectConverter();
        }
        public async Task<dynamic> PostJourneyId(dynamic request) 
        {
            try
            {
                var body = JsonConvert.DeserializeObject<ExpandoObject>(request.ToString(), _converter);
                var result = await _httpClientUtils.PostAsync(ApiRoute.JourneyId, JsonConvert.SerializeObject(body));
                return result;
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex.Message, nameof(PostJourneyId));
                throw;
            }
        }
        public async Task<dynamic> PostOkayId(dynamic request)
        {
            try
            {
                var body = JsonConvert.DeserializeObject<ExpandoObject>(request.ToString(), _converter);
                var result = await _httpClientUtils.PostAsync(ApiRoute.OkayId, JsonConvert.SerializeObject(body));
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, nameof(PostOkayId));
                throw;
            }
        }
        public async Task<dynamic> PostOkayFace(object data)
        {
            try
            {
                var result = await _httpClientUtils.PostFormDataAsync(ApiRoute.OkayFace, (IFormCollection)data);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, nameof(PostOkayFace));
                throw;
            }
        }
        public async Task<dynamic> PostOkayDoc(dynamic request)
        {
            try
            {
                var body = JsonConvert.DeserializeObject<ExpandoObject>(request.ToString(), _converter);
                var result = await _httpClientUtils.PostAsync(ApiRoute.OkayDoc, JsonConvert.SerializeObject(body));
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, nameof(PostOkayDoc));
                throw;
            }
        }
        public async Task<dynamic> GetScoreCard(object data)
        {
            try
            {
                StringBuilder query = new StringBuilder();
                foreach (var item in (IQueryCollection)data) 
                {
                    query.Append($"{item.Key}={item.Value}");
                }
                string url = query.Length == 0 ? ApiRoute.ScoreCard : $"{ApiRoute.ScoreCard}?{query}";
                var result = await _httpClientUtils.GetAsync(url);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, nameof(GetScoreCard));
                throw;
            }
        }
    }
}
