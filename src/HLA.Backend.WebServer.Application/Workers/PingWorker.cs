using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HLA.Backend.WebServer.Application.Workers
{
    public class PingWorker : BackgroundService
    {
        private readonly ILogger<PingWorker> _logger;
        private readonly string _healthCheckUrl;

        public PingWorker(ILogger<PingWorker> logger, IConfiguration configuration)
        {
            _logger = logger;
            if (string.IsNullOrEmpty(configuration?.GetSection("HealthCheckUrl").Value))
            {
                _healthCheckUrl = string.Empty;
            }
            else
            {
                _healthCheckUrl = configuration.GetSection("HealthCheckUrl").Value ?? string.Empty;
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                //ping every 5 minutes
                await Task.Delay(300 * 1000, stoppingToken);
                if (!string.IsNullOrEmpty(_healthCheckUrl))
                {
                    using (var handler = new HttpClientHandler())
                    {
                        handler.ServerCertificateCustomValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
                        using (var httpClient = new HttpClient(handler))
                        {
                            try
                            {
                                var result = await httpClient.GetAsync(_healthCheckUrl, stoppingToken);
                                _logger.LogInformation($"Ping at {DateTime.UtcNow:s}, HttpStatus: {result.StatusCode}");
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex.Message);
                            }
                        }
                    }
                }
            }
        }
    }
}
