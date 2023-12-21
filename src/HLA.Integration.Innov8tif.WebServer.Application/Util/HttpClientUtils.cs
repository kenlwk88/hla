using HLA.Integration.Innov8tif.WebServer.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text;

namespace HLA.Integration.Innov8tif.WebServer.Application.Util
{
    public class HttpClientUtils
    {
        private readonly ILogger<HttpClientUtils> _logger;
        private readonly HttpClient _httpClient;
        private readonly VendorConfig _vendor;
        public HttpClientUtils(ILogger<HttpClientUtils> logger, IHttpClientFactory httpClientFactory, ProxyConfig proxy, VendorConfig vendor)
        {
            _logger = logger;
            _vendor = vendor;

            #region Setup HttpClient
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            if (proxy.isEnable)
            {
                handler.Proxy = new WebProxy()
                {
                    Address = new Uri(proxy.Address ?? throw new ArgumentException("Missing Proxy Url")),
                    BypassProxyOnLocal = proxy.BypassProxyOnLocal,
                    UseDefaultCredentials = proxy.UseDefaultCredentials
                };
            }
            _httpClient = httpClientFactory.CreateClient();
            _httpClient = new HttpClient(handler);
            #endregion
        }
        public async Task<dynamic> GetAsync(string Url)
        {
            try
            {
                var apiUrl = $"{_vendor.ApiUrl}{Url}";
                var response = await _httpClient.GetAsync(apiUrl);
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"Url: {apiUrl}, StatusCode: {response.StatusCode} , Result: {content}");
                response.EnsureSuccessStatusCode();
                return content;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, nameof(GetAsync));
                throw;
            }
        }
        public async Task<dynamic> PostAsync(string Url, string body)
        {
            try
            {
                var requestBody = new StringContent(body, Encoding.UTF8, "application/json");
                var apiUrl = $"{_vendor.ApiUrl}{Url}";
                var response = await _httpClient.PostAsync(apiUrl, requestBody);
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"Url: {apiUrl}, StatusCode: {response.StatusCode} , Result: {content}");
                response.EnsureSuccessStatusCode();
                return content;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, nameof(PostAsync));
                throw;
            }
        }
        public async Task<dynamic> PostFormDataAsync(string Url, IFormCollection data)
        {
            try
            {
                using (var request = new HttpRequestMessage())
                {
                    var multiFormData = new MultipartFormDataContent();
                    request.Method = HttpMethod.Post;
                    foreach (var item in data)
                    {
                        multiFormData.Add(new StringContent(item.Value), item.Key);
                    }
                    var apiUrl = $"{_vendor.ApiUrl}{Url}";
                    request.RequestUri = new Uri(apiUrl);
                    request.Content = multiFormData;
                    var response = await _httpClient.SendAsync(request);
                    var content = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation($"Url: {apiUrl}, StatusCode: {response.StatusCode} , Result: {content}");
                    response.EnsureSuccessStatusCode();
                    return content;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, nameof(PostFormDataAsync));
                throw;
            }
        }
    }
}
