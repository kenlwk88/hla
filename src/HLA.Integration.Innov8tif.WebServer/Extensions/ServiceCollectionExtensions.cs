using HLA.Backend.Core.Domain.Auth;
using HLA.Integration.Innov8tif.WebServer.Application;
using HLA.Integration.Innov8tif.WebServer.Application.Implementation;
using HLA.Integration.Innov8tif.WebServer.Application.Util;
using HLA.Integration.Innov8tif.WebServer.Domain;
using HLA.Integration.Innov8tif.WebServer.Filter.Security;
using Serilog;

namespace HLA.Integration.Innov8tif.WebServer.Extensions
{
    /// <summary>
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <returns></returns>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <returns></returns>
        public static void AddApplicationServices(this IServiceCollection services, WebApplicationBuilder builder, IConfiguration config)
        {
            AddSecurity(services, config);
            AddSeriLog(builder, config);
            AddServices(services);
            AddConfiguration(services, config);
        }
        /// <summary>
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <returns></returns>
        public static void AddSecurity(IServiceCollection services, IConfiguration config)
        {
            services.AddSingleton<ApiKeyAuthorizationFilter>();
            services.AddSingleton<IApiKeyValidator, ApiKeyValidator>();
            var _authConfig = new AuthConfig();
            config.Bind("Auth", _authConfig.Auth);
            services.AddSingleton<AuthConfig>(_authConfig);
        }
        /// <summary>
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <returns></returns>
        public static void AddSeriLog(this WebApplicationBuilder builder, IConfiguration config)
        {
            var logger = new LoggerConfiguration()
              .ReadFrom.Configuration(config)
              .CreateLogger();
            builder.Logging.ClearProviders();
            builder.Logging.AddSerilog(logger);
        }
        /// <summary>
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <returns></returns>
        public static void AddServices(IServiceCollection services)
        {
            services.AddTransient<IEkycServices, EkycServices>();
            services.AddSingleton<HttpClientUtils>();
        }
        /// <summary>
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <returns></returns>
        public static void AddConfiguration(this IServiceCollection services, IConfiguration config)
        {
            //Proxy
            ProxyConfig _proxy = new ProxyConfig();
            config.Bind("Proxy",_proxy);
            services.AddSingleton<ProxyConfig>(_proxy);

            //Vendor
            VendorConfig _vendor = new VendorConfig();
            config.Bind("Innov8tif", _vendor);
            services.AddSingleton<VendorConfig>(_vendor);
        }
    }
}
